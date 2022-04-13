using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class NodeManager {
    private NodeType UIPencilType { get; set; }
    private FindingPathType UIFindingPathType { get; set; }
    private bool HasNodeValue { get; set; } = true;
    
    private SOGameMeshData gameMeshData;
    
    private int nodeLayer;
    private Material[] setMats; // 0:DefaultNode 1:StartNode 2:EndNode 3:WallNode
    private int xMax;
    private int yMax;
    private Transform objRoot;

    private Node lastTouchNode;
    private Node nowTouchNode;

    private Node startNode;
    private Node endNode;

    private Dictionary<int, Node> nodeDic = new Dictionary<int, Node>();

    private Dictionary<NodePos, GameObject> txtDic = new Dictionary<NodePos, GameObject>();
    private Dictionary<NodePos, int> nodeValueDic = new Dictionary<NodePos, int>();

    private bool isBtnDown = false;

    public NodeManager(SOGameMeshData gameMeshData, Transform root) {
        this.gameMeshData = gameMeshData;
        nodeLayer = 1 << LayerMask.NameToLayer("Node");
        setMats = gameMeshData.materials;
        UIPencilType = default;
        objRoot = root;
        CreateNode();
    }

    private void CreateNode() {
        xMax = gameMeshData.xNum;
        yMax = gameMeshData.yNum;
        var scale = gameMeshData.objScale;
        NodeUtil.Init(xMax, yMax, scale, gameMeshData.nodeOffset);
        
        // 设置默认 start and end
        var startIndexX = Random.Range(0, xMax);
        var startIndexY = Random.Range(0, yMax);
        var endIndexX = (startIndexX < xMax / 2)? Random.Range(startIndexX + 1, xMax) : Random.Range(0, startIndexX);
        var endIndexY = (startIndexY < yMax / 2)? Random.Range(startIndexY + 1, yMax) : Random.Range(0, startIndexY);
        
        for (int i = 0; i < yMax; i++) {
            for (int j = 0; j < xMax; j++) {
                var pos = NodeUtil.GetPosByXAndY(i, j);
                var obj = ObjectPool.Instance.GetObjBySign(ObjSign.Node);
                obj.transform.parent = objRoot;
                obj.transform.localScale = Vector3.one * scale;
                obj.transform.localPosition = pos;
                obj.name = $"{i},{j}";
                var nodeComponent = obj.AddComponent<Node>();
                nodeComponent.Init(i, j, NodeType.Default, setMats);
                nodeDic.Add(NodeUtil.GetIndexByXAndY(i, j), nodeComponent);
                if (i == startIndexX && j == startIndexY) {
                    ChangeNodeType(nodeComponent, NodeType.Start);
                } else if (i == endIndexX && j == endIndexY) {
                    ChangeNodeType(nodeComponent, NodeType.End);
                }
            }
        }

        UpdateAroundValue();
        UpdateNodeValue();
    }

    // 更新各个节点之间的可到达节点
    private void UpdateAroundValue() {
        foreach (var node in nodeDic.Values) {
            node.NodeValue.AddAroundNode(NodeUtil.GetAroundTableNode(node.x, node.y));
        }
    }
    
    // 更新两节点之间的权值
    private void UpdateNodeValue() {
        var range = gameMeshData.valueRange;
        var rangeX = (int)range.x;
        var rangeY = (int)range.y;
        
        var random = 0;
        for (int i = 0; i < yMax; i++) {
            for (int j = 0; j < xMax; j++) {
                var index = NodeUtil.GetIndexByXAndY(i, j);
                var node = nodeDic[index];
                var aroundList = node.NodeValue.AroundList;
                if(null != aroundList && aroundList.Count > 0){
                    foreach (var nextIndex in aroundList) {
                        var v2 = new NodePos(index, nextIndex);
                        var otherWayValue = new NodePos(nextIndex, index);
                        if (!nodeValueDic.ContainsKey(otherWayValue)) {
                            random = Random.Range(rangeX, rangeY);
                            nodeValueDic.Add(v2, random);
                        }
                    }
                }
            }
        }

        CreateNodeValueObj();
    }
    
    // 刷新两节点之间的权值
    private void RefreshNodeValue() {
        var range = gameMeshData.valueRange;
        var rangeX = (int)range.x;
        var rangeY = (int)range.y;
        var list = nodeValueDic.Keys.ToList();
        foreach (var key in list) {
            var random = Random.Range(rangeX, rangeY);
            nodeValueDic[key] = random;
        }
        CreateNodeValueObj();
    }

    private void CreateNodeValueObj() {
        foreach (var nodeValue in nodeValueDic) {
            // 更新两两之间的显示
            var nodePos = nodeValue.Key;
            var randomValue = nodeValue.Value;
            var pos1 = NodeUtil.GetPosByIndex(nodePos.x);
            var pos2 = NodeUtil.GetPosByIndex(nodePos.y);
            var posOffset = (pos1 + pos2) / 2;

            if (txtDic.ContainsKey(nodeValue.Key) && !txtDic[nodeValue.Key].activeSelf) {
                txtDic.Remove(nodeValue.Key);
            }

            if (txtDic.ContainsKey(nodeValue.Key) && txtDic[nodeValue.Key].activeSelf) {
                var nowObj = txtDic[nodeValue.Key];
                nowObj.transform.localPosition = posOffset;
                nowObj.GetComponent<TextMesh>().text = randomValue.ToString();
            } else {
                var obj = ObjectPool.Instance.GetObjBySign(ObjSign.NodeTxt);
                obj.transform.parent = objRoot;
                obj.transform.localPosition = posOffset;
                obj.GetComponent<TextMesh>().text = randomValue.ToString();
                txtDic.Add(nodeValue.Key, obj);
            }
        }
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

        RefreshWallNodePos();
        isFinding = true;
        PathFindingBase pathFinding = null;
        switch (UIFindingPathType) {
            case FindingPathType.BFS:
                pathFinding = new BFS();
                break;
            case FindingPathType.DIJKSTRA:
                pathFinding = new Dijkstra();
                break;
            case FindingPathType.ASTAR:
                break;
            case FindingPathType.JPS:
                break;
        }
        pathFinding?.Init(nodeDic, xMax, yMax);
    }

    public void ResetFind() {
        foreach (var node in nodeDic.Values) {
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

    public void ChangePathHasValue(bool isShow) {
        if (HasNodeValue == isShow) {
            return;
        }

        HasNodeValue = isShow;
        if (isShow) {
            this.RefreshNodeValue();
        } else {
            foreach (var value in txtDic) {
                value.Value.SetActive(false);
            }
        }
        RefreshNodePos();
    }

    private void RefreshNodePos() {
        foreach (var node in nodeDic.Values) {
            node.transform.localPosition = HasNodeValue? NodeUtil.GetPosByXAndY(node.x, node.y) : NodeUtil.GetPosByXAndYNoOffset(node.x, node.y);
        }
    }

    private void RefreshWallNodePos() {
        foreach (var nodeValue in txtDic) {
            var nodePos = nodeValue.Key;
            if (!NodeUtil.CanGoNode(nodeDic[nodePos.x]) || !NodeUtil.CanGoNode(nodeDic[nodePos.y])) {
                nodeValue.Value.SetActive(false);
            }
        }
    }
}