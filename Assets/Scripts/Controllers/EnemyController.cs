using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 5f; // Hız
    public float animationSpeedMultiplier = 0.1f; // Animasyon hızı çarpanı

    public Sprite[] rightSprites; // Sağa hareket ederken kullanılacak animasyon
    public Sprite[] leftSprites; // Sola hareket ederken kullanılacak animasyon

   

    private SpriteRenderer spriteRenderer;
    public AnimatedSpriteRenderer spriteRendererDeath;

    private Rigidbody2D rb2D;

    private float direction = 1f; // Hareket yönü (+1 sağa, -1 sola)
    private float directionChangeInterval = 4f; // Yön değiştirme aralığı
    private float timeSinceLastDirectionChange = 0f; // Son yön değiştirme zamanı

    private int currentFrame = 0; // Mevcut animasyon çerçevesi
    private Sprite[] currentAnimation; // Mevcut animasyon dizisi

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2D = GetComponent<Rigidbody2D>();

        // Başlangıçta sağa doğru hareket eden animasyonu ayarla
        currentAnimation = rightSprites;
    }

    private void FixedUpdate()
    {
        // Yön değiştirme zamanı geldi mi kontrol et
        timeSinceLastDirectionChange += Time.deltaTime;
        if (timeSinceLastDirectionChange >= directionChangeInterval)
        {
            // Yön değiştir
            direction *= -1f;
            timeSinceLastDirectionChange = 0f;

            // Yeni yön için animasyonu ayarla
            if (direction > 0f)
            {
                currentAnimation = rightSprites;
            }
            else
            {
                currentAnimation = leftSprites;
            }

            // Animasyon çerçevesini sıfırla
            currentFrame = 0;
        }

        // Hareket et
        Vector2 movement = new Vector2(speed * direction, 0f);
        rb2D.velocity = movement;

        // Animasyonu oynat
        float animationSpeed = currentAnimation.Length * animationSpeedMultiplier;
        if (animationSpeed <= 0f)
        {
            animationSpeed = 1f;
        }
        int frameChangeInterval = Mathf.RoundToInt(animationSpeed / Time.fixedDeltaTime);
        if (frameChangeInterval <= 0)
        {
            frameChangeInterval = 1;
        }
        if (Time.frameCount % frameChangeInterval == 0)
        {
            currentFrame = (currentFrame + 1) % currentAnimation.Length;
        }
        spriteRenderer.sprite = currentAnimation[currentFrame];
    }
    private void DeathSequence2()
    {
        enabled = false;




        spriteRendererDeath.enabled = true;

        Invoke(nameof(OnDeathSequenceEnded), 1.25f);
    }
    private void OnDeathSequenceEnded()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Explosion"))
        {
         
            DeathSequence2();
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = false;
       
        }


    }
}
