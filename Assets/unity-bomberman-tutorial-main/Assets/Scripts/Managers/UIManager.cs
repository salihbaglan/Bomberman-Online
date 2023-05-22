using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public GameObject ExplotionButton;

    public void SetExplotionButtonState(bool isActive)
    {
        ExplotionButton.SetActive(isActive);
    }
    
}
