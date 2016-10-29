using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtonManager : MonoBehaviour {

	[SerializeField] Text hostNameText;

	public void NewGame (string sceneName) {
		SceneManager.LoadScene (sceneName);
	}

	public void JoinGame() {
		string hostName = hostNameText.text;
		Debug.Log ("Joining host: " + hostName);
		ClientNetworkingManager netMan = new ClientNetworkingManager ();
		netMan.ConnectToHost (hostName);

	}

	public void ExitGame () {
		Application.Quit ();
	}
}
