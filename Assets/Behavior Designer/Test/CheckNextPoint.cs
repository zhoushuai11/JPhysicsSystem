using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class CheckNextPoint : Conditional {
    [SerializeField]
    private SharedVector3 point;

    [SerializeField]
    private List<Vector3> points;

    private int index;
    private bool inInit = false;

    public override TaskStatus OnUpdate() {
        if (points.Count == 0) {
            return TaskStatus.Failure;
        }

        if (!inInit) {
            index = 0;
            point.Value = points[0];
            inInit = true;
        }

        var dis = Vector3.Distance(transform.position, point.Value);
        if (dis < 0.2f) {
            index++;
            if (index >= points.Count) {
                index = 0;
            }
            point.Value = points[index];
        }

        return TaskStatus.Success;
    }
}