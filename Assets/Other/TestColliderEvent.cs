using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class TestColliderEvent : MonoBehaviour {
    private void Start() {
        var meshRender = GetComponent<MeshRenderer>();
        var meshFilter = GetComponent<MeshFilter>();
        var mesh = meshFilter.mesh;
    }
}