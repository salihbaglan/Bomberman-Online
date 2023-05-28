using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using static ItemPickup;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject[] players;
    public GameObject characterPrefab;
    public Transform[] spawnPoints;
    public Transform[] itemPoses;

    public PlayerController localPlayer;

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }


    private Vector3 GetRandomPos => spawnPoints[Random.Range(0, spawnPoints.Length)].position;

    private void Start()
    {
        SpawnItems();
        CreatePlayer();

    }

    private void SpawnItems()
    {
        DropItem(ItemType.Ghost, itemPoses[0].position);
        DropItem(ItemType.isActiveBombControl, itemPoses[1].position);
        DropItem(ItemType.ExtraBomb, itemPoses[2].position);
        DropItem(ItemType.MultiBomb, itemPoses[3].position);
    }

    private void CreatePlayer()
    {
        PhotonNetwork.Instantiate(characterPrefab.name, GetRandomPos, Quaternion.identity);
    }

    public void ReSpawn()
    {
        localPlayer.photonView.RPC("SpawnRPC", RpcTarget.All, GetRandomPos);
    }

    public void DropItem(ItemType type, Vector2 postion)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        string itemName = "Items/" + GetItemName(type);
        PhotonNetwork.InstantiateRoomObject(itemName, postion, Quaternion.identity).GetComponent<ItemPickup>();
    }
    private string GetItemName(ItemType type)
    {
        string itemName = "";
        switch (type)
        {
            case ItemType.ExtraBomb:
                itemName = "ExtraBomb";
                break;

            case ItemType.BlastRadius:
                itemName = "BlastRadius";

                break;

            case ItemType.SpeedIncrease:
                itemName = "SpeedIncrease";

                break;

            case ItemType.PushItem:
                itemName = "PushItem";

                break;

            case ItemType.MultiBomb:
                itemName = "MultiBomb";

                break;

            case ItemType.Ghost:
                itemName = "Ghost";

                break;

            case ItemType.MaxRadius:
                itemName = "MaxRadius";

                break;

            case ItemType.isActiveBombControl:
                itemName = "isActiveBombControl";

                break;
            case ItemType.luckItem:
                itemName = "luckItem";
                break;
        }

        return itemName;
    }

}
