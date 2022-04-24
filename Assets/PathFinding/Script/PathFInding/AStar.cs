using System.Collections.Generic;
using UnityEngine;

public class AStar : PathFindingBase {
    private Dictionary<int, AStartNode> allCost = new Dictionary<int, AStartNode>();
    private Dictionary<int, int> hCost = new Dictionary<int, int>();
    private List<AStartNode> travelCostList = new List<AStartNode>();
    private List<int> nowWay = new List<int>();

    private class AStartNode {
        public int index;
        public int g;
        public int h;
        public List<int> wayList;

        public AStartNode(int setIndex, int setGValue, int setHValue, List<int> setList) {
            index = setIndex;
            g = setGValue;
            h = setHValue;
            wayList = setList;
        }
    }

    public override void Find() {
        AStarFind();
        DelayShowWayParent();
    }

    protected override void DelayShowWayParent() {
        var m = 0;
        foreach (var value in allCost[endIndex].wayList) {
            var node = nodeIndexDic[value];
            node.DelayShowFinalWay(m++, finalIndex);
        }
    }

    /// <summary>
    /// 同时采用启发式函数计算目标点到终点的距离和计算起点到目标点的消耗
    /// 然后选取最优解。
    /// </summary>
    private void AStarFind() {
        finalIndex = 0;
        travelList.Clear();
        travelCostList.Add(new AStartNode(startIndex, 0, GetManhattanDistance(startIndex), new List<int> {
            startIndex
        }));
        while (travelCostList.Count > 0 && !isFindingOver) {
            var costValue = GetMinCostIndex();
            var value = costValue.index;
            travelCostList.Remove(costValue);
            if (travelList.Contains(value)) {
                continue;
            }

            travelList.Add(value);
            var node = nodeIndexDic[value];
            var list = listTableDic[value];
            node.DelayShowCheckWay(finalIndex++);
            if (node.nodeType == NodeType.End) {
                // 结束
                isFindingOver = true;
                return;
            }

            for (var i = 0; i < list.Count; i++) {
                var nextValue = list[i];
                if (!travelList.Contains(nextValue)) {
                    UpdateNodeCost(value, nextValue, costValue);
                    travelCostList.Add(allCost[nextValue]);
                }
            }
        }
    }

    /// <summary>
    /// 获取当前列表里 cost 最低的节点
    /// </summary>
    /// <returns></returns>
    private AStartNode GetMinCostIndex() {
        var costValue = travelCostList[0];
        var cost = costValue.g + costValue.h;
        foreach (var node in travelCostList) {
            var nodeCost = node.g + node.h;
            if (nodeCost < cost) {
                cost = nodeCost;
                costValue = node;
            }
        }

        return costValue;
    }

    /// <summary>
    /// 更新当前节点周围可环绕节点的 cost 值（到终点的曼哈顿距离 + 起点到该节点的消耗）
    /// </summary>
    /// <param name="index"></param>
    /// <param name="cost"></param>
    private void UpdateNodeCost(int index, int nextIndex, AStartNode nowIndexCostValue) {
        var nowCost = nowIndexCostValue.g + nowIndexCostValue.h;
        var wayList = nowIndexCostValue.wayList;
        
        var newGCost = nowIndexCostValue.g + GetCostByIndex(index, nextIndex);
        var newHCost = GetNodeHIndex(nextIndex);
        var newCost = newGCost + newHCost;
        
        AStartNode costValue;
        if (allCost.ContainsKey(nextIndex)) {
            costValue = allCost[nextIndex];
            if (newCost < costValue.g + costValue.h) {
                var list = new List<int>(wayList.ToArray());
                list.Add(nextIndex);
                costValue.g = newGCost;
                costValue.h = newHCost;
                costValue.wayList = list;
            }
        } else {
            var list = new List<int>(wayList.ToArray());
            list.Add(nextIndex);
            costValue = new AStartNode(nextIndex, newGCost, newHCost, list);
            allCost.Add(nextIndex, costValue);
        }
    }

    private int GetNodeHIndex(int index) {
        if (!hCost.ContainsKey(index)) {
            hCost.Add(index, GetManhattanDistance(index));
        }

        Debug.LogError($"{index}  {hCost[index]}");
        return hCost[index];
    }

    /// <summary>
    /// 获取两个节点之间的 cost
    /// </summary>
    /// <param name="index"></param>
    /// <param name="nextIndex"></param>
    /// <returns></returns>
    private int GetCostByIndex(int index, int nextIndex) {
        foreach (var nodeValue in nodeValueDic) {
            if (index == nodeValue.Key.x && nextIndex == nodeValue.Key.y || index == nodeValue.Key.y && nextIndex == nodeValue.Key.x) {
                return nodeValue.Value;
            }
        }

        Debug.LogError($"Error: {index} {nextIndex}");
        return -1;
    }
}