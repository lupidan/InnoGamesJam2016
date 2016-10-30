using UnityEngine;
using UnityEngine.UI;

public class IngameSubmitButtonManager : MonoBehaviour {

    private ClientNetworkingManager clientNetworkingManager;
    private Button advanceButton;
    private Text advanceButtonText;

    void Start()
    {
        advanceButton = GameObject.Find("AdvanceButton").GetComponent<Button>();
        advanceButtonText = advanceButton.GetComponentInChildren<Text>();
        ClientGameLogicManager.GetClientLogicFromScene().PlayerMayInteractHandler += UnlockButton;
    }

    private void UnlockButton()
    {
        advanceButton.enabled = true;
        advanceButtonText.text = "Next";
    }

    public void SubmitTurn()
    {
        advanceButton.enabled = false;
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
