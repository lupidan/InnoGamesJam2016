using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AnnouncementDelegate : MonoBehaviour
{

    private ClientGameLogicManager clientGameLogicManager;
    private Text textField;

    // Use this for initialization
	void Start ()
	{
	    textField = gameObject.GetComponent<Text>();
	    clientGameLogicManager = ClientGameLogicManager.GetClientLogicFromScene();
	    clientGameLogicManager.StateUpdatedHandler += OnStateChanged;
	}

    public void OnStateChanged()
    {
        switch (clientGameLogicManager.CurrentServerSideState.CurrentPhase)
        {
            case GamePhase.WaitingForStart:
                textField.text = "Waiting for player...";
                break;
            case GamePhase.Planning:
                textField.text = "Choose you movement";
                break;
            case GamePhase.Revision:
                textField.text = "Cheat time!";
                break;
            case GamePhase.Finished:
                textField.text = "Player " + clientGameLogicManager.CurrentServerSideState.WinningPlayerId + " won!";
                textField.fontSize = 24;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

    }

}
