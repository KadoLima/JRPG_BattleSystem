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
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        OnConnectedToLobby?.Invoke(1);
    }

    public void CreateRoom(string roomName)
    {
        if (!string.IsNullOrEmpty(roomName))
        {
            PhotonNetwork.CreateRoom(roomName,new Photon.Realtime.RoomOptions { MaxPlayers = 2 });
        }
    }

    public void JoinRoom(string roomName)
    {
        if (string.IsNullOrEmpty(roomName))
            return;

        PhotonNetwork.JoinRoom(roomName);

    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        roomJoined = false;
    }


    public override void OnJoinedRoom()
    {
        roomJoined = true;
    }

    public int ConnectedPlayersCount()
    {
        if (!roomJoined)
        {
            return 0;
        }

        return PhotonNetwork.CurrentRoom.PlayerCount;
    }


}
