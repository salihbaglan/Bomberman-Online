using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class BombController : MonoBehaviourPunCallbacks
{
    [Header("Explotion")]
    public int explosionRadius = 1; // Patlama yarýçapý
    [SerializeField] private int MaxexplosionRadius = 7; // Maksimum patlama yarýçapý


    [Header("Bomb Drop")]
    public float bombDropInterval = 0.9f; // Bomba býrakma aralýðý
    public float bombDropDuration = 7f; // Bomba býrakma süresi

    public GameObject bombPrefab; // Bomba prefabý
    public float bombFuseTime = 3f; // Bomba fitil süresi
    public int bombAmount = 1; // Sahip olunan bomba sayýsý
    private int bombsRemaining; // Kullanýlabilir bomba sayýsý
    [SerializeField] private int maxBombAmount = 6; // Maksimum bomba sayýsý
    public bool ExplosionButton = false; // Patlama tuþu
    public bool isActiveController = true; // Kontrolcü aktif mi?





    public bool canIPush = false; // Ýterek itme özelliði
    [SerializeField] private LayerMask bombPushLayers; // Ýterek itme katmaný
    private List<GameObject> bombs = new List<GameObject>(); // Býrakýlan bombalarýn listesi

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

    // Patlama tuþuna basýldýðýnda tüm bombalarý patlatýr
    public void OnClickBomb()
    {
        //if (!ExplosionButton) return;
        var copyOfBombs = bombs.ToArray();
        foreach (var bomb in copyOfBombs)
        {
            bomb.GetComponent<Bomb>().DoExplotion();
        }
    }

    // Bomba yerleþtirme iþlemi
    public void PlaceBomb()
    {

        Vector2 bombPosition = transform.position;
        bombPosition.x = Mathf.Round(bombPosition.x);
        bombPosition.y = Mathf.Round(bombPosition.y);

        // Duvarýn üzerindeyken bomba býrakmayý engelle
        Collider2D hit = Physics2D.OverlapCircle(bombPosition, placeBombRadius, bombCantPlaceLayers);
        if (hit == null)
        {
            // Bombayý ekle ve diðer iþlemleri yap
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
            Debug.Log("Biþey Var");
            yield return new WaitForSeconds(1f);
        } while (hit != null);
        wall.SetActive(true);
    }
    public void AddBomb()
    {
        // Bomba sayýsýný artýr, maksimum sayýyý geçmiyorsa artýþý uygula
        bombAmount++;
        if (bombAmount > maxBombAmount)
        {
            bombAmount = maxBombAmount;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Tetikleyiciden çýkan nesne bir bombaysa tetikleyici özelliðini kapat
        if (other.gameObject.layer == LayerMask.NameToLayer("Bomb"))
        {
            other.isTrigger = false;
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Çarpýþan nesne bir bombaysa ve itebilme özelliði aktif ise
        if (collision.gameObject.CompareTag("Bomb") && canIPush)
        {
            // Eðer hareket kontrolcüsü bileþeni varsa
            if (TryGetComponent<MovementController>(out var movementController))
            {
                // Ýtme iþlemi için gerekli deðiþkenleri tanýmla
                var radius = 0.6f;
                var direction = movementController.Direction;
                var startPos = (Vector2)collision.transform.position + direction * radius;
                RaycastHit2D hit = Physics2D.Raycast(startPos, direction, 500, bombPushLayers);

                Debug.Log($"bombayý it direction:{direction}");

                // Eðer itme iþlemi sýrasýnda baþka bir nesne ile temas olduysa
                if (hit.collider != null)
                {
                    // Hedef pozisyonu belirle ve bombanýn "Push" metodunu çaðýrarak itme iþlemini gerçekleþtir
                    var targetPos = hit.point - direction * 0.5f;
                    collision.transform.GetComponent<Bomb>().Push(targetPos);
                }
            }
        }
    }

    // Çoklu Bomba Ýtemi fonksiyonu
    public void OnItemEaten()
    {
        UIManager.Instance.ShowItemIndicattor(ItemPickup.ItemType.MultiBomb);
        StartCoroutine(DropBombsRoutine());
    }

    private IEnumerator DropBombsRoutine()
    {
        // Bomba býrakma iþlemini belirli bir süre boyunca tekrarla
        float timer = 0f;
        while (timer < bombDropDuration)
        {
            yield return new WaitForSeconds(bombDropInterval); // Belirlenen aralýklarla bomba býrak

            // Eðer hala bomba sayýsý varsa bomba býrakma iþlemini baþlat
            if (bombsRemaining > 0)
            {
                PlaceBomb(); // Bomba býrakma kodunu çalýþtýr
            }

            timer += bombDropInterval;
        }
        UIManager.Instance.HideItemIndicattor(ItemPickup.ItemType.MultiBomb);
    }

    public void ExplosionRadius()
    {
        // Patlama yarýçapýný artýr, maksimum deðeri aþmýyorsa artýþý uygula
        explosionRadius++;
        if (explosionRadius > MaxexplosionRadius)
        {
            explosionRadius = MaxexplosionRadius;
        }
    }

    public void MaxExplosionRadius()
    {
        // Patlama yarýçapýný maksimum deðere ayarla, maksimum deðeri aþmýyorsa ayarý uygula
        explosionRadius = 6;
        if (explosionRadius > MaxexplosionRadius)
        {
            explosionRadius = MaxexplosionRadius;
        }
    }

    public void Push()
    {
        // Ýtme özelliðini etkinleþtir
        UIManager.Instance.ShowItemIndicattor(ItemPickup.ItemType.PushItem);
        canIPush = true;
    }

    [PunRPC]
    public void ExplosionButtonItem()
    {
        if (photonView.IsMine)
        {
            // Patlama düðmesini etkinleþtir
            UIManager.Instance.ShowItemIndicattor(ItemPickup.ItemType.isActiveBombControl);
            ExplosionButton = true;
        }

    }

    public void CheckActiveButton()
    {
        // Eðer "B" tuþuna basýldýysa patlama düðmesini etkinleþtir ve bomba fitil süresini ayarla
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

