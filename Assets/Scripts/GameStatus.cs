using UnityEngine;
using System.Collections;

public class GameStatus : MonoBehaviour {
	public enum Status{menu,logo,loading,waitforstart,roundstart,roundover,result}
	
	public Status status;
	private FiniteStateMachine<GameStatus> FSM;
		
	public void Start()
	{
		DontDestroyOnLoad(this.gameObject);
	}
	public void ChangeState(FSMState<GameStatus> e) {
		FSM.ChangeState(e);		
	}
	
	public void Awake() {
		FSM = new FiniteStateMachine<GameStatus>();
		FSM.Configure(this,GameMenu.Instance);
	}
	
	public void Update() {
		FSM.Update();
	}
	public void ChanageStatus(Status s)
	{
		status=s;
	}
}

public sealed class GameMenu :  FSMState<GameStatus> {
	
	static readonly GameMenu instance = new GameMenu();
	public static GameMenu Instance {
		get {
			return instance;
		}
	}

	static GameMenu() { }
	private GameMenu() { }
	public override void Enter (GameStatus m) {
		if (m.status!=GameStatus.Status.menu) {
			Debug.Log("Entering the GameMenu...");
			m.ChanageStatus(GameStatus.Status.menu);
		}
	}
	public override void Execute (GameStatus m) {
	}
	public override void Exit(GameStatus m) {
	}
}

public sealed class Loading :  FSMState<GameStatus> {
	
	static readonly Loading instance = new Loading();
	public static Loading Instance {
		get {
			return instance;
		}
	}
	static Loading() { }
	private Loading() { }
	public override void Enter (GameStatus m) {
		if (m.status!=GameStatus.Status.loading) {
			m.ChanageStatus(GameStatus.Status.loading);
		}
	}
	public override void Execute (GameStatus m) {
		
	}
	public override void Exit(GameStatus m) {
	}
}

public sealed class GameOVer : FSMState<GameStatus>{
	static readonly GameOVer instance = new GameOVer();
	public static GameOVer Instance{
		get{
			return instance;
		}
	}
//	private float countDown = 5f;
	static GameOVer(){}
	private GameOVer(){}
	public override void Enter (GameStatus m)
	{
		Sound.Instance.playSound(1);
		
		if(m.status!=GameStatus.Status.roundover){
			m.ChanageStatus(GameStatus.Status.roundover);
		}
		//todo
		//设置分数
	}
	public override void Execute (GameStatus m)
	{
		if(m.status != GameStatus.Status.waitforstart )
		{
//			 if(countDown >= 0) {
//				float diffValue = Time.deltaTime;
//			    countDown -= diffValue;
//			}else
//			{
//				Application.LoadLevel(Application.loadedLevelName);
//				Time.timeScale = 1;
//				countDown = 5;
				GameCtrl.round --;
				m.ChangeState(WaitForStart.Instance);
//			}
		}
	}
	
	public override void Exit (GameStatus entity)
	{
	}
}

public sealed class GameStart :FSMState<GameStatus>
{
	static readonly GameStart instance = new GameStart();
	public static GameStart Instance{
		get{
			return instance;
		}
	}
	static GameStart(){}
	private GameStart(){}
	public override void Enter (GameStatus m)
	{
		Door.Instance.OpenDoor();
		Debug.Log("Game Started");
		if(m.status!=GameStatus.Status.roundstart){
			m.ChanageStatus(GameStatus.Status.roundstart);
		}
	}
	public override void Execute (GameStatus m)
	{
		
	}
	
	public override void Exit (GameStatus entity)
	{
	}
}

public sealed class WaitForStart :FSMState<GameStatus>
{
	static readonly WaitForStart instance = new WaitForStart();
	public static WaitForStart Instance{
		get{
			return instance;
		}
	}
	public float countDown =3;
	public string niceTime;
	public GameCtrl ctrl;
	static WaitForStart(){}
	private WaitForStart(){}
	public override void Enter (GameStatus m)
	{
		Debug.Log("Wait for Game to Start!!! ");
		Sound.Instance.playSound(3);
		Door.Instance.CloseDoor();
		if(m.status!=GameStatus.Status.waitforstart){
			m.ChanageStatus(GameStatus.Status.waitforstart);
		}
		else
		{
			Debug.Log("Wait for Game to Start!!! " + m.status.ToString()); 
		}
		ctrl = GameObject.Find("GameControl").GetComponent<GameCtrl>();
		ctrl.SetRandom();
	}
	public override void Execute (GameStatus m)
	{
//		Debug.Log("sadjfjadsf;ljasfasgsdal;gjlfds;j");
		if(GameCtrl.round==0)
		{
			m.ChangeState(GameResult.Instance);
		}else
		{
			Debug.Log("12312312321312------------" +countDown);
			if(countDown>0)
			{
			    countDown -= Time.deltaTime;
				if(countDown >= 0 )
				{
					Debug.Log("2152152152------------" +countDown);
				    int minutes = Mathf.FloorToInt(countDown / 60F);
				    int seconds = Mathf.FloorToInt(countDown - minutes * 60);
				    niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);
				}else
				{
//					Debug.Log("3152152152------------" +countDown);
//					ctrl.goalAngle = Mathf.Round(Random.Range(0.0f,180.0f));
//		
//					ctrl.goalAngle2 = Mathf.Round(Random.Range(0.0f,360.0f));
//				
//				 	ctrl.t1 = Random.Range(1,13);
//				 	ctrl.t2 = Random.Range(1,61);
//				 	ctrl.t3 = Random.Range(1,61);
//				
//				 	ctrl.f1 =ctrl.t1 *30;
//				 	ctrl.f2 =ctrl.t2 *6;
//				 	ctrl.f3 =ctrl.t3 *6;
//					ctrl.goalAngle3 =new float[]{ctrl.f1,ctrl.f2,ctrl.f3};
					if(GameCtrl.goal==GameCtrl.GameGoal.random)
						GameCtrl.goal=GameCtrl.GetRandomEnum<GameCtrl.GameGoal>();
					countDown = 3;
					m.ChangeState(GameStart.Instance);
				}
			}
		}
	}
	
	public override void Exit (GameStatus entity)
	{
		
	}
}

public sealed class GameResult :FSMState<GameStatus>
{
	static readonly GameResult instance = new GameResult();
	public static GameResult Instance{
		get{
			return instance;
		}
	}
	static GameResult(){}
	private GameResult(){}
	public override void Enter (GameStatus m)
	{
		Debug.Log("Show Game Result ");
		Sound.Instance.playStop();
		Sound.Instance.playSound(2);
		if(m.status!=GameStatus.Status.result){
			m.ChanageStatus(GameStatus.Status.result);
		}
		
	}
	public override void Execute (GameStatus m)
	{
		m.ChangeState(GameMenu.Instance);
		Application.LoadLevel("Result");
	}
	
	public override void Exit (GameStatus entity)
	{
	}
}