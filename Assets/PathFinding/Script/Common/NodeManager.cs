using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NodeManager {
    public NodeType UIPencilType { get; private set; }
    
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
        var scale = gameMeshData.objScale;
        var prefab = gameMeshData.defaultObj;
        prefab.transform.localScale = Vector3.one * scale;
        for (int i = 0; i < maxX; i++) {
            for (int j = 0; j < maxY; j++) {
                var pos = GetPosByScaleAndIndex(scale, i, j);
                var obj = GameObject.Instantiate(prefab, objRoot);
                obj.transform.localPosition = pos;
                obj.name = $"{i},{j}";
                var nodeComponent = obj.AddComponent<Node>();
                nodeComponent.Init(i, j, NodeType.Default);
                nodeDic.Add(nodeComponent, obj);
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
        
        node.ChangeNodeType(type, setMats[(int)type]);
    }

    private bool isFinding = false;
    public void StartFind() {
        if (null == startNode || null == endNode || isFinding) {
            return;
        }

        isFinding = true;
    }

    public void ResetFind() {
        foreach (var node in nodeDic.Keys) {
            ChangeNodeType(node, NodeType.Default);
        }
    }

    public void ChangeUIPencilNode(int nt) {
        UIPencilType = (NodeType)nt;
    }
}

public class Node : MonoBehaviour {
    public int x;
    public int y;
    public NodeType nodeType;

    private SpriteRenderer render;

    public void Init(int setX, int setY, NodeType setNodeType) {
        x = setX;
        y = setY;
        nodeType = setNodeType;
        render = transform.GetComponent<SpriteRenderer>();
    }

    public void ChangeNodeType(NodeType nt, Material mat) {
        if (nodeType == nt) {
            return;
        }

        nodeType = nt;
        render.material = mat;
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