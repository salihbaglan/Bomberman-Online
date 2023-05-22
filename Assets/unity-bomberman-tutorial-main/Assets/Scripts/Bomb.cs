using UnityEngine;

public class Bomb : MonoBehaviour
{
    private Vector2 targetPos = Vector2.zero;
    [SerializeField] private float movementSpeed = 1;

    private void Start()
    {
        // Başlangıçta hedef pozisyonu mevcut pozisyona eşitle
        targetPos = transform.position;
    }

    // Her karede bir kez çağrılan Update fonksiyonu
    public void Update()
    {
        // Eğer mevcut pozisyon ile hedef pozisyon arasındaki mesafe 0'dan büyük ise
        if (Vector2.Distance(transform.position, targetPos) > 0)
        {
            // Transform pozisyonunu, hedef pozisyona doğru belirli bir hızla hareket ettir
            transform.position = Vector2.MoveTowards(transform.position, targetPos, Time.deltaTime * movementSpeed);
        }
    }
    

    // Aşırı yükleme (overloading)
    public void Push(Vector2 targetPos)
    {
        // Hedef pozisyonunu verilen hedef pozisyonuyla değiştir
        this.targetPos = targetPos;
    }
}
