using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AnnouncementDelegate : MonoBehaviour
{

    private ClientGameLogicManager clientGameLogicManager;
    private Text textField;
    private bool hasGameFinished;

    // Use this for initialization
	void Start ()
	{
	    textField = gameObject.GetComponent<Text>();
	    clientGameLogicManager = ClientGameLogicManager.GetClientLogicFromScene();
	    clientGameLogicManager.StateUpdatedHandler += OnStateChanged;
	}

    public void OnStateChanged()
    {
        if (hasGameFinished)
        {
            return;
        }

        switch (clientGameLogicManager.CurrentServerSideState.CurrentPhase)
        {
            case GamePhase.WaitingForStart:
                textField.text = "Waiting for player...";
                break;
            case GamePhase.Planning:
                textField.text = "Preview your movement";
                break;
            case GamePhase.Revision:
                textField.text = "Choose your cheating monkey";
                break;
            case GamePhase.Finished:
                hasGameFinished = true;
                bool hasBlueWon = clientGameLogicManager.CurrentServerSideState.WinningPlayerId == 0;
                textField.color = hasBlueWon ? new Color(0.6f, 0.6f, 1f, 1f) : new Color(1f, 0.4f, 0.4f, 1f);
                textField.text = "Player " + (hasBlueWon? "blue" : "red") + " won!";
                textField.fontSize = 24;

                IngameSubmitButtonManager.GetIngameSubmitButtonManager().SetGameFinished();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

    }

}
