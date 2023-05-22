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

    private void Start()
    {
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Vector3 randomPosition = spawnPoints[randomIndex].position;
        PhotonNetwork.Instantiate(characterPrefab.name, randomPosition, Quaternion.identity);
    }
}
