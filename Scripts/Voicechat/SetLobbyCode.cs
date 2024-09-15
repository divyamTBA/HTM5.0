using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Coherence.Toolkit;
using Coherence.Connection;
using System;
using Coherence.Samples.RoomsDialog;
public class SetLobbyCode : MonoBehaviour
{
    public GameObject LoginScreenUI;
    public CoherenceBridge coherenceBridge;
    public RoomsDialogUI roomsDialogUI;
    public void SetLobbyName(ClientID iD)
    {
        LoginScreenUI.SetActive(true);
        Debug.Log("Joined room " + roomsDialogUI.JoinedRoomName);
        VivoxVoiceManager.LobbyChannelName = roomsDialogUI.JoinedRoomName;
    }
    public void ResetLobbyName(ConnectionCloseReason reason)
    {
        LoginScreenUI.SetActive(false);
        VivoxVoiceManager.LobbyChannelName = roomsDialogUI.JoinedRoomName;
    }

    private void OnEnable()
    {
        coherenceBridge.Client.OnConnected += SetLobbyName;
        coherenceBridge.Client.OnDisconnected += ResetLobbyName;
    }


    private void OnDisable()
    {
        coherenceBridge.Client.OnConnected -= SetLobbyName;
        coherenceBridge.Client.OnDisconnected -= ResetLobbyName;
    }

}
