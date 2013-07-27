using UnityEngine;
using System.Collections;

public class Sound : MonoBehaviour {

	public AudioClip bg_Audio;
	public AudioClip[] _Aduio;
	private static Sound instance;
	// Use this for initialization
	
	void Start () {
		instance = this;
		audio.Play();
	}
	
	public static Sound Instance
	{
			get
			{
				if(!instance)
					instance = new Sound();
				return instance;
			}
				
		}
	
	
	public void playSound(int num)
	{
		audio.PlayOneShot(_Aduio[num]);
		}
		
	public void playStop()
	{
		audio.Stop();
		
		}
	
}
