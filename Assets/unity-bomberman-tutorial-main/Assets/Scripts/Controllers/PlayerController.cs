using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks
{
    private BombController bombController;
    private MovementController movementController;


    public bool isDead = false;
    private void Start()
    {
        if (photonView.IsMine)
        {
            GameManager.Instance.localPlayer = this;
        }
        bombController = gameObject.GetComponent<BombController>();
        movementController = gameObject.GetComponent<MovementController>();
    }


    public void AddRemainingBomb()
    {
        bombController.AddRemainingBomb();
    }

    [PunRPC]
    public void DeadRPC()
    {
        isDead = true;
        SpawnDeathSequence();
    }
    [PunRPC]
    public void SpawnRPC(Vector3 vector3)
    {
        transform.position = vector3;
        isDead = false;
        movementController.Reset();
        bombController.Reset();
        SpawnDeathSequence();
    }
    private void SpawnDeathSequence()
    {
        enabled = !isDead;
        GetComponent<BombController>().enabled = !isDead;

        movementController.spriteRendererUp.enabled = !isDead;
        movementController.spriteRendererDown.enabled = !isDead;
        movementController.spriteRendererLeft.enabled = !isDead;
        movementController.spriteRendererRight.enabled = !isDead;
        movementController.spriteRendererDeath.enabled = isDead;

        Invoke(nameof(OnDeathSequenceEnded), 1.25f);
    }


    private void OnDeathSequenceEnded()
    {
        if (isDead && photonView.IsMine) { GameManager.Instance.ReSpawn(); }
        gameObject.SetActive(!isDead);

    }
}
