
using System.Collections.Generic;
using UnityEngine;

public interface IPathFinding {
    public void Init(Dictionary<Node, GameObject> nodeDic, int xMax, int yMax);
    public void Find();
}

public enum FindingPathType {
    BFS,
    DIJKSTRA,
    ASTAR,
    JPS
}

public class PathFindingBase : IPathFinding {
    protected Dictionary<int, List<int>> listTableDic = new Dictionary<int, List<int>>();
    protected Dictionary<int, Node> nodeIndexDic = new Dictionary<int, Node>();

    protected int xMax = 0;
    protected int yMax = 0;

    protected Node startNode;
    protected int startIndex;
    protected Node endNode;
    protected int endIndex;

    protected int finalIndex = 0;
    protected bool isFindingOver = false;
    protected List<int> travelList = new List<int>();

    public void Init(Dictionary<Node, GameObject> nodeDic, int xMax, int yMax) {
        this.xMax = xMax;
        this.yMax = yMax;
        foreach (var key in nodeDic.Keys) {
            nodeIndexDic.Add(GetIndexByXAndY(key), key);
        }
        CreatePathTable();
        Find();
    }

    // 根据所给节点数据构建图，此处使用邻接表
    private void CreatePathTable() {
        if (null == nodeIndexDic || nodeIndexDic.Count <= 0) {
            return;
        }

        // 初始化字典
        foreach (var node in nodeIndexDic.Values) {
            listTableDic.Add(GetIndexByXAndY(node), GetAroundTableNode(node.x, node.y));
            if (node.nodeType == NodeType.Start) {
                startNode = node;
                startIndex = GetIndexByXAndY(startNode);
            }else if (node.nodeType == NodeType.End) {
                endNode = node;
                endIndex = GetIndexByXAndY(endNode);
            }
        }
    }

    // 获取当前节点周围的可走结点
    private List<int> GetAroundTableNode(int x, int y) {
        var index = GetIndexByXAndY(x, y);
        var setXMin = 0;
        var setXMax = yMax * xMax - 1;
        var upIndex = index - xMax;
        var downIndex = index + xMax;
        var rightIndex = index + 1;
        var leftIndex = index - 1;

        var list = new List<int>(4);
        if (upIndex >= 0 && CanGoNode(upIndex)) {
            list.Add(upIndex);
        }

        if (downIndex <= setXMax && CanGoNode(downIndex)) {
            list.Add(downIndex);
        }

        if (y % yMax != 0 && CanGoNode(leftIndex)) {
            list.Add(leftIndex);
        }

        if ((y + 1) % yMax != 0 && CanGoNode(rightIndex)) {
            list.Add(rightIndex);
        }
        return list;
    }

    private int GetIndexByXAndY(Node node) {
        return GetIndexByXAndY(node.x, node.y);
    }
    
    private int GetIndexByXAndY(int x, int y) {
        return x * xMax + y;
    }

    private bool CanGoNode(int index) {
        return nodeIndexDic.ContainsKey(index) && (nodeIndexDic[index].nodeType != NodeType.Wall);
    }

    public virtual void Find() {
        
    }

    protected void DelayShowWayParent() {
        var parentIndex = endIndex;
        var m = 0;
        while (parentIndex != -1) {
            var parentNode = nodeIndexDic[parentIndex];
            parentNode.DelayShowFinalWay(m++, finalIndex);
            parentIndex = parentNode.ParentNodeIndex;
        }
    }
}