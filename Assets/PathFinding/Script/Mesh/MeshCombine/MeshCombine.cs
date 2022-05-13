using System;
using UnityEngine;

public class MeshCombine : MonoBehaviour {
    public GameObject[] meshs;
    public bool isStart = false;

    private void Update() {
        if (isStart) {
            isStart = false;
            CombineMesh();
        }
    }

    private void CombineMesh() {
        var len = meshs.Length;
        var combine = new CombineInstance[len];
        var mats = new Material[len];
        
        for (int i = 0; i < len; i++) {
            var targetMeshFillter = meshs[i].GetComponent<MeshFilter>();
            combine[i].mesh = targetMeshFillter.sharedMesh;
            combine[i].transform = targetMeshFillter.transform.localToWorldMatrix;

            var targetMeshRender = meshs[i].GetComponent<MeshRenderer>();
            mats[i] = targetMeshRender.sharedMaterial;

            DestroyImmediate(meshs[i]);
        }
        
        var newMesh = new Mesh();
        newMesh.name = "TestCombineMesh";
        newMesh.CombineMeshes(combine);

        var obj = new GameObject("CombineResult");
        var meshFilter = obj.AddComponent<MeshFilter>();
        var meshRender = obj.AddComponent<MeshRenderer>();
        
        meshFilter.mesh = newMesh;
        meshRender.sharedMaterials = mats;

        var collider = obj.AddComponent<MeshCollider>();
    }
}