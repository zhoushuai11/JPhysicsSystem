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
    
    /// <summary>
    /// 根据启发式函数来重新排序访问顺序。。
    /// </summary>
    /// <param name="aroundList"></param>
    /// <param name="endIndex"></param>
    /// <returns></returns>
    protected void ReSortAroundList(List<int> aroundList, int endIndex) {
        aroundList.Sort((x, y) => {
            var xDistance = GetManhattanDistance(x);
            var yDistance = GetManhattanDistance(y);
            if (xDistance > yDistance) {
                return 1;
            } else if (xDistance < yDistance) {
                return -1;
            } else {
                return 0;
            }
        });
    }

    protected int SelectPointInList(List<int> list) {
        var minCost = GetManhattanDistance(list[0]);
        var nowIndex = list[0];
        foreach (var value in list) {
            var valueCost = GetManhattanDistance(value);
            if (valueCost < minCost) {
                minCost = valueCost;
                nowIndex = value;
            }
        }
        return nowIndex;
    }

    private int GetManhattanDistance(int nowIndex) {
        var startPosX = 0;
        var startPosY = 0;
        NodeUtil.GetXAndYByIndex(nowIndex, ref startPosX, ref startPosY);
        var endPosX = 0;
        var endPosY = 0;
        NodeUtil.GetXAndYByIndex(endIndex, ref endPosX, ref endPosY);
        return (Mathf.Abs(startPosX - endPosX) + Mathf.Abs(startPosY - endPosY));
    }
}