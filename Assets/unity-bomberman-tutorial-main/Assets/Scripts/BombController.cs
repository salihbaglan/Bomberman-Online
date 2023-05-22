using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class BombController : Singleton<BombController>
{

    public GameObject bombPrefab; // Bomba prefabý
    public float bombFuseTime = 3f; // Bomba fitil süresi
    public int bombAmount = 1; // Sahip olunan bomba sayýsý
    private int bombsRemaining; // Kullanýlabilir bomba sayýsý
    [SerializeField] private int maxBombAmount = 6; // Maksimum bomba sayýsý
    public bool ExplosionButton = false; // Patlama tuþu
    public bool isActiveController = true; // Kontrolcü aktif mi?

    [Header("Explosion")]
    public Explosion explosionPrefab; // Patlama prefabý
    public LayerMask explosionLayerMask; // Patlama katmaný maskesi
    public float explosionDuration = 1f; // Patlama süresi
    public int explosionRadius = 1; // Patlama yarýçapý
    [SerializeField] private int MaxexplosionRadius = 7; // Maksimum patlama yarýçapý
    public float bombDropInterval = 0.9f; // Bomba býrakma aralýðý
    public float bombDropDuration = 7f; // Bomba býrakma süresi

    [Header("Destructible")]
    public Tilemap destructibleTiles; // Yýkýlabilir tilemap
    public Destructible destructiblePrefab; // Yýkýlabilir prefab

    public bool canIPush = false; // Ýterek itme özelliði
    [SerializeField] private LayerMask bombPushLayers; // Ýterek itme katmaný
    private List<GameObject> bombs = new List<GameObject>(); // Býrakýlan bombalarýn listesi

    public GameObject PushGo;
    public GameObject BombDropGo;

    public float placeBombRadius = 0.5f;
    public LayerMask bombCantPlaceLayers;
    private void Start()
    {
        InputManager.Instance.OnClickBomb.AddListener(OnClickBomb);
        InputManager.Instance.OnLeavekBomb.AddListener(StartCorutineBomb);
    }
    private void OnEnable()
    {
        bombsRemaining = bombAmount;


    }

    private void OnDisable()
    {
        StopAllCoroutines();
        InputManager.Instance.OnClickBomb.RemoveListener(OnClickBomb);
        InputManager.Instance.OnLeavekBomb.RemoveListener(StartCorutineBomb);

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
        if (!ExplosionButton) return;
        var copyOfBombs = bombs.ToArray();
        foreach (var bomb in copyOfBombs)
        {
            Bombed(bomb);
        }
    }

    // Bomba yerleþtirme iþlemi
    public IEnumerator PlaceBomb()
    {

        Vector2 bombPosition = transform.position;
        bombPosition.x = Mathf.Round(bombPosition.x);
        bombPosition.y = Mathf.Round(bombPosition.y);




        // Duvarýn üzerindeyken bomba býrakmayý engelle
        Collider2D hit = Physics2D.OverlapCircle(bombPosition, placeBombRadius, bombCantPlaceLayers);

        if (hit == null)
        {
            // Bombayý ekle ve diðer iþlemleri yap
            GameObject bomb = Instantiate(bombPrefab, bombPosition, Quaternion.identity);
            bombs.Add(bomb);
            bombAmount--;
            bombsRemaining--;

            if (!ExplosionButton)
            {
                yield return new WaitForSeconds(bombFuseTime);
                Bombed(bomb);
            }
        }

    }

    private void OnDrawGizmos()
    {
        Vector2 bombPosition = transform.position;
        bombPosition.x = Mathf.Round(bombPosition.x);
        bombPosition.y = Mathf.Round(bombPosition.y);


        Gizmos.color = Color.red;

        Gizmos.DrawSphere(bombPosition, placeBombRadius);
    }

    private void Bombed(GameObject bomb)
    {
        // Bombanýn bulunduðu konumu al
        Vector2 position = bomb.transform.position;

        // Patlama efektini oluþtur ve baþlangýç durumunu ayarla
        Explosion explosion1 = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion1.SetActiveRenderer(explosion1.start);
        explosion1.DestroyAfter(explosionDuration);

        // Patlamayý yukarý, aþaðý, sola ve saða doðru geniþlet
        Explode(position, Vector2.up, explosionRadius);
        Explode(position, Vector2.down, explosionRadius);
        Explode(position, Vector2.left, explosionRadius);
        Explode(position, Vector2.right, explosionRadius);

        // Bomba sayýsýný ve kalan bomba sayýsýný artýr
        bombAmount++;
        bombsRemaining++;

        // Yok olan bombayý listeden kaldýr ve yok et
        bombs.Remove(bomb);
        Destroy(bomb.gameObject);
    }

    private void Explode(Vector2 position, Vector2 direction, int length)
    {
        // Patlama uzunluðu 0 veya daha küçük ise iþlemi sonlandýr
        if (length <= 0)
        {
            return;
        }


        // Yeni pozisyonu hesapla
        position += direction;
        var hit = Physics2D.OverlapBox(position, Vector2.one / 2f, 0f, explosionLayerMask);
        // Patlama sýrasýnda engelle çarpýþma var mý kontrol et
        if (hit && !hit.CompareTag("Player"))
        {
            if (!hit.CompareTag("DontDestroy"))
            {
                // Eðer varsa yýkýlabilir tile'ý temizle
                ClearDestructible(position, hit);
            }
            return;
        }

        // Patlama efektini oluþtur ve aktif durumu ayarla
        Explosion explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion.SetActiveRenderer(length > 1 ? explosion.middle : explosion.end);
        explosion.SetDirection(direction);
        explosion.DestroyAfter(explosionDuration);

        // Patlamayý geniþletmek için Explode fonksiyonunu tekrar çaðýr
        Explode(position, direction, length - 1);
    }

    private void ClearDestructible(Vector2 position, Collider2D hit)
    {
        Instantiate(destructiblePrefab, position, Quaternion.identity);
        hit.gameObject.SetActive(false);
        StartCoroutine(SpawnWalls(hit.gameObject));
    }

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Explosion"))
        {

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
        StartCoroutine(DropBombsRoutine());
        BombDropGo.SetActive(true);
    }

    private IEnumerator DropBombsRoutine()
    {
        // Bomba býrakma iþlemini belirli bir süre boyunca tekrarla
        float timer = 0f;
        while (timer < bombDropDuration)
        {
            yield return new WaitForSeconds(bombDropInterval); // Belirlenen aralýklarla bomba býrak

            // Eðer hala bomba sayýsý varsa bomba býrakma iþlemini baþlat
            if (bombAmount > 0)
            {
                StartCoroutine(PlaceBomb()); // Bomba býrakma kodunu çalýþtýr
            }

            timer += bombDropInterval;
        }
        BombDropGo.SetActive(false);
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
        canIPush = true;
        PushGo.SetActive(true);
    }

    public void ExplosionButtonItem()
    {
        // Patlama düðmesini etkinleþtir
        ExplosionButton = true;
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


    public void StartCorutineBomb()
    {

        if (bombsRemaining > 0)
        {
            StartCoroutine(PlaceBomb());
        }

    }

}

