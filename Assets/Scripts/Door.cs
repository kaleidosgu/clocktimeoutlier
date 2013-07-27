using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
	
	private static Door _instacne; 
	public Transform leftDoor;
	public Transform rightDoor;
	public UILabel information;
	
	// Use this for initialization
	public static  Door Instance{
		get{
			if(!_instacne)
			{
				_instacne = new Door();
			}
			return _instacne;
		}
	}
	void Start () {
	_instacne = this;
	information.text = "";
		CloseText();
	}
	
	public void OpenDoor(){
		information.text = "";
		leftDoor.animation.Play ("move@left");
		rightDoor.animation.Play("move@right");
		
	}
	
	public void CloseDoor()
	{
		CloseText();
		leftDoor.animation.Play ("back@left");
		rightDoor.animation.Play("back@right");
	}
	
	public void CloseText()
	{
		switch(GameCtrl.goal)
		{
			case GameCtrl.GameGoal.angle:
				information.text = "Make the angle ("+GameCtrl.goalAngle+") with others!";
				break;
			case GameCtrl.GameGoal.pos:
				information.text = "Go to the position (" + GameCtrl.goalAngle2+")";
				break;
			case GameCtrl.GameGoal.time:
				information.text = "Go to the position of time";
				break;
		}
	}
	
}
