using UnityEngine;
using UnityEngine.EventSystems;

public class MapController : MonoBehaviour {

	public float movingSpeed = 2.0f;

	void Update()
	{
		Vector3 movingVector = Vector3.zero;
		movingVector.x = Input.GetAxis("Horizontal") * movingSpeed * Time.deltaTime;
		movingVector.y = Input.GetAxis("Vertical") * movingSpeed * Time.deltaTime;

		Vector3 newCameraPosition = Camera.main.transform.position;
		newCameraPosition += movingVector;
		Camera.main.transform.position = newCameraPosition;
	}

}
