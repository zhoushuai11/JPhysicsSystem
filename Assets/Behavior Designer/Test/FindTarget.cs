using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class FindTarget : Conditional {
    [SerializeField]
    private SharedGameObject target;
    
    public override TaskStatus OnUpdate() {
        if (null == target.Value) {
            target.Value = gameObject;
        }

        if (null == target.Value) {
            return TaskStatus.Failure;
        } else {
            return TaskStatus.Success;
        }
    }
}