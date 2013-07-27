using UnityEngine;
using System.Collections;

public class CameraPlanerMovement_VLS : MonoBehaviour
{
    public float speed = 25;

    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width/2 - 5, Screen.height/2 - 5, 10, 10), "");
    }

	// Update is called once per frame
	void Update () 
    {
        transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * speed, Input.GetAxis("Vertical") * Time.deltaTime * speed, 0);
	}
}
