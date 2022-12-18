using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class DoAction : Action {
    [SerializeField]
    private string logInfo;

    public override TaskStatus OnUpdate() {
        Debug.Log($"name:DoAction!!! {logInfo}");
        return TaskStatus.Running;
    }
}