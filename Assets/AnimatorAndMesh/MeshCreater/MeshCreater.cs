using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCreater : MonoBehaviour {
    private void Start() {
        var meshFilter = gameObject.AddComponent<MeshFilter>();
        var meshRender = gameObject.AddComponent<MeshRenderer>();
        var mesh = new Mesh();
        mesh.name = "FFFFFF";
        var vertices = new Vector3[] {
            new Vector3(1, 1, 0),
            new Vector3(-1, 1, 0),
            new Vector3(1, -1, 0),
            new Vector3(-1, -1, 0),
        };
        var triangles = new int[2 * 3] {
            0, 2, 1, 1, 2, 3
            // 0, 3, 1, 0, 2, 3
        };
        // var triangles2 = new int[2 * 3] {
        //     0, 1, 3, 0, 3, 2
        // };

        mesh.vertices = vertices;
        // mesh.triangles = triangles;
        mesh.subMeshCount = 1;
        mesh.SetTriangles(triangles, 0);
        // mesh.SetTriangles(triangles2, 1);
        for (var i = 0; i < vertices.Length; i++) {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = vertices[i];
            cube.transform.localScale = Vector3.one * 0.2f;
            cube.name = i.ToString();
        }
        

        meshFilter.sharedMesh = mesh;
        meshRender.materials = new Material[] {
            new Material(Shader.Find("Standard")), new Material(Shader.Find("Standard"))
        };

        var point = new Vector4(2, 2, 2, 1);
        var moveMatrix = Matrix4x4.identity;
        moveMatrix.m03 = -1;
        moveMatrix.m13 = 0;
        moveMatrix.m23 = 0;
        DebugEx.LogError(moveMatrix * point);

        var list = new List<int> {
            1,
            2,
            3,
            4,
            5,
            6,
            7
        };
        for (var i = 0; i < list.Count; i++) {
            DebugEx.LogError(i);
            if (i > 5) {
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        DebugEx.Log(other.name);
    }
}