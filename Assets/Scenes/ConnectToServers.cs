using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConnectToServers : MonoBehaviourPunCallbacks
{

    void Start()
    {
        ConnectToTheServer();
    }
    public void ConnectToTheServer()
    {

        PhotonNetwork.NickName = "User" + Random.Range(1, 100);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();


    }

    public override void OnConnectedToMaster()
    {

        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        SceneManager.LoadScene("Bomberman");
    }
}
