using UnityEngine;

public class Explosion : MonoBehaviour
{
    public AnimatedSpriteRenderer start; // Patlama animasyonunun baþlangýç karesini tutan sprite renderer
    public AnimatedSpriteRenderer middle; // Patlama animasyonunun orta karesini tutan sprite renderer
    public AnimatedSpriteRenderer end; // Patlama animasyonunun son karesini tutan sprite renderer

    public void SetActiveRenderer(AnimatedSpriteRenderer renderer)
    {
        start.enabled = renderer == start; // Baþlangýç karesi sprite renderer'ý aktifse, diðerlerini devre dýþý býrakýr
        middle.enabled = renderer == middle; // Orta kare sprite renderer'ý aktifse, diðerlerini devre dýþý býrakýr
        end.enabled = renderer == end; // Son kare sprite renderer'ý aktifse, diðerlerini devre dýþý býrakýr
    }

    public void SetDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x); // Yön vektörünün açýsýný hesaplar
        transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward); // Patlama objesinin rotasyonunu belirtilen yöne ayarlar
    }

    public void DestroyAfter(float seconds)
    {
        Destroy(gameObject, seconds); // Belirtilen süre sonunda patlama objesini yok eder
    }
}
