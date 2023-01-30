using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TestGpuInstance : MonoBehaviour {
    public GameObject Cube;
    public int instanceCount = 10;

    public Material testMat;
    private Mesh testMesh;

    private Matrix4x4[] matrix;
    private Vector4[] colors;
    private MaterialPropertyBlock propertyBlock;
    private void Start() {
        var obj = GameObject.Instantiate(Cube);
        testMesh = obj.GetComponent<MeshFilter>().sharedMesh;
        testMat = obj.GetComponent<MeshRenderer>().sharedMaterial;
        Destroy(obj);
        matrix = new Matrix4x4[instanceCount];
        colors = new Vector4[instanceCount];
        propertyBlock = new MaterialPropertyBlock();
        for (int i = 0; i < instanceCount; i++) {
            float x = Random.Range(-20, 20);
            float y = Random.Range(-3, 3);
            float z = 0;
            var nowMatrix = Matrix4x4.identity;
            nowMatrix.SetColumn(3, new Vector4(x, y, z, 1));
            matrix[i] = nowMatrix;
            colors[i] = new Vector4(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                1);
            propertyBlock.SetVectorArray("_Color", colors);
        }
    }

    private void Update() {
        Graphics.DrawMeshInstanced(testMesh, 0, testMat, matrix, matrix.Length, propertyBlock);
    }

}