using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class CheckDistance : Conditional {
    [SerializeField]
    private SharedGameObject targetGameObject;

    [SerializeField]
    private float checkDistance;

    public override TaskStatus OnUpdate() {
        var nowPos = targetGameObject.Value.transform.position;
        var dis = Vector3.Distance(nowPos, Vector3.zero);
        Debug.Log($"Distance:{dis}");
        if (dis > checkDistance) {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}