using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class CheckEnemyLock : Conditional {
    [SerializeField]
    private SharedGameObject target;

    [SerializeField]
    private List<GameObject> objLists;

    [SerializeField]
    private float checkDistance = 3;

    public override TaskStatus OnUpdate() {
        if (target.Value == null) {
            return CheckLockEnemy()? TaskStatus.Success : TaskStatus.Failure;
        } else {
            var targetNowPos = target.Value.transform.position;
            var myPos = transform.position;
            var dis = Vector3.Distance(myPos, targetNowPos);
            if (dis < checkDistance) {
                target.Value = null;
                return CheckLockEnemy()? TaskStatus.Success : TaskStatus.Failure;
            }
        }

        return TaskStatus.Success;
    }

    private bool CheckLockEnemy() {
        foreach (var obj in objLists) {
            var targetNowPos = obj.transform.position;
            var myPos = transform.position;
            var dis = Vector3.Distance(myPos, targetNowPos);
            if (dis < checkDistance) {
                target.Value = obj;
                return true;
            }
        }

        return false;
    }
}