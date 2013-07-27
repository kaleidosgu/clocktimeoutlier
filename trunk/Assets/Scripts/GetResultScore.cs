using UnityEngine;
using System.Collections;

public class GetResultScore : MonoBehaviour {
	public UILabel hourLabel;
	public UILabel minuteLabel;
	public UILabel secondLabel;
	
	public UISprite hourSprite;
	public UISprite minuteSprite;
	public UISprite secondSprite;
	
	public GameObject hourLogo;
	public GameObject minuteLogo;
	public GameObject secondLogo;
	
	
	// Use this for initialization
	void Start () {
	float tempValue = PlayerPrefs.GetFloat("hour");
	int winner = 0;
	if(PlayerPrefs.GetFloat("hour") < PlayerPrefs.GetFloat("minute"))
		{
			tempValue = PlayerPrefs.GetFloat("minute");
			winner  = 1;
		}
	if(PlayerPrefs.GetFloat("hour") < PlayerPrefs.GetFloat("second"))
		{
			tempValue = PlayerPrefs.GetFloat("second");
			winner  = 2;
		}
	switch (winner)
		{
		case 0:
			hourLogo.SetActive(true);
			minuteLogo.SetActive(false);
			secondLogo.SetActive(false);
			break;
		case 1:
			hourLogo.SetActive(false);
			minuteLogo.SetActive(true);
			secondLogo.SetActive(false);
			break;
		case 2:
			hourLogo.SetActive(false);
			minuteLogo.SetActive(false);
			secondLogo.SetActive(true);
			break;
		}
		hourSprite.fillAmount = PlayerPrefs.GetFloat("hour") * 0.001f;
		minuteSprite.fillAmount = PlayerPrefs.GetFloat("minute") * 0.001f;
		secondSprite.fillAmount = PlayerPrefs.GetFloat("second") * 0.001f;
		
		
	hourLabel.text = PlayerPrefs.GetFloat("hour").ToString();
	minuteLabel.text = PlayerPrefs.GetFloat("minute").ToString();
	secondLabel.text = PlayerPrefs.GetFloat("second").ToString();
		
	}
	
	
}
