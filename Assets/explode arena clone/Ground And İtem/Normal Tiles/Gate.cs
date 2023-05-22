using UnityEngine;
using UnityEngine.SceneManagement;

public class Gate : MonoBehaviour
{

    public new SpriteRenderer renderer;
    public Collider2D boxCol; // Kapının kutu çarpışma özelliği.
    public Collider2D circleCol; // Kapının daire çarpışma özelliği.

    public Sprite gateClosed; // Kapının kapalı sprite'ı.
    public Sprite gateOpen; // Kapının açık sprite'ı.

    void Update()
    {
        // Eğer düşman sayısı sıfırsa kapıyı aç.
        if (GetComponent<MovementController>().isKey)
        {
            renderer.sprite = gateOpen;
            // "isKey" özelliği true ise buradaki kodlar çalışır.
            Debug.Log("isKey");

        }

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Oyuncu kapıya çarptığında.
        if (col.gameObject.tag.Equals("Player"))
        {
            Debug.Log("kapıya çarptı");
        }
    }



}
