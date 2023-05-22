﻿using Photon.Pun;
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
        if (!isInit) return;
        // Eğer mevcut pozisyon ile hedef pozisyon arasındaki mesafe 0'dan büyük ise
        if (Vector2.Distance(transform.position, targetPos) > 0)
        {
            // Transform pozisyonunu, hedef pozisyona doğru belirli bir hızla hareket ettir
            transform.position = Vector2.MoveTowards(transform.position, targetPos, Time.deltaTime * movementSpeed);
        }
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


    // Aşırı yükleme (overloading)
    public void Push(Vector2 targetPos)
    {
        // Hedef pozisyonunu verilen hedef pozisyonuyla değiştir
        this.targetPos = targetPos;
    }

    public void DoExplotion()
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

        GameManager.Instance.localPlayer.AddRemainingBomb();
        Destroy(gameObject);
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
    }


}
