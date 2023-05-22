using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class İtemSettings : MonoBehaviour
{
    public Image barImage; // Seviye çubuğunun Image bileşeni
    public Sprite levelSprite; // Seviye çubuğu sprite'ı

    public int levelCount = 1; // Seviye sayacı, başlangıçta 1 olarak ayarlanır

    public void IncreaseLevel()
    {
        levelCount++; // Seviye sayacı arttırılır
        UpdateLevel();
    }

    public void UpdateLevel()
    {
        // Seviye sayısı, 1'den küçük veya seviye sprite'larının uzunluğundan büyükse hata mesajı verilir
        if (levelCount < 1)
        {
            Debug.LogError("Seviye sayısı 1'den küçük olamaz!");
            levelCount = 1;
        }
        else if (levelCount > 1)
        {
            // Doğru sprite'ı seçmek için seviye sayacından bir eksik çıkarılıyor
            barImage.sprite = levelSprite;
        }
    }

}
