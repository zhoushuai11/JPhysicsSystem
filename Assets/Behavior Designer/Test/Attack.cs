using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class Attack : Action {
    [SerializeField]
    private float attackRatio = 1.0f;

    private float time = 0.0f;
    public override TaskStatus OnUpdate() {
        if(time < attackRatio) {
            time += Time.deltaTime;
            return TaskStatus.Running;
        }
        time = 0.0f;
        DebugEx.LogError("Attack!");
        return TaskStatus.Running;
    }
}