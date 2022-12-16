using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class Conditonal : Conditional {
    public Transform target;
    public int ratio;

    public override void OnStart() {
    }

    public override TaskStatus OnUpdate() {
        return TaskStatus.Success;
    }
}