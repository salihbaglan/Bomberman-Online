using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ItemPickup;

public class UIManager : Singleton<UIManager>
{
    public GameObject ExplotionButton;
    public Image GhostGo; // Hayalet nesnesi
    public Image SpeedGo; // Hız nesnesi
    public Image MultiDropGo;
    public Image PushGo;
    public Image BombButtonGo;
    public Image LuckyGo;



    public void SetExplotionButtonState(bool isActive)
    {
        ExplotionButton.SetActive(isActive);
    }


    public void ShowItemIndicattor(ItemType type)
    {

        switch (type)
        {
            case ItemType.ExtraBomb:
                break;

            case ItemType.BlastRadius:
                break;

            case ItemType.SpeedIncrease:
                SpeedGo.enabled = true;

                break;

            case ItemType.PushItem:
                PushGo.enabled = true;

                break;

            case ItemType.MultiBomb:
                MultiDropGo.enabled = true;

                break;

            case ItemType.Ghost:
                GhostGo.enabled = true;
                break;

            case ItemType.KeyItem:
                break;

            case ItemType.MaxRadius:
                break;

            case ItemType.isActiveBombControl:
                BombButtonGo.enabled = true;
                SetExplotionButtonState(true);

                break;

        }
    }
    public void HideItemIndicattor(ItemType type)
    {

        switch (type)
        {
            case ItemType.ExtraBomb:
                break;

            case ItemType.BlastRadius:
                break;

            case ItemType.SpeedIncrease:
                SpeedGo.enabled = false;

                break;

            case ItemType.PushItem:
                PushGo.enabled = false;
                break;

            case ItemType.MultiBomb:
                MultiDropGo.enabled = false;

                break;

            case ItemType.Ghost:
                GhostGo.enabled = false;
                break;

            case ItemType.KeyItem:
                break;

            case ItemType.MaxRadius:
                break;

            case ItemType.isActiveBombControl:
                BombButtonGo.enabled = false;
                SetExplotionButtonState(false);

                break;


        }
    }

    public void ResetUI()
    {
        SpeedGo.enabled = false;
        PushGo.enabled = false;
        MultiDropGo.enabled = false;
        GhostGo.enabled = false;
        BombButtonGo.enabled = false;

    }

}
