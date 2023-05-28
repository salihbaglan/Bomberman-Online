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




    [Header("Sprites")]
    public AnimatedSpriteRenderer spriteRendererUp; // Yukar� d�n�k animasyonlu sprite renderer
    public AnimatedSpriteRenderer spriteRendererDown; // A�a�� d�n�k animasyonlu sprite renderer
    public AnimatedSpriteRenderer spriteRendererLeft; // Sola d�n�k animasyonlu sprite renderer
    public AnimatedSpriteRenderer spriteRendererRight; // Sa�a d�n�k animasyonlu sprite renderer
    public AnimatedSpriteRenderer spriteRendererRightUp; // Yukar� d�n�k animasyonlu sprite renderer
    public AnimatedSpriteRenderer spriteRendererRightDown; // A�a�� d�n�k animasyonlu sprite renderer
    public AnimatedSpriteRenderer spriteRendererLeftUp; // Sola d�n�k animasyonlu sprite renderer
    public AnimatedSpriteRenderer spriteRendererLeftDown; // Sa�a d�n�k animasyonlu sprite renderer
    public AnimatedSpriteRenderer spriteRendererDeath; // �l�m animasyonlu sprite renderer
    private AnimatedSpriteRenderer activeSpriteRenderer; // Aktif sprite renderer
    [SerializeField] private float moveDistance = 1; // Hareket mesafesi
    [SerializeField] private LayerMask wallLayer; // Duvar katman�
    private float ghostDuration = 5f;//isGhoos'tu false �evir
    PhotonView view;
    PlayerController playerController;
    UIManager uiManager;
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        uiManager = gameObject.GetComponent<UIManager>();
        rigidbody = GetComponent<Rigidbody2D>();
        activeSpriteRenderer = spriteRendererDown; // Ba�lang��ta a�a�� d�n�k sprite renderer'� aktif olarak ayarla
    }

    private void Start()
    {
        view = GetComponent<PhotonView>();
    }
    private void Update()
    {

        if (view.IsMine && !playerController.isDead) SetDirection(InputManager.Instance.Move); // Hareket y�n�n� ayarla       c

    }

    private void FixedUpdate()
    {
        // Y�n tu�lar�na bas�lm�yorsa fonksiyondan ��k.
        if (direction == Vector2.zero) return;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, moveDistance, wallLayer);
        // �n�mde duvar yoksa veya hayalet modunda ise (isGhost).
        if ((hit.collider == null || isGhost) && view.IsMine && !playerController.isDead)
        {
            Move(); // Hareket et
        }

    }


    public void Reset()
    {
        isGhost = false;
        speed = 2.5f;
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
    private int ToOne(float value)
    {
        return value > 0 ? 1 : -1;
    }
    [SerializeField]
    private void SetDirection(Vector2 newDirection)
    {
        var rightUp = Vector2.right + Vector2.up;
        var rightDown = Vector2.right + Vector2.down;
        var leftUp = Vector2.left + Vector2.up;
        var leftDown = Vector2.left + Vector2.down;

        var range = 0.3f;
        AnimatedSpriteRenderer spriteRenderer;

        var dirMove = newDirection;
        //var deger = false ? 3 : 2; 
        dirMove.x = Math.Abs(dirMove.x) >= range ? ToOne(dirMove.x) : Math.Abs(dirMove.x) > Math.Abs(dirMove.y) ? ToOne(dirMove.x) : 0;
        dirMove.y = Math.Abs(dirMove.y) >= range ? ToOne(dirMove.y) : Math.Abs(dirMove.y) > Math.Abs(dirMove.x) ? ToOne(dirMove.y) : 0;


        if (dirMove == Vector2.up)
        {
            spriteRenderer = spriteRendererUp;
        }
        else if (dirMove == Vector2.down)
        {
            spriteRenderer = spriteRendererDown;
        }
        else if (dirMove == Vector2.left)
        {
            spriteRenderer = spriteRendererLeft;
        }
        else if (dirMove == Vector2.right)
        {
            spriteRenderer = spriteRendererRight;
        }
        else if (dirMove == rightUp)
        {
            spriteRenderer = spriteRendererRightUp;
        }
        else if (dirMove == rightDown)
        {
            spriteRenderer = spriteRendererRightDown;

        }
        else if (dirMove == leftUp)
        {
            spriteRenderer = spriteRendererLeftUp;

        }
        else if (dirMove == leftDown)
        {
            spriteRenderer = spriteRendererLeftDown;

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
        spriteRendererRightUp.enabled = spriteRenderer == spriteRendererRightUp;
        spriteRendererRightDown.enabled = spriteRenderer == spriteRendererRightDown;
        spriteRendererLeftUp.enabled = spriteRenderer == spriteRendererLeftUp;
        spriteRendererLeftDown.enabled = spriteRenderer == spriteRendererLeftDown;

        activeSpriteRenderer = spriteRenderer;
        activeSpriteRenderer.idle = direction == Vector2.zero;

        if (view != null && view.IsMine)
        {
            view.RPC("SetDirectionRPC", RpcTarget.Others, newDirection);
        }

    }

    [PunRPC]
    public void SetDirectionRPC(Vector2 newDirection)
    {
        SetDirection(newDirection);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Explosion") && view.IsMine)
        {
            view.RPC("DeadRPC", RpcTarget.All);
        }

    }




    [PunRPC]
    public void Ghost()
    {
        // Sadece yerel oyuncunun karakteri etkilenir
        if (view.IsMine)
        {
            isGhost = true;
            StartCoroutine(DisableGhostAfterDelay());
        }
    }

    //isGhostu false �evir
    private IEnumerator DisableGhostAfterDelay()
    {
        yield return new WaitForSeconds(ghostDuration);
        UIManager.Instance.HideItemIndicattor(ItemPickup.ItemType.Ghost);

        Vector2 RoundedPosition;
        var bombControler = GetComponent<BombController>();
        Collider2D hit;


        do
        {
            RoundedPosition = transform.position;
            RoundedPosition.x = Mathf.Round(RoundedPosition.x);
            RoundedPosition.y = Mathf.Round(RoundedPosition.y);
            hit = Physics2D.OverlapCircle(RoundedPosition, bombControler.placeBombRadius, bombControler.bombCantPlaceLayers);
            Debug.Log("Duvar var!");
            yield return new WaitForEndOfFrame();
        } while (hit != null);
        // open close prensibi
        // geni�lemeye a��k m�dahaleye kapal�
        // simple responsibilitiy -> tek sorumluluk
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

        float oldSpeed = speed;
        speed = maxSpeed;
        yield return new WaitForSeconds(duration);
        UIManager.Instance.HideItemIndicattor(ItemPickup.ItemType.SpeedIncrease);
        speed = 2.5f;



    }

    //�a��ralan item
    public void SpeedItem()
    {
        UIManager.Instance.ShowItemIndicattor(ItemPickup.ItemType.SpeedIncrease);
        StartCoroutine(SetSpeed(4f, 8f));
    }

    public void Keyitem()
    {
        isKey = true;
    }

}
