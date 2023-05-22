using UnityEngine;

public class ExplosionDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Explosion"))
        {

            Destroy(gameObject);
        }
    }
}
