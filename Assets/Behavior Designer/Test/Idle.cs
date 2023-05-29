using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class Idle : Action {

    public override TaskStatus OnUpdate() {
        return TaskStatus.Running;
    }
}