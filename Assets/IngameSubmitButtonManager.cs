using UnityEngine;
using UnityEngine.UI;

public class IngameSubmitButtonManager : MonoBehaviour {

    private ClientNetworkingManager clientNetworkingManager;
    private Button advanceButton;
    private Text advanceButtonText;

    public static IngameSubmitButtonManager GetIngameSubmitButtonManager()
    {
        return GameObject.Find("AdvanceButton").GetComponent<IngameSubmitButtonManager>();
    }

    public bool IsWaiting()
    {
        return !advanceButton.interactable;
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
        advanceButton.interactable = false;
        advanceButtonText.text = "Wait";


        var actions = ClientGameLogicManager.GetClientLogicFromScene().QueuedGameActions;
        ClientNetworkingManager.GetClientNetworkingManager().SendActions(actions);

        UnitController[] unitControllers = FindObjectsOfType<UnitController>();
        for (int i = 0; i < unitControllers.Length; i++)
        {
            unitControllers[i].SetDestinationTileController(null);
        }
    }

}
