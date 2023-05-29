using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class CheckEnemyFollow : Conditional {
    [SerializeField]
    private SharedGameObject target;
    [SerializeField]
    private List<GameObject> objLists;

    [SerializeField]
    private float checkDistance = 5;

    public override TaskStatus OnUpdate() {
        var targetNowPos = target.Value.transform.position;
        var myPos = transform.position;
        var dis = Vector3.Distance(myPos, targetNowPos);
        return dis < checkDistance? TaskStatus.Success : TaskStatus.Failure;
    }
}