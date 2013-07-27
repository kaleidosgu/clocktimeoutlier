using UnityEngine;
using System.Collections;

public class Control : MonoBehaviour {
	
	public Needle needle;
	
	public GameStatus status; 
	
	//行动限制
	public float actionBar;
	
	public float actionNum;
	//行动恢复数量
	public float regainNum;
	
	public bool canMove;
	
	void Start()
	{
		needle=gameObject.GetComponent<Needle>();
		if(!status)
			status = GameObject.Find("GameStatus").GetComponent<GameStatus>();
		actionNum = actionBar;
		canMove = true;
	}
	
	void FixedUpdate ()
	{	
		if(status.status == GameStatus.Status.roundstart)
		{
			if(actionNum>0 && canMove )
			{
				switch(needle.type)
				{
					#region ---------------------
					case Needle.Type.hour:
						if(Input.GetKey(KeyCode.A))
						{
							this.gameObject.transform.Rotate(0,0,needle.rotateSpeed,Space.Self);
							actionNum -=needle.rotateSpeed;
							needle.action = Needle.Action.move;
							Debug.Log("action -- : "+needle.rotateSpeed+" --------------");
						}
						if(Input.GetKey(KeyCode.D))
						{
							this.gameObject.transform.Rotate(0,0,-needle.rotateSpeed,Space.Self);
							actionNum -=needle.rotateSpeed;
							needle.action = Needle.Action.move;
							Debug.Log("action -- : "+needle.rotateSpeed+" --------------");
						}
						break;
					case Needle.Type.min:
						if(Input.GetKey(KeyCode.LeftArrow))
						{
							this.gameObject.transform.Rotate(0,0,needle.rotateSpeed,Space.Self);
							actionNum -=needle.rotateSpeed;
							needle.action = Needle.Action.move;
							Debug.Log("action -- : "+needle.rotateSpeed+" --------------");
						}
						
						if(Input.GetKey(KeyCode.RightArrow))
						{
							this.gameObject.transform.Rotate(0,0,-needle.rotateSpeed,Space.Self);
							actionNum -=needle.rotateSpeed;
							needle.action = Needle.Action.move;
							Debug.Log("action -- : "+needle.rotateSpeed+" --------------");
						}
						
						break;
					case Needle.Type.sec:
						if(Input.GetKey(KeyCode.N))
						{
							this.gameObject.transform.Rotate(0,0,needle.rotateSpeed,Space.Self);
							actionNum -=needle.rotateSpeed;
							needle.action = Needle.Action.move;
							Debug.Log("action -- : "+needle.rotateSpeed+" --------------"+needle.action);
						}
						
						if(Input.GetKey(KeyCode.M))
						{
							this.gameObject.transform.Rotate(0,0,-needle.rotateSpeed,Space.Self);
							actionNum -=needle.rotateSpeed;
							needle.action = Needle.Action.move;
							Debug.Log("action -- : "+needle.rotateSpeed+" --------------"+needle.action);
						}
						break;
					#endregion
				}
			}
			if(Input.GetKeyUp(KeyCode.A)||Input.GetKeyUp(KeyCode.D)||Input.GetKeyUp(KeyCode.N)||Input.GetKeyUp(KeyCode.M)||Input.GetKeyUp(KeyCode.LeftArrow)||Input.GetKeyUp(KeyCode.RightArrow))
			{
				needle.action = Needle.Action.stop;
			}
			if(needle.action==Needle.Action.stop)
			{
				
				if(actionNum>0 && actionNum<actionBar)
				{
					actionNum+=regainNum*Time.deltaTime;
					if(actionNum>=actionBar)
						canMove=true;
				}
				else if(actionNum<0)
				{
					canMove = false;
					actionNum+=regainNum*Time.deltaTime;
				}
			}
		}
	}
	
}
