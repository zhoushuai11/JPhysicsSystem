using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class Go : Conditional
{
	[SerializeField]
	private SharedGameObject targetGameObject;
	public override void OnStart()
	{
		Debug.Log("Go");
	}

	public override TaskStatus OnUpdate()
	{
		return TaskStatus.Running;
	}
}