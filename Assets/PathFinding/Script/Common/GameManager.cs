using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private SOGameMeshData gameMeshData;
    private string gameMeshDataPath = "Assets/PathFinding/Data/SOGameMeshData.asset";
    
    public NodeManager nodeManager { get; private set; }

    private void Awake() {
        var objRoot = new GameObject("NodeRoot").transform;
        objRoot.SetParent(transform);
        objRoot.localPosition = Vector3.zero;
        gameMeshData = UnityEditor.AssetDatabase.LoadAssetAtPath<SOGameMeshData>(gameMeshDataPath);
        
        ObjectPool.Instance.Init(gameMeshData);
        nodeManager = new NodeManager(gameMeshData, objRoot);
    }

    private void Update() {
        nodeManager?.OnUpdate();
    }
}