using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Photon.Realtime;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    public static Action<int> OnConnectedToLobby;


    public static MultiplayerManager instance;

    //bool roomCreated = false;
    bool roomJoined = false;
    public bool RoomJoined => roomJoined;

    private void Awake()
    {
        instance = this;
    }

    //public override void OnEnable()
    //{
    //    base.OnEnable();
    //    //StartMenuUIController.OnPlayerChoseCoop += InitializeConnectionToServer;
    //}

    //public override void OnDisable()
    //{
    //    base.OnDisable();
    //    //StartMenuUIController.OnPlayerChoseCoop -= InitializeConnectionToServer;
    //}

    public void InitializeConnectionToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public static void ForceDisconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnConnectedToMaster()
    {
        //Debug.LogWarning("CONNECTED TO MASTER.");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        //Debug.LogWarning("CONNECTED TO LOBBY");
        OnConnectedToLobby?.Invoke(1);
    }

    public void CreateRoom(string roomName)
    {
        if (!string.IsNullOrEmpty(roomName))
        {
            PhotonNetwork.CreateRoom(roomName,new Photon.Realtime.RoomOptions { MaxPlayers = 2 });
            //roomCreated = true;
        }
    }

    public void JoinRoom(string roomName)
    {
        if (string.IsNullOrEmpty(roomName))
            return;

        PhotonNetwork.JoinRoom(roomName);

        //if (PhotonNetwork.JoinRoom(roomName))
        //{
        //    if (PhotonNetwork.IsMasterClient)
        //        Debug.LogWarning("ANOTHER PLAYER IS JOINING YOU!");

        //    else
        //        Debug.LogWarning("Joining room!!!!");

        //}

    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning("FAILED TO JOIN ROOM");
        roomJoined = false;
    }


    public override void OnJoinedRoom()
    {
        Debug.LogWarning("ROOM JOINED");
        roomJoined = true;
        //Debug.LogWarning("ROOM CREATED SUCCESSFULLY!");
        //PhotonNetwork.LoadLevel("Main_Online");
    }

    public int ConnectedPlayersCount()
    {

        if (!roomJoined)
        {
            Debug.LogWarning("nope, room not created yet");
            return 0;
        }

        return PhotonNetwork.CurrentRoom.PlayerCount;
    }


}
