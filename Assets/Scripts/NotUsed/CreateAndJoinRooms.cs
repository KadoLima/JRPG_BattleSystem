using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField createRoomInput;
    [SerializeField] TMP_InputField joinRoomInput;

    //public void CreateRoom()
    //{
    //    if (!string.IsNullOrEmpty(createRoomInput.text))
    //        PhotonNetwork.CreateRoom(createRoomInput.text);
    //}

    //public void JoinRoom()
    //{
    //    if (!string.IsNullOrEmpty(joinRoomInput.text))
    //        PhotonNetwork.JoinRoom(joinRoomInput.text);
    //}

    //public override void OnJoinedRoom()
    //{
    //    PhotonNetwork.LoadLevel("Main_Online");
    //}

}
