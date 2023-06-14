using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bomb : MonoBehaviourPunCallbacks
{

    [Header("Explosion")]
    public Explosion explosionPrefab; // Patlama prefabý
    public LayerMask explosionLayerMask; // Patlama katmaný maskesi
    public float explosionDuration = 1f; // Patlama süresi
    public int explosionRadius = 1; // Patlama yarýçapý
    [SerializeField] private int MaxexplosionRadius = 7; // Maksimum patlama yarýçapý


    [Header("Destructible")]
    public Destructible destructiblePrefab; // Yýkýlabilir prefab

    private Vector2 targetPos = Vector2.zero;
    [SerializeField] private float movementSpeed = 1;
    [SerializeField] private float lifeTime = 1;
    private bool isInit = false;
    private void Start()
    {
        // Başlangıçta hedef pozisyonu mevcut pozisyona eşitle
        targetPos = transform.position;
        object[] instantiationData = photonView.InstantiationData;
        if (instantiationData != null && instantiationData.Length > 0)
        {
            Initialize((float)instantiationData[0], (int)instantiationData[1]);
        }
    }

    public void Initialize(float bombFuseTime, int radius)
    {
        lifeTime = bombFuseTime;
        explosionRadius = radius;
        isInit = true;
    }

    // Her karede bir kez çağrılan Update fonksiyonu
    public void Update()
    {
        if (!isInit || !photonView.IsMine) return;
        // Eğer mevcut pozisyon ile hedef pozisyon arasındaki mesafe 0'dan büyük ise
        if (Vector2.Distance(transform.position, targetPos) > 0)
        {
            // Transform pozisyonunu, hedef pozisyona doğru belirli bir hızla hareket ettir
            transform.position = Vector2.MoveTowards(transform.position, targetPos, Time.deltaTime * movementSpeed);
        }
        var IsExplotionButtonActive = UIManager.Instance.ExplotionButton.activeSelf;
        if (!IsExplotionButtonActive) Life();
    }


    private void Life()
    {
        if (lifeTime > 0)
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0)
            {
                lifeTime = 0;
                DoExplotion();
            }
        }
    }

    public void DoExplotion()
    {
        photonView.RPC("DoExplotionRPC", RpcTarget.All);
        GameManager.Instance.localPlayer.AddRemainingBomb();
        GameManager.Instance.localPlayer.GetComponent<BombController>().RemovoBomb(this);
    }


    // Aşırı yükleme (overloading)
    public void Push(Vector2 targetPos)
    {
        // Hedef pozisyonunu verilen hedef pozisyonuyla değiştir
        this.targetPos = targetPos;
    }


    [PunRPC]
    public void DoExplotionRPC()
    {
        // Bombanýn bulunduðu konumu al
        Vector2 position = transform.position;

        // Patlama efektini oluþtur ve baþlangýç durumunu ayarla
        Explosion explosion1 = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion1.SetActiveRenderer(explosion1.start);
        explosion1.DestroyAfter(explosionDuration);

        // Patlamayý yukarý, aþaðý, sola ve saða doðru geniþlet
        Explode(position, Vector2.up, explosionRadius);
        Explode(position, Vector2.down, explosionRadius);
        Explode(position, Vector2.left, explosionRadius);
        Explode(position, Vector2.right, explosionRadius);
        Destroy(gameObject);
    }

    private void Explode(Vector2 position, Vector2 direction, int length)
    {
        if (length <= 0)
        {
            return;
        }

        position += direction;
        var hit = Physics2D.OverlapBox(position, Vector2.one / 2f, 0f, explosionLayerMask);

        if (hit && hit.CompareTag("Bomb"))  // Çarpışan nesne "Bomb" etiketine sahip mi?
        {
            Debug.Log("bombaya çarptı");
        }
        else if (hit && !hit.CompareTag("Player"))
        {
            if (!hit.CompareTag("DontDestroy"))
            {
                ClearDestructible(position, hit);
            }
            return;
        }

        Explosion explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion.SetActiveRenderer(length > 1 ? explosion.middle : explosion.end);
        explosion.SetDirection(direction);
        explosion.DestroyAfter(explosionDuration);

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
        yield return new WaitForSeconds(5f);
        Vector2 RoundedPosition;
        Collider2D hit;


        do
        {
            RoundedPosition = wall.transform.position;
            RoundedPosition.x = Mathf.Round(RoundedPosition.x);
            RoundedPosition.y = Mathf.Round(RoundedPosition.y);
            hit = Physics2D.OverlapCircle(RoundedPosition, 0.3f);
            yield return new WaitForSeconds(1f);
        } while (hit != null);
        wall.SetActive(true);
    }



}
