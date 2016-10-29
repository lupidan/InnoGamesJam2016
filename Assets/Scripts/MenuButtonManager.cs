using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuButtonManager : MonoBehaviour {

	public void NewGame (string sceneName) {
		SceneManager.LoadScene (sceneName);
	}

	public void ExitGame () {
	
	}
}
