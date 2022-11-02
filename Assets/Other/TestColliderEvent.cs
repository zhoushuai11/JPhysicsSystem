using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class TestColliderEvent : MonoBehaviour {
    public GameObject cube;
    private Transform nowTrans;

    private void Start() {
        nowTrans = cube.transform;
    }

    private float time = 0.2f;
    private bool isTrue = false;
    private Vector3 pos = Vector3.back;

    private void Update() {
        if (time > 0) {
            time -= 0.2f;
            return;
        }

        time = 0.2f;

        if (isTrue) {
            Profiler.BeginSample("UseTransform");
            for (int i = 0; i < 10000; i++) {
                cube.transform.position = pos;
            }
            Profiler.EndSample();
        } else {
            Profiler.BeginSample("NoTransform");
            for (int i = 0; i < 10000; i++) {
                nowTrans.position = pos;
            }
            Profiler.EndSample();
        }

        isTrue = !isTrue;
    }
}