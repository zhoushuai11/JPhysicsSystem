using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class NodeManager {
    private NodeType UIPencilType { get; set; }
    private FindingPathType UIFindingPathType { get; set; }
    
    private SOGameMeshData gameMeshData;
    
    private int nodeLayer;
    private Material[] setMats; // 0:DefaultNode 1:StartNode 2:EndNode 3:WallNode
    private int maxX;
    private int maxY;
    private Transform objRoot;

    private Node lastTouchNode;
    private Node nowTouchNode;

    private Node startNode;
    private Node endNode;
    
    private Dictionary<Node, GameObject> nodeDic = new Dictionary<Node, GameObject>();

    private bool isBtnDown = false;
    
    public void Init(SOGameMeshData gameMeshData,Transform root) {
        this.gameMeshData = gameMeshData;
        nodeLayer = 1 << LayerMask.NameToLayer("Node");
        setMats = gameMeshData.materials;
        UIPencilType = default;
        objRoot = root;
        CreateNode();
    }

    private void CreateNode() {
        maxX = gameMeshData.xNum;
        maxY = gameMeshData.yNum;
        
        // 设置默认 start and end
        var startIndexX = Random.Range(0, maxX);
        var startIndexY = Random.Range(0, maxY);
        var endIndexX = (startIndexX < maxX / 2)? Random.Range(startIndexX + 1, maxX) : Random.Range(0, startIndexX);
        var endIndexY = (startIndexY < maxY / 2)? Random.Range(startIndexY + 1, maxY) : Random.Range(0, startIndexY);
        
        var scale = gameMeshData.objScale;
        var prefab = gameMeshData.defaultObj;
        prefab.transform.localScale = Vector3.one * scale;
        for (int i = 0; i < maxY; i++) {
            for (int j = 0; j < maxX; j++) {
                var pos = GetPosByScaleAndIndex(scale, i, j);
                var obj = GameObject.Instantiate(prefab, objRoot);
                obj.transform.localPosition = pos;
                obj.name = $"{i},{j}";
                var nodeComponent = obj.AddComponent<Node>();
                nodeComponent.Init(i, j, NodeType.Default, setMats);
                nodeDic.Add(nodeComponent, obj);
                if (i == startIndexX && j == startIndexY) {
                    ChangeNodeType(nodeComponent, NodeType.Start);
                } else if (i == endIndexX && j == endIndexY) {
                    ChangeNodeType(nodeComponent, NodeType.End);
                }
            }
        }
    }

    private Vector2 GetPosByScaleAndIndex(float scale, int x, int y) {
        var xcenter = maxX / 2;
        var ycenter = maxY / 2;
        var xPos = (x - xcenter) * scale * 1.03f;
        var yPos = (y - ycenter) * scale * 1.03f;
        return new Vector2(xPos, yPos);
    }
    
    public void OnUpdate() {
        if (Input.GetMouseButtonDown(0)) {
            OnMouseBtnDown();
        }

        if (Input.GetMouseButton(0)) {
            OnMouseBtnStay();
        }

        if (Input.GetMouseButtonUp(0)) {
            OnMouseBtnUp();
        }
    }

    private void OnMouseBtnDown() {
        isBtnDown = true;
        CheckNowTouchNode(true);
        lastTouchNode = nowTouchNode;
    }

    private void OnMouseBtnStay() {
        if (!isBtnDown || (UIPencilType != NodeType.Wall && UIPencilType != NodeType.Default)) {
            return;
        }
        
        CheckNowTouchNode(false);
        if (lastTouchNode != nowTouchNode) {
            ChangeNodeType(nowTouchNode, UIPencilType);
            lastTouchNode = nowTouchNode;
        }
    }

    private void OnMouseBtnUp() {
        if (!isBtnDown) {
            return;
        }
        isBtnDown = false;
        nowTouchNode = null;
        lastTouchNode = null;
    }

    private void CheckNowTouchNode(bool isSetState) {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var newMousePos = new Vector2(mousePos.x, mousePos.y);
        var nowCheckNode = Physics2D.OverlapPoint(newMousePos, nodeLayer);
        if (nowCheckNode != null) {
            var nodeComponent = nowCheckNode.GetComponent<Node>();
            if (null != nodeComponent) {
                nowTouchNode = nodeComponent;
                if (isSetState) {
                    ChangeNodeType(nowTouchNode, UIPencilType);
                }
            }
        }
    }

    private void ChangeNodeType(Node node, NodeType type) {
        if (type == NodeType.Start) {
            if (startNode != null) {
                ChangeNodeType(startNode, NodeType.Default);
            }
            startNode = node;
        } else if (type == NodeType.End) {
            if (endNode != null) {
                ChangeNodeType(endNode, NodeType.Default);
            }
            endNode = node;
        }

        if (node.nodeType == NodeType.Start) {
            startNode = null;
        } else if (node.nodeType == NodeType.End) {
            endNode = null;
        }
        
        node.ChangeNodeType(type);
    }

    private bool isFinding = false;
    public void StartFind() {
        if (null == startNode || null == endNode || isFinding) {
            return;
        }

        isFinding = true;
        PathFindingBase pathFinding = null;
        switch (UIFindingPathType) {
            case FindingPathType.BFS:
                pathFinding = new BFS();
                break;
            case FindingPathType.DIJKSTRA:
                break;
            case FindingPathType.ASTAR:
                break;
            case FindingPathType.JPS:
                break;
        }
        pathFinding?.Init(nodeDic, maxX, maxY);
    }

    public void ResetFind() {
        foreach (var node in nodeDic.Keys) {
            ChangeNodeType(node, NodeType.Default);
        }
        isFinding = false;
    }

    public void ChangeUIPencilNode(int nt) {
        UIPencilType = (NodeType)nt;
    }

    public void ChangePathFindingType(int nt) {
        UIFindingPathType = (FindingPathType)nt;
    }
}

public class Node : MonoBehaviour {
    public int x;
    public int y;
    public NodeType nodeType;
    public int ParentNodeIndex { get; set; } = -1;

    private SpriteRenderer render;
    private TextMesh textMesh;
    private Material[] mats;
    private const float TIMEOFFSET = 0.1f;

    public void Init(int setX, int setY, NodeType setNodeType, Material[] mats) {
        x = setX;
        y = setY;
        nodeType = setNodeType;
        render = transform.GetComponent<SpriteRenderer>();
        textMesh = transform.GetComponentInChildren<TextMesh>();
        this.mats = mats;
        SetText("");
    }

    public void ChangeNodeType(NodeType nt) {
        if (nodeType == nt) {
            return;
        }

        nodeType = nt;
        render.material = mats[(int)nt];
        if (nt != NodeType.FinalWay) {
            SetText("");
        }

        if (nt == NodeType.Default) {
            ParentNodeIndex = -1;
        }
    }

    public void SetText(string t) {
        textMesh.text = t;
    }

    private bool isStartDelayShowPoint = false;
    private float delayShowPointTime = 0.0f;
    private bool isStartDelayShowFinalWay = false;
    private int finalPointIndex = 0;
    private float delayShowFinal = 0.0f;
    private float allWayCountTime = 0.0f;
    /// <summary>
    /// 演示搜索路径
    /// </summary>
    /// <param name="index">第几步</param>
    public void DelayShowCheckWay(int index) {
        delayShowPointTime = TIMEOFFSET * index;
        isStartDelayShowPoint = true;
    }

    /// <summary>
    /// 演示最终路径
    /// </summary>
    /// <param name="index">第几步</param>
    /// <param name="allWayPointCount">一共几部</param>
    public void DelayShowFinalWay(int index, int allWayPointCount) {
        allWayCountTime = allWayPointCount * TIMEOFFSET;
        finalPointIndex = index;
        delayShowFinal = index * TIMEOFFSET;
        isStartDelayShowFinalWay = true;
    }

    private void Update() {
        if (!isStartDelayShowPoint && !isStartDelayShowFinalWay) {
            return;
        }

        if (isStartDelayShowPoint) {
            delayShowPointTime -= Time.deltaTime;
            if (delayShowPointTime <= 0) {
                ChangeNodeType(NodeType.Checked);
                isStartDelayShowPoint = false;
            }
        }

        if (isStartDelayShowFinalWay) {
            if (allWayCountTime > 0) {
                allWayCountTime -= Time.deltaTime;
            } else {
                delayShowFinal -= Time.deltaTime;
                if (delayShowFinal < 0) {
                    isStartDelayShowFinalWay = false;
                    ChangeNodeType(NodeType.FinalWay);
                    SetText(finalPointIndex.ToString());
                }
            }
        }
    }
}

public enum NodeType {
    Default,
    Start,
    End,
    Wall,
    Checked,
    FinalWay,
}