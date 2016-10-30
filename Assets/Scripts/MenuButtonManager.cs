using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtonManager : MonoBehaviour {

    void Start() {
        SoundController.Instance.PlayBackgroundClip("main screen theme FINAL");
    }

	public void NewGame (string sceneName)
	{
	    SceneManager.LoadScene (sceneName);
        SoundController.Instance.PlayBackgroundClip("planet_background");
	}

    public void NewGameAsClient(string sceneName)
    {
        GameObject addressKeeperGameObject = GameObject.Find("HostAddressKeeper");
        HostAddressKeeper addressKeeper = addressKeeperGameObject.GetComponent<HostAddressKeeper>();
        GameObject ipFieldGameObject = GameObject.Find("IpField");
        var inputField = ipFieldGameObject.GetComponent<InputField>();
        var inputFieldText = FilterIpAddress(inputField.text);

        if (inputFieldText != null)
        {
            addressKeeper.HostAddress = inputFieldText;
            SceneManager.LoadScene (sceneName);
            SoundController.Instance.PlayBackgroundClip("planet_background");
        }
    }

    private string FilterIpAddress(string inputFieldText)
    {
        Regex ip = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
        MatchCollection result = ip.Matches(inputFieldText);
        return result.Count > 0 ? result[0].Value : null;
    }

    public void ExitGame () {
	    Application.Quit();
	}
}
