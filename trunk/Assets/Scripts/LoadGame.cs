using UnityEngine;
using System.Collections;

public class LoadGame : MonoBehaviour {
	
	public UISprite sprite;
	public UIPopupList goal;
	public UIPopupList round;
	public GameObject progress;
	public GameObject menu;
	public bool isloading;
	public GameStatus status; 
	public void Start()
	{
		sprite.fillAmount=0;
		NGUITools.SetActive(progress.gameObject,false);
		if(!status)
			status = GameObject.Find("GameStatus").GetComponent<GameStatus>();
	}
	public void Update()
	{
		if(isloading)
		{
			sprite.fillAmount+=Random.Range(0.5f,0.9f)*Time.deltaTime;
			if(sprite.fillAmount>=1)
			{
//				StartCoroutine("loadLevel");
//				sprite.fillAmount=-1;
				Application.LoadLevel("Main");
				
//				status.ChangeState(WaitForStart.Instance);
			}
		}
	}
	
	IEnumerator loadLevel()
	{
		AsyncOperation async = Application.LoadLevelAsync("Main");
        yield return async;
		 Debug.Log("Loading complete");
		status.ChangeState(WaitForStart.Instance);
       
	}
	public void OnSelectionChange()
	{
		switch(goal.selection)
		{
			case "Angle":
			GameCtrl.goal =GameCtrl.GameGoal.angle;
			break;
			case "Pos":
			GameCtrl.goal=GameCtrl.GameGoal.pos;
			break;
			case "Time":
			GameCtrl.goal=GameCtrl.GameGoal.time;
			break;
			case "Random":
			GameCtrl.goal=GameCtrl.GameGoal.random;
			break;
			default :
			Debug.Log(round.selection);
			break;
		}
	}
	
	public void StartLoad()
	{
		isloading = true;
		GameCtrl.round =int.Parse(round.selection);
		
		NGUITools.SetActive(menu,false);
		NGUITools.SetActive(progress.gameObject,true);
	}
	
}
