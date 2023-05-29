using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class Follow : Action {
    [SerializeField]
    private SharedGameObject target;

    [SerializeField]
    private float speed = 1.0f;

    public override TaskStatus OnUpdate() {
        if (null == target.Value) {
            return TaskStatus.Failure;
        }

        var trans = transform;
        trans.position = Vector3.Lerp(trans.position, target.Value.transform.position, 0.05f * speed);
        return TaskStatus.Running;
    }
}