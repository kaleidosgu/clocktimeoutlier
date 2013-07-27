using UnityEngine;
using System.Collections;

public class Needle : MonoBehaviour {
	
	//指针类型
	public enum Type{hour,min,sec}
	
	public Type type = Type.min;
	
	//旋转速度
	public float rotateSpeed;
	
	//当前指针位置
	public float angle;
	
	//buff状态
	public enum Buff{stop,move,speedup}
	
	//buff状态
	public Buff buffer = Buff.stop;
	
	//行动状态
	public enum Action{stop,move,speedup}
	
	//行动状态
	public Action action = Action.move;
	
	public Control ctrl;
	
	public void Update()
	{
		this.angle = 360-this.gameObject.transform.rotation.eulerAngles.z;
	}
}
