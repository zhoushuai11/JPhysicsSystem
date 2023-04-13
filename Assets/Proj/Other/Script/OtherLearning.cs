using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherLearning : MonoBehaviour {
    private Transform targetTransform;

    // Start is called before the first frame update
    void Start() {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        targetTransform = obj.transform;
        GameObject nullObj = null;
        if (nullObj.isStatic) {
            Debug.LogError("TTTTT");
        }
    }

    // Update is called once per frame
    void Update() {
        var objDir = targetTransform.forward;
        var worldForward = Vector3.forward;
        var angle = Vector3.Angle(worldForward, objDir);
        // Debug.LogError(angle);
    }
}