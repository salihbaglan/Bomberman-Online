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
    public bool isKey = false; // Anahtar m�?
    public bool isGrounded = true;
    public UnityEvent canDropBomb = new UnityEvent();

    public Vector2 Direction { get { return direction; } } // Hareket y�n�
    public float speed = 2.5f; // Hareket h�z�
    private float maxSpeed = 4f; // Maksimum hareket h�z�

    public GameObject GhostGo; // Hayalet nesnesi
    public GameObject SpeedGo; // H�z nesnesi


    [Header("Sprites")]
    public AnimatedSpriteRenderer spriteRendererUp; // Yukar� d�n�k animasyonlu sprite renderer
    public AnimatedSpriteRenderer spriteRendererDown; // A�a�� d�n�k animasyonlu sprite renderer
    public AnimatedSpriteRenderer spriteRendererLeft; // Sola d�n�k animasyonlu sprite renderer
    public AnimatedSpriteRenderer spriteRendererRight; // Sa�a d�n�k animasyonlu sprite renderer
    public AnimatedSpriteRenderer spriteRendererDeath; // �l�m animasyonlu sprite renderer
    private AnimatedSpriteRenderer activeSpriteRenderer; // Aktif sprite renderer
    [SerializeField] private float moveDistance = 1; // Hareket mesafesi
    [SerializeField] private LayerMask wallLayer; // Duvar katman�
    private float ghostDuration = 5f;//isGhoos'tu false �evir
    PhotonView view;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        activeSpriteRenderer = spriteRendererDown; // Ba�lang��ta a�a�� d�n�k sprite renderer'� aktif olarak ayarla
    }

    private void Start()
    {
        view = GetComponent<PhotonView>();
    }
    private void Update()
    {
        SetDirection(InputManager.Instance.Move); // Hareket y�n�n� ayarla       c
    }

    private void FixedUpdate()
    {
        // Y�n tu�lar�na bas�lm�yorsa fonksiyondan ��k.
        if (direction == Vector2.zero) return;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, moveDistance, wallLayer);
        // �n�mde duvar yoksa veya hayalet modunda ise (isGhost).
        if (hit.collider == null || isGhost)
        {
            Move(); // Hareket et
        }

    }




    /// <summary>
    /// Karakterin hareket etmesini sa�lar.
    /// </summary>
    /// 
    private void Move()
    {
        Vector2 position = rigidbody.position; // Karakterin �u anki konumu
        Vector2 translation = direction * speed * Time.fixedDeltaTime; // Karakterin yapmas� gereken yer de�i�tirme
        rigidbody.MovePosition(position + translation); // Karakterin konumunu g�ncelle
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
    //isGhostu false �evir
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


    //H�z� 5 Sniyeli�ine 8 e e�itleme kodu
    IEnumerator SetSpeed(float maxSpeed, float duration)
    {
        SpeedGo.SetActive(true);
        float oldSpeed = speed;
        speed = maxSpeed;
        yield return new WaitForSeconds(duration);
        speed = 2.5f;
        SpeedGo.SetActive(false);


    }

    //�a��ralan item
    public void SpeedItem()
    {
        StartCoroutine(SetSpeed(4f, 8f));
    }

    public void Key�tem()
    {
        isKey = true;
    }

}
