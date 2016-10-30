using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IngameSubmitButtonManager : MonoBehaviour {

    private ClientNetworkingManager clientNetworkingManager;
    private Button advanceButton;
    private Text advanceButtonText;
    private bool hasFinished;

    public static IngameSubmitButtonManager GetIngameSubmitButtonManager()
    {
        return GameObject.Find("AdvanceButton").GetComponent<IngameSubmitButtonManager>();
    }

    public bool IsWaiting()
    {
        return !advanceButton.interactable || hasFinished;
    }

    public void SetGameFinished()
    {
        hasFinished = true;
        advanceButton.interactable = true;
        advanceButtonText.text = "Finish";
    }

    void Start()
    {
        advanceButton = GameObject.Find("AdvanceButton").GetComponent<Button>();
        advanceButtonText = advanceButton.GetComponentInChildren<Text>();
        ClientGameLogicManager.GetClientLogicFromScene().PlayerMayInteractHandler += UnlockButton;
    }

    private void UnlockButton()
    {
        advanceButton.interactable = true;
        advanceButtonText.text = "Next";
    }

    public void SubmitTurn()
    {
        if (hasFinished)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            advanceButton.interactable = false;
            advanceButtonText.text = "Wait";

            var clientGameLogicManager = ClientGameLogicManager.GetClientLogicFromScene();

            var actions = clientGameLogicManager.QueuedGameActionsPlanning;
            if (clientGameLogicManager.CurrentServerSideState.CurrentPhase == GamePhase.Revision) {
                actions = clientGameLogicManager.QueuedGameActionsRevision;
            }

            ClientNetworkingManager.GetClientNetworkingManager().SendActions(actions);

            clientGameLogicManager.QueuedGameActionsRevision =
                new List<GameAction>(clientGameLogicManager.QueuedGameActionsPlanning);

            UnitController[] unitControllers = FindObjectsOfType<UnitController>();
            for (int i = 0; i < unitControllers.Length; i++)
            {
                unitControllers[i].SetDestinationTileController(null);
            }
        }
    }

}
