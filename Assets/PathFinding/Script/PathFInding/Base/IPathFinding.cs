using System.Collections.Generic;
using UnityEngine;

public interface IPathFinding {
    public void Init(Dictionary<int, Node> nodeDic, int xMax, int yMax);
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

    public void Init(Dictionary<int, Node> nodeDic, int xMax, int yMax) {
        this.xMax = xMax;
        this.yMax = yMax;
        nodeIndexDic = nodeDic;

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
            listTableDic.Add(NodeUtil.GetIndexByXAndY(node), GetAroundTableNode(node));
            if (node.nodeType == NodeType.Start) {
                startNode = node;
                startIndex = NodeUtil.GetIndexByXAndY(startNode);
            } else if (node.nodeType == NodeType.End) {
                endNode = node;
                endIndex = NodeUtil.GetIndexByXAndY(endNode);
            }
        }
    }

    // 获取当前节点周围的可走结点
    private List<int> GetAroundTableNode(Node node) {
        var aroundList = node.NodeValue.AroundList.ToArray();
        var list = new List<int>(aroundList.Length);
        foreach (var around in aroundList) {
            var nextNode = nodeIndexDic[around];
            if (NodeUtil.CanGoNode(nextNode)) {
                list.Add(around);
            }
        }

        return list;
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