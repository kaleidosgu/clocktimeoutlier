using UnityEngine;
using System.Collections;

public class GameCtrl : MonoBehaviour {
	public Needle hour;
	
	public Needle minute;
	
	public Needle second;
	
	public GameStatus status; 
	
	//任务类型 :1 指定角度 2 指定达到同一点 3 指定到达不同点
	public enum GameGoal {angle=0,pos,time,random}
	
	//关卡任务目标
	public static GameGoal goal;
	
	//关卡回合数
	public static int round;
	
	//任务1 指定的两者之间的角度
	public static float goalAngle;
	
	//任务2指定时间需要到达的点
	public static float goalAngle2;
	
	public float[] goalAngle3;
	
	public float countDown;
	
	public int t1,t2,t3;
	
	public float f1,f2,f3;
	public void Start()
	{
		if(!hour)
			hour = GameObject.Find("hour").GetComponent<Needle>();
		if(!minute)
			minute = GameObject.Find("minute").GetComponent<Needle>();
		if(!second)
			second = GameObject.Find("second").GetComponent<Needle>();
		if(!status)
			status = GameObject.Find("GameStatus").GetComponent<GameStatus>();
		
//		goalAngle = Mathf.Round(Random.Range(0.0f,180.0f));
//		
//		goalAngle2 = Mathf.Round(Random.Range(0.0f,360.0f));
//	
//	 	t1 = Random.Range(1,13);
//	 	t2 = Random.Range(1,61);
//	 	t3 = Random.Range(1,61);
//	
//	 	f1 =t1 *30 ;
//	 	f2 =t2 *6;
//	 	f3 =t3 *6;
//		goalAngle3 =new float[]{f1,f2,f3};
//		if(goal==GameGoal.random)
//			goal=GetRandomEnum<GameGoal>();
		SetRandom();
		status.ChangeState(WaitForStart.Instance);
	}
	
	public void SetRandom()
	{
		goalAngle = Mathf.Round(Random.Range(30.0f,180.0f));
		
		goalAngle2 = Mathf.Round(Random.Range(0.0f,360.0f));
	
	 	t1 = Random.Range(1,13);
	 	t2 = Random.Range(1,61);
	 	t3 = Random.Range(1,61);
	
	 	f1 =t1 *30 ;
	 	f2 =t2 *6;
	 	f3 =t3 *6;
		goalAngle3 =new float[]{f1,f2,f3};
		if(goal==GameGoal.random)
			goal=GetRandomEnum<GameGoal>();
	}
	
	//设置关卡任务目标
	public void SetGoal(GameGoal goal)
	{
		goal=goal;
	}
	
	//获取两针之间的角度
	public float GetAngle(Needle from,Needle to)
	{
		return Vector3.Angle(-from.transform.right,-to.transform.right);
	}
	
	//判断是否游戏结束
	public int[] GameOver()
	{
		switch(goal)
		{
			case GameGoal.angle:
			if(Mathf.RoundToInt(GetAngle(hour,second) - goalAngle)==0)
			{
				int[] param= {1,0}; 
				return param;
			}
			if(Mathf.RoundToInt(GetAngle(hour,minute)-goalAngle)==0)
			{
				int[] param= {1,1}; 
				return param;	
			}
			if(Mathf.RoundToInt(GetAngle(minute,second)-goalAngle)==0)
			{
				int[] param= {1,2}; 
				return param;
			}
			break;
			case GameGoal.pos:
			if(Mathf.RoundToInt(hour.angle)==goalAngle2)
			{
				int[] param = {1,0};
				return param;
			}
			if(Mathf.RoundToInt(minute.angle)==goalAngle2)
			{
				int[] param = {1,1};
				return param;
			}
			if(Mathf.RoundToInt(second.angle)==goalAngle2)
			{
				int[] param = {1,2};
				return param;
			}
			break;
			case GameGoal.time:
			if(Mathf.RoundToInt(hour.angle)==goalAngle3[0])
			{
				int[] param = {1,0};
				return param;
			}
			if(Mathf.RoundToInt(minute.angle)==goalAngle3[1])
			{
				int[] param = {1,1};
				return param;
			}
			if(Mathf.RoundToInt(second.angle)==goalAngle3[2])
			{
				int[] param = {1,2};
				return param;
			}
			break;
			
		}
		int[] result={0,0};
		return result;
	}
	public string niceTime;

	void Update()
	{
		if(status.status!=GameStatus.Status.roundstart)
		{
//			if(round==0)
//			{
//				status.ChangeState(GameResult.Instance);
//			}else
//			{
//				if(countDown>0)
//				{
//				    countDown -= Time.deltaTime;
//					if(countDown >= 0 )
//					{
//					    int minutes = Mathf.FloorToInt(countDown / 60F);
//					    int seconds = Mathf.FloorToInt(countDown - minutes * 60);
//					    niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);
//					}else
//					{
//						status.ChangeState(GameStart.Instance);
//						if(goal==GameGoal.random)
//							goal=GetRandomEnum<GameGoal>();
//						countDown = 3;
//					}
//				}
//			}
		}
		if(status.status==GameStatus.Status.roundstart)
		{
			if(GameOver()[0]==1)
			{
				switch(goal)
				{
					case GameGoal.angle:
					switch(GameOver()[1])
					{
						case 0:
							Debug.Log("Game over ! Hour,Second Wins");
							Score.Instance.AddHourScore(100);
							Score.Instance.AddSecondScore(100);
						break;
						case 1:
							Debug.Log("Game over ! Hour,Minute Wins");
							Score.Instance.AddHourScore(100);
							Score.Instance.AddMinuteScore(100);
						break;
						case 2:
							Debug.Log("Game over ! Minute,Second Wins");
							Score.Instance.AddSecondScore(100);
							Score.Instance.AddMinuteScore(100);
						break;
						
					}
					if(status.status!=GameStatus.Status.roundover)
						status.ChangeState(GameOVer.Instance);
					break;
					case GameGoal.pos:
					switch(GameOver()[1])
					{
						case 0:
							Debug.Log("Game over ! Hour Wins");
							Score.Instance.AddHourScore(100);							
						break;
						case 1:
							Debug.Log("Game over ! Minute Wins");
							Score.Instance.AddMinuteScore(100);
						break;
						case 2:
							Debug.Log("Game over ! Second Wins");
							Score.Instance.AddSecondScore(100);						
						break;
					}
					if(status.status!=GameStatus.Status.roundover)
						status.ChangeState(GameOVer.Instance);
					break;
					case GameGoal.time:
					switch(GameOver()[1])
					{
						case 0:
							Debug.Log("Game over ! Hour Wins");
							Score.Instance.AddHourScore(100);		
						break;
						case 1:
							Debug.Log("Game over ! Minute Wins");
							Score.Instance.AddMinuteScore(100);
						break;
						case 2:
							Debug.Log("Game over ! Second Wins");
							Score.Instance.AddSecondScore(100);	
						break;
					}
					if(status.status!=GameStatus.Status.roundover)
						status.ChangeState(GameOVer.Instance);
					break;
				}
			}
			if(Input.GetKeyDown(KeyCode.Space))
			{
				Time.timeScale=1;
				Application.LoadLevel(Application.loadedLevelName);
				status.ChangeState(GameStart.Instance);
			}
		}
	}
	
	void OnGUI()
	{
		/*
		GUILayout.Label("Hour : second "+GetAngle(hour,second).ToString());
		GUILayout.Label("Hour : minute "+GetAngle(hour,minute).ToString());
		GUILayout.Label("minute : second "+GetAngle(minute,second).ToString());
		GUILayout.Label("Hour : "+hour.angle.ToString());
		GUILayout.Label("Minute : "+minute.angle.ToString());
		GUILayout.Label("Seconds : "+second.angle.ToString());
		GUILayout.FlexibleSpace();
		GUILayout.Label("当前status : "+status.status.ToString());
		switch(goal){
		case GameGoal.angle:
			GUILayout.Label("需要达成角度 : "+goalAngle);
			break;
		case GameGoal.pos:
		GUILayout.Label("需要达成角度2 : "+goalAngle2);
		
			break;
		case GameGoal.time:
		GUILayout.Label("需要摆成的时间 : "+t1+" H "+t2+" M "+t3 +" S ");
		GUILayout.Label("角度 : "+f1.ToString()+"----"+f2.ToString()+"----"+f3.ToString());
			break;
		}*/
//		if(GUILayout.Button("模式1"))
//		{
//			goal=GameGoal.angle;
//			status.ChangeState(GameStart.Instance);
//		}
//		if(GUILayout.Button("模式1"))
//		{
//			goal=GameGoal.pos;
//			status.ChangeState(GameStart.Instance);
//		}
//		if(GUILayout.Button("模式1"))
//		{
//			goal=GameGoal.time;
//			status.ChangeState(GameStart.Instance);
//		}
//		if(GUILayout.Button("随机"))
//		{
//			goal = GetRandomEnum<GameGoal>();
//			status.ChangeState(GameStart.Instance);
//		}
		//GUILayout.Label("当前模式 :　"+goal.ToString());
		//GUILayout.Label("倒计时 : "+niceTime);
	}
	
	public static T GetRandomEnum<T>()
	{
	    System.Array A = System.Enum.GetValues(typeof(T));
	    T V = (T)A.GetValue(UnityEngine.Random.Range(0,A.Length));
	    return V;
	}
	
	public void Moshi1()
	{
		goal=GameGoal.angle;
		status.ChangeState(GameStart.Instance);
	}
}
