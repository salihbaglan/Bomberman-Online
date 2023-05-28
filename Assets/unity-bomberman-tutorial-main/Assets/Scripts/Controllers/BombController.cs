using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class BombController : MonoBehaviourPunCallbacks
{
    [Header("Explotion")]
    public int explosionRadius = 1; // Patlama yar��ap�
    [SerializeField] private int MaxexplosionRadius = 7; // Maksimum patlama yar��ap�


    [Header("Bomb Drop")]
    public float bombDropInterval = 0.9f; // Bomba b�rakma aral���
    public float bombDropDuration = 7f; // Bomba b�rakma s�resi

    public GameObject bombPrefab; // Bomba prefab�
    public float bombFuseTime = 3f; // Bomba fitil s�resi
    public int bombAmount = 1; // Sahip olunan bomba say�s�
    private int bombsRemaining; // Kullan�labilir bomba say�s�
    [SerializeField] private int maxBombAmount = 6; // Maksimum bomba say�s�
    public bool ExplosionButton = false; // Patlama tu�u
    public bool isActiveController = true; // Kontrolc� aktif mi?





    public bool canIPush = false; // �terek itme �zelli�i
    [SerializeField] private LayerMask bombPushLayers; // �terek itme katman�
    private List<GameObject> bombs = new List<GameObject>(); // B�rak�lan bombalar�n listesi

    public GameObject PushGo;
    public GameObject BombDropGo;

    public float placeBombRadius = 0.5f;
    public LayerMask bombCantPlaceLayers;

    private void Start()
    {

    }
    private void OnEnable()
    {
        if (photonView.IsMine)
        {
            InputManager.Instance.OnClickBomb.AddListener(OnClickBomb);
            InputManager.Instance.OnLeavekBomb.AddListener(DropBomb);
        }
        bombsRemaining = bombAmount;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        InputManager.Instance.OnClickBomb.RemoveListener(OnClickBomb);
        InputManager.Instance.OnLeavekBomb.RemoveListener(DropBomb);

    }

    private void Update()
    {


        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnClickBomb();
        }

    }

    // Patlama tu�una bas�ld���nda t�m bombalar� patlat�r
    public void OnClickBomb()
    {
        //if (!ExplosionButton) return;
        var copyOfBombs = bombs.ToArray();
        foreach (var bomb in copyOfBombs)
        {
            bomb.GetComponent<Bomb>().DoExplotion();
        }
    }

    // Bomba yerle�tirme i�lemi
    public void PlaceBomb()
    {

        Vector2 bombPosition = transform.position;
        bombPosition.x = Mathf.Round(bombPosition.x);
        bombPosition.y = Mathf.Round(bombPosition.y);

        // Duvar�n �zerindeyken bomba b�rakmay� engelle
        Collider2D hit = Physics2D.OverlapCircle(bombPosition, placeBombRadius, bombCantPlaceLayers);
        if (hit == null)
        {
            // Bombay� ekle ve di�er i�lemleri yap
            var data = new object[] { bombFuseTime, explosionRadius };
            var bomb = PhotonNetwork.Instantiate(bombPrefab.name, bombPosition, Quaternion.identity, 0, data);
            bombs.Add(bomb);
            bombsRemaining--;
        }
    }
    public void AddRemainingBomb()
    {
        bombsRemaining++;
    }
    private void OnDrawGizmos()
    {
        Vector2 bombPosition = transform.position;
        bombPosition.x = Mathf.Round(bombPosition.x);
        bombPosition.y = Mathf.Round(bombPosition.y);


        Gizmos.color = Color.red;

        Gizmos.DrawSphere(bombPosition, placeBombRadius);
    }

    internal void RemovoBomb(Bomb bomb)
    {
        if (bombs.Contains(bomb.gameObject))
            bombs.Remove(bomb.gameObject);
    }







    // TODO: spawn wall mechanic add game manager
    private IEnumerator SpawnWalls(GameObject wall)
    {
        yield return new WaitForSeconds(60f);
        Vector2 RoundedPosition;
        Collider2D hit;


        do
        {
            RoundedPosition = wall.transform.position;
            RoundedPosition.x = Mathf.Round(RoundedPosition.x);
            RoundedPosition.y = Mathf.Round(RoundedPosition.y);
            hit = Physics2D.OverlapCircle(RoundedPosition, 0.3f);
            Debug.Log("Bi�ey Var");
            yield return new WaitForSeconds(1f);
        } while (hit != null);
        wall.SetActive(true);
    }
    public void AddBomb()
    {
        // Bomba say�s�n� art�r, maksimum say�y� ge�miyorsa art��� uygula
        bombAmount++;
        if (bombAmount > maxBombAmount)
        {
            bombAmount = maxBombAmount;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Tetikleyiciden ��kan nesne bir bombaysa tetikleyici �zelli�ini kapat
        if (other.gameObject.layer == LayerMask.NameToLayer("Bomb"))
        {
            other.isTrigger = false;
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �arp��an nesne bir bombaysa ve itebilme �zelli�i aktif ise
        if (collision.gameObject.CompareTag("Bomb") && canIPush)
        {
            // E�er hareket kontrolc�s� bile�eni varsa
            if (TryGetComponent<MovementController>(out var movementController))
            {
                // �tme i�lemi i�in gerekli de�i�kenleri tan�mla
                var radius = 0.6f;
                var direction = movementController.Direction;
                var startPos = (Vector2)collision.transform.position + direction * radius;
                RaycastHit2D hit = Physics2D.Raycast(startPos, direction, 500, bombPushLayers);

                Debug.Log($"bombay� it direction:{direction}");

                // E�er itme i�lemi s�ras�nda ba�ka bir nesne ile temas olduysa
                if (hit.collider != null)
                {
                    // Hedef pozisyonu belirle ve bomban�n "Push" metodunu �a��rarak itme i�lemini ger�ekle�tir
                    var targetPos = hit.point - direction * 0.5f;
                    collision.transform.GetComponent<Bomb>().Push(targetPos);
                }
            }
        }
    }

    // �oklu Bomba �temi fonksiyonu
    public void OnItemEaten()
    {
        UIManager.Instance.ShowItemIndicattor(ItemPickup.ItemType.MultiBomb);
        StartCoroutine(DropBombsRoutine());
    }

    private IEnumerator DropBombsRoutine()
    {
        // Bomba b�rakma i�lemini belirli bir s�re boyunca tekrarla
        float timer = 0f;
        while (timer < bombDropDuration)
        {
            yield return new WaitForSeconds(bombDropInterval); // Belirlenen aral�klarla bomba b�rak

            // E�er hala bomba say�s� varsa bomba b�rakma i�lemini ba�lat
            if (bombsRemaining > 0)
            {
                PlaceBomb(); // Bomba b�rakma kodunu �al��t�r
            }

            timer += bombDropInterval;
        }
        UIManager.Instance.HideItemIndicattor(ItemPickup.ItemType.MultiBomb);
    }

    public void ExplosionRadius()
    {
        // Patlama yar��ap�n� art�r, maksimum de�eri a�m�yorsa art��� uygula
        explosionRadius++;
        if (explosionRadius > MaxexplosionRadius)
        {
            explosionRadius = MaxexplosionRadius;
        }
    }

    public void MaxExplosionRadius()
    {
        // Patlama yar��ap�n� maksimum de�ere ayarla, maksimum de�eri a�m�yorsa ayar� uygula
        explosionRadius = 6;
        if (explosionRadius > MaxexplosionRadius)
        {
            explosionRadius = MaxexplosionRadius;
        }
    }

    public void Push()
    {
        // �tme �zelli�ini etkinle�tir
        UIManager.Instance.ShowItemIndicattor(ItemPickup.ItemType.PushItem);
        canIPush = true;
    }

    [PunRPC]
    public void ExplosionButtonItem()
    {
        if (photonView.IsMine)
        {
            // Patlama d��mesini etkinle�tir
            UIManager.Instance.ShowItemIndicattor(ItemPickup.ItemType.isActiveBombControl);
            ExplosionButton = true;
        }

    }

    public void CheckActiveButton()
    {
        // E�er "B" tu�una bas�ld�ysa patlama d��mesini etkinle�tir ve bomba fitil s�resini ayarla
        if (Input.GetKeyDown(KeyCode.B))
        {
            ExplosionButton = true;
            bombFuseTime = 0.01f;
        }
    }


    public void DropBomb()
    {

        if (bombsRemaining > 0)
        {
            PlaceBomb();
        }

    }

    public void Reset()
    {
        canIPush = false;
        explosionRadius = 1;
        bombAmount = 1;
        bombsRemaining = 1;
        UIManager.Instance.HideItemIndicattor(ItemPickup.ItemType.PushItem);
    }
}

