using UnityEngine;

public class Destructible : MonoBehaviour
{
    public float destructionTime = 1f; // Nesnenin yok edilmesi için gerekli süre

    [Range(0f, 1f)]
    public float itemSpawnChance = 0.2f; // Yaratýklarýn yeni bir nesne yaratma olasýlýðý

    public GameObject[] spawnableItems; // Yaratýlabilecek nesnelerin listesi

    private void Start()
    {
        // Nesnenin belirtilen süre sonra yok edilmesi
        Destroy(gameObject, destructionTime);
    }

    private void OnDestroy()
    {
        // Yaratýklarýn yeni bir nesne yaratma olasýlýðýný kontrol etme ve nesneyi yaratma
        if (spawnableItems.Length > 0 && Random.value < itemSpawnChance)
        {
            // Yaratýlabilecek nesnelerden rastgele birini seçme
            int randomIndex = Random.Range(0, spawnableItems.Length);

            // Seçilen nesneyi mevcut nesnenin konumunda ve varsayýlan dönüþümde yaratma
            Instantiate(spawnableItems[randomIndex], transform.position, Quaternion.identity);
        }
    }
}
