using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class CheckEnemyAttack : Conditional {
    [SerializeField]
    private SharedGameObject target;

    [SerializeField]
    private float checkDistance = 1f;

    public override TaskStatus OnUpdate() {
        if (null == target.Value) {
            return TaskStatus.Failure;
        }
        var targetNowPos = target.Value.transform.position;
        var myPos = transform.position;
        var dis = Vector3.Distance(myPos, targetNowPos);
        return dis < checkDistance? TaskStatus.Success : TaskStatus.Failure;
    }
}