using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviourPunCallbacks
{
    public enum ItemType
    {
        ExtraBomb, // Ekstra bomba almak için kullanýlan öðe
        BlastRadius, // Patlama yarýçapýný artýrmak için kullanýlan öðe
        SpeedIncrease, // Hýzý artýrmak için kullanýlan öðe
        PushItem, // Ýtemleri itmek için kullanýlan öðe
        MultiBomb, // Birden fazla bomba koyabilmek için kullanýlan öðe
        Ghost, // Hayalet moduna geçmek için kullanýlan öðe
        MaxRadius, // Maksimum patlama yarýçapýný ayarlamak için kullanýlan öðe
        isActiveBombControl, // Patlama düðmesini aktif hale getiren öðe
        luckItem,
        Count,
        None,
    }
    private List<ItemType> availableItemTypes = new List<ItemType> { ItemType.SpeedIncrease, ItemType.MultiBomb, ItemType.Ghost };
    private ItemType lastSelectedItemType = ItemType.None;

    public ItemType type; // Öðenin tipi

    private void OnItemPickup(GameObject player)
    {
        if (player == null)
        {
            return;
        }
        UIManager.Instance.ShowItemIndicattor(type);

        switch (type)
        {
            case ItemType.ExtraBomb:
                player.GetComponent<BombController>().AddBomb(); // Ekstra bomba ekleyen fonksiyonu çaðýrýr
                break;

            case ItemType.BlastRadius:
                player.GetComponent<BombController>().ExplosionRadius(); // Patlama yarýçapýný artýran fonksiyonu çaðýrýr
                break;

            case ItemType.SpeedIncrease:
                player.GetComponent<MovementController>().SpeedItem(); // Hýzý artýran fonksiyonu çaðýrýr
                break;

            case ItemType.PushItem:
                player.GetComponent<BombController>().Push(); // Ýtemleri iten fonksiyonu çaðýrýr
                break;

            case ItemType.MultiBomb:
                player.GetComponent<BombController>().OnItemEaten(); // Birden fazla bomba koyan fonksiyonu çaðýrýr
                break;

            case ItemType.Ghost:
                player.GetComponent<MovementController>().Ghost(); // Hayalet moduna geçen fonksiyonu çaðýrýr
                break;

            case ItemType.MaxRadius:
                player.GetComponent<BombController>().MaxExplosionRadius(); // Maksimum patlama yarýçapýný ayarlayan fonksiyonu çaðýrýr
                break;

            case ItemType.isActiveBombControl:
                player.GetComponent<BombController>().ExplosionButtonItem(); // Patlama düðmesini aktif hale getiren fonksiyonu çaðýrýr
                //UIManager.Instance.SetExplotionButtonState(true);
                break;

            case ItemType.luckItem:
                GameObject playerObject = player;// Burada oyuncunun referansýný almanýz gerekiyor
                SelectRandomItem(playerObject);
                break;
        }
        gameObject.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<PhotonView>().IsMine)
                OnItemPickup(other.gameObject); // Oyuncu temas ettiðinde öðeyi toplar
            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.Destroy(gameObject);
        }

    }

    // Önceki kodun devamý...
    IEnumerator HideItemIndicatorAfterDelay(ItemPickup.ItemType itemType, float delay)
    {
        yield return new WaitForSeconds(delay);

        UIManager.Instance.HideItemIndicattor(itemType);
    }


    public void SelectRandomItem(GameObject player)
    {
        if (availableItemTypes.Count == 0)
        {
            Debug.Log("Tüm öðeler tüketildi. Liste yeniden baþlatýlýyor.");
            ResetAvailableItems();
        }

        ItemType randomItemType;
        do
        {
            randomItemType = availableItemTypes[Random.Range(0, availableItemTypes.Count)];

        } while (randomItemType == lastSelectedItemType);

        // Seçilen öðe türünü listeden çýkar
        availableItemTypes.Remove(randomItemType);
        lastSelectedItemType = randomItemType;

        // Seçilen öðe türüne göre ilgili fonksiyonu çaðýr
        switch (randomItemType)
        {
            case ItemType.SpeedIncrease:
                player.GetComponent<MovementController>().SpeedItem();
                UIManager.Instance.ShowItemIndicattor(ItemPickup.ItemType.SpeedIncrease);
                StartCoroutine(HideItemIndicatorAfterDelay(ItemPickup.ItemType.SpeedIncrease, 5f));
                break;

            case ItemType.MultiBomb:
                player.GetComponent<BombController>().OnItemEaten();
                UIManager.Instance.ShowItemIndicattor(ItemPickup.ItemType.MultiBomb);
                StartCoroutine(HideItemIndicatorAfterDelay(ItemPickup.ItemType.SpeedIncrease, 5f));
                break;

            case ItemType.Ghost:
                player.GetComponent<MovementController>().Ghost();
                UIManager.Instance.ShowItemIndicattor(ItemPickup.ItemType.Ghost);
                StartCoroutine(HideItemIndicatorAfterDelay(ItemPickup.ItemType.Ghost, 5f));
                break;
        }
    }

    private void ResetAvailableItems()
    {
        availableItemTypes.Clear();
        availableItemTypes.AddRange(new ItemType[] { ItemType.SpeedIncrease, ItemType.MultiBomb, ItemType.Ghost });
        lastSelectedItemType = ItemType.None;
    }



}
