using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject[] players;
    public GameObject characterPrefab;
    public Transform[] spawnPoints;


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
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        PhotonNetwork.Instantiate(characterPrefab.name, GetRandomPos, Quaternion.identity);
    }

    public void ReSpawn()
    {
        localPlayer.photonView.RPC("SpawnRPC", RpcTarget.All, GetRandomPos);
    }
}
