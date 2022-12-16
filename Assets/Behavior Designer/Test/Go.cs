using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class Go : Action {
    public SharedInt GlobalRatio;

    public override void OnStart() {
    }

    public override TaskStatus OnUpdate() {
        return TaskStatus.Success;
    }
}

[System.Serializable]
public class CustomClass {
    public int myInt;
    public Object myObject;
}

[System.Serializable]
public class SharedCustomClass : SharedVariable<CustomClass> {
    public static implicit operator SharedCustomClass(CustomClass value) {
        return new SharedCustomClass {
            Value = value
        };
    }
}