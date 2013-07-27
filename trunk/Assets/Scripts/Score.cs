using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {

	private static Score _instance;
		private float hourFloat;
		private float minuteFloat;
		private float secondFloat;
	public UISprite hourSprite;
	public UISprite minuteSprite;
	public UISprite secondSprite;
	public Control hourControl;
	public Control minuteControl;
	public Control secondControl;
	public static Score Instance
	{
		get{
			if(!_instance)
			_instance = new Score();
			return _instance;
			}
	}
	// Use this for initialization
	void Start () {
	_instance = this;
	hourFloat = 0.0f;
	minuteFloat = 0.0f;
	secondFloat = 0.0f;
	}
	
	public void AddHourScore(float num)
	{
		hourFloat += num * 0.001f ;	
		PlayerPrefs.SetFloat("hour",hourFloat * 1000);
	}
	
		public void AddMinuteScore(float num)
	{
		minuteFloat+= num * 0.001f ;	
		PlayerPrefs.SetFloat("minute",minuteFloat * 1000);
	}
	
		public void AddSecondScore(float num)
	{
		secondFloat += num * 0.001f ;
		PlayerPrefs.SetFloat("second",secondFloat * 1000);
	}
	
	void Update()
	{
		hourSprite.fillAmount = hourControl.actionNum / hourControl.actionBar;
		minuteSprite.fillAmount = minuteControl.actionNum / minuteControl.actionBar;
		secondSprite.fillAmount = secondControl.actionNum / secondControl.actionBar;
		
	}
}
