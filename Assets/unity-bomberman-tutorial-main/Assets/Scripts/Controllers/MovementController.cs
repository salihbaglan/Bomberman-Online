using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;


[RequireComponent(typeof(Rigidbody2D))]
public class MovementController : Singleton<MovementController>
{
    private new Rigidbody2D rigidbody;
    private Vector2 direction = Vector2.down;
    public static bool isGhost = false; // Hayalet mi?
    public bool isKey = false; // Anahtar mý?
    public bool isGrounded = true;
    public UnityEvent canDropBomb = new UnityEvent();

    public Vector2 Direction { get { return direction; } } // Hareket yönü
    public float speed = 2.5f; // Hareket hýzý
    private float maxSpeed = 4f; // Maksimum hareket hýzý

    public GameObject GhostGo; // Hayalet nesnesi
    public GameObject SpeedGo; // Hýz nesnesi


    [Header("Sprites")]
    public AnimatedSpriteRenderer spriteRendererUp; // Yukarý dönük animasyonlu sprite renderer
    public AnimatedSpriteRenderer spriteRendererDown; // Aþaðý dönük animasyonlu sprite renderer
    public AnimatedSpriteRenderer spriteRendererLeft; // Sola dönük animasyonlu sprite renderer
    public AnimatedSpriteRenderer spriteRendererRight; // Saða dönük animasyonlu sprite renderer
    public AnimatedSpriteRenderer spriteRendererDeath; // Ölüm animasyonlu sprite renderer
    private AnimatedSpriteRenderer activeSpriteRenderer; // Aktif sprite renderer
    [SerializeField] private float moveDistance = 1; // Hareket mesafesi
    [SerializeField] private LayerMask wallLayer; // Duvar katmaný
    private float ghostDuration = 5f;//isGhoos'tu false çevir
    PhotonView view;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        activeSpriteRenderer = spriteRendererDown; // Baþlangýçta aþaðý dönük sprite renderer'ý aktif olarak ayarla
    }

    private void Start()
    {
        view = GetComponent<PhotonView>();
    }
    private void Update()
    {
        SetDirection(InputManager.Instance.Move); // Hareket yönünü ayarla       c
    }

    private void FixedUpdate()
    {
        // Yön tuþlarýna basýlmýyorsa fonksiyondan çýk.
        if (direction == Vector2.zero) return;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, moveDistance, wallLayer);
        // Önümde duvar yoksa veya hayalet modunda ise (isGhost).
        if (hit.collider == null || isGhost)
        {
            Move(); // Hareket et
        }

    }




    /// <summary>
    /// Karakterin hareket etmesini saðlar.
    /// </summary>
    /// 
    private void Move()
    {
        Vector2 position = rigidbody.position; // Karakterin þu anki konumu
        Vector2 translation = direction * speed * Time.fixedDeltaTime; // Karakterin yapmasý gereken yer deðiþtirme
        rigidbody.MovePosition(position + translation); // Karakterin konumunu güncelle
    }

    [SerializeField]
    private void SetDirection(Vector2 newDirection)
    {
        if (view.IsMine)
        {
            AnimatedSpriteRenderer spriteRenderer;

            if (newDirection == Vector2.up)
            {
                spriteRenderer = spriteRendererUp;
            }
            else if (newDirection == Vector2.down)
            {
                spriteRenderer = spriteRendererDown;
            }
            else if (newDirection == Vector2.left)
            {
                spriteRenderer = spriteRendererLeft;
            }
            else if (newDirection == Vector2.right)
            {
                spriteRenderer = spriteRendererRight;
            }
            else
            {
                spriteRenderer = activeSpriteRenderer;
            }

            direction = newDirection;

            spriteRendererUp.enabled = spriteRenderer == spriteRendererUp;
            spriteRendererDown.enabled = spriteRenderer == spriteRendererDown;
            spriteRendererLeft.enabled = spriteRenderer == spriteRendererLeft;
            spriteRendererRight.enabled = spriteRenderer == spriteRendererRight;

            activeSpriteRenderer = spriteRenderer;
            activeSpriteRenderer.idle = direction == Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Explosion"))
        {
            DeathSequence();

        }

    }


    private void DeathSequence()
    {
        enabled = false;
        GetComponent<BombController>().enabled = false;

        spriteRendererUp.enabled = false;
        spriteRendererDown.enabled = false;
        spriteRendererLeft.enabled = false;
        spriteRendererRight.enabled = false;
        spriteRendererDeath.enabled = true;

        Invoke(nameof(OnDeathSequenceEnded), 1.25f);
    }

    private void OnDeathSequenceEnded()
    {
        gameObject.SetActive(false);
    }

    public void Ghost()
    {
        isGhost = true;
        GhostGo.SetActive(true);
        StartCoroutine(DisableGhostAfterDelay());
    }
    //isGhostu false çevir
    private IEnumerator DisableGhostAfterDelay()
    {
        yield return new WaitForSeconds(ghostDuration);
        GhostGo.SetActive(false);

        Vector2 RoundedPosition;
        var bombControler = GetComponent<BombController>();
        Collider2D hit;




        isGhost = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") && !isGhost)
        {
            rigidbody.AddForce(direction * speed * Time.fixedDeltaTime, ForceMode2D.Impulse);

        }

    }


    //Hýzý 5 Sniyeliðine 8 e eþitleme kodu
    IEnumerator SetSpeed(float maxSpeed, float duration)
    {
        SpeedGo.SetActive(true);
        float oldSpeed = speed;
        speed = maxSpeed;
        yield return new WaitForSeconds(duration);
        speed = 2.5f;
        SpeedGo.SetActive(false);


    }

    //çaðýralan item
    public void SpeedItem()
    {
        StartCoroutine(SetSpeed(4f, 8f));
    }

    public void Keyitem()
    {
        isKey = true;
    }

}
