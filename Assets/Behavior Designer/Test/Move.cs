using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class Move : Action {
    [SerializeField]
    private SharedVector3 point;

    [SerializeField]
    private float speed = 1.0f;

    public override TaskStatus OnUpdate() {
        transform.position = Vector3.Lerp(transform.position, point.Value, 0.05f * speed);
        if(Vector3.Distance(transform.position, point.Value) < 0.05f) {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }
}