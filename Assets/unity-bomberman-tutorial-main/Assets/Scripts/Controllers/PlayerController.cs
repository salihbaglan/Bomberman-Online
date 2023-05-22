using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks
{
    private BombController bombController;

    private void Start()
    {
        if (photonView.IsMine)
        {
            GameManager.Instance.localPlayer = this;
        }
        bombController = gameObject.GetComponent<BombController>();
    }


    public void AddRemainingBomb()
    {
        bombController.AddRemainingBomb(); 
    }
}
