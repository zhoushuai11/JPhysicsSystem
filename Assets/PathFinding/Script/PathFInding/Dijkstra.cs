using System.Collections.Generic;
using UnityEngine;

public class Dijkstra : PathFindingBase {
    private Dictionary<int, CostValue> allCost = new Dictionary<int, CostValue>();
    private List<CostValue> travelCostList = new List<CostValue>();
    private List<int> nowWay = new List<int>();

    public class CostValue {
        public int index = 0;
        public int allCost = 0;
        public List<int> wayList = new List<int>();

        public CostValue(int setIndex, int setAllCost, List<int> setList) {
            index = setIndex;
            allCost = setAllCost;
            wayList = setList;
        }

        public void SetNewCost(int setAllCost, List<int> setList) {
            allCost = setAllCost;
            wayList = setList;
        }
    }

    public override void Find() {
        base.Find();
        DijkstraTravel();
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
    /// 采用广度优先搜索的思想，每次更新最低消耗。
    /// 迪杰斯特拉只关心起点到目标点的消耗，是基于贪心策略的。
    /// 每次从距节点消耗最低的点开始
    /// </summary>
    private void DijkstraTravel() {
        finalIndex = 0;
        travelList.Clear();
        var openList = new List<int> {
            startIndex
        };
        travelCostList.Add(new CostValue(startIndex, 0, new List<int> {
            startIndex
        }));
        while (openList.Count > 0 && !isFindingOver) {
            var costValue = GetMinCostIndex();
            var value = costValue.index;
            openList.Remove(value);
            travelCostList.Remove(costValue);
            if (travelList.Contains(value)) {
                continue;
            }

            travelList.Add(value);
            var node = nodeIndexDic[value];
            var list = listTableDic[value];
            if (node.nodeType == NodeType.End) {
                // 结束
                isFindingOver = true;
                return;
            }

            for (var i = 0; i < list.Count; i++) {
                var nextValue = list[i];
                if (!travelList.Contains(nextValue)) {
                    openList.Add(nextValue);
                    nodeIndexDic[nextValue].DelayShowCheckWay(finalIndex);
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
    private CostValue GetMinCostIndex() {
        var cost = -1;
        var index = 0;
        foreach (var node in travelCostList) {
            var nodeCost = node.allCost;
            if (cost == -1 || nodeCost < cost) {
                index = node.index;
                cost = nodeCost;
                return node;
            }
        }

        return null;
    }

    /// <summary>
    /// 获取两个节点之间的 cost
    /// </summary>
    /// <param name="index"></param>
    /// <param name="nextIndex"></param>
    /// <returns></returns>
    private int GetCostByIndex(int index, int nextIndex) {
        foreach (var nodeValue in nodeValueDic) {
            if (index == nodeValue.Key.x && nextIndex == nodeValue.Key.y || 
                index == nodeValue.Key.y && nextIndex == nodeValue.Key.x) {
                return nodeValue.Value;
            }
        }

        Debug.LogError($"Error: {index} {nextIndex}");
        return -1;
    }

    /// <summary>
    /// 更新当前节点周围可环绕节点的 cost 值
    /// </summary>
    /// <param name="index"></param>
    /// <param name="cost"></param>
    private void UpdateNodeCost(int index, int nextIndex, CostValue nowIndexCostValue) {
        var cost = nowIndexCostValue.allCost;
        var wayList = nowIndexCostValue.wayList;
        var newCost = cost + GetCostByIndex(index, nextIndex);
        CostValue costValue;
        if (allCost.ContainsKey(nextIndex)) {
            costValue = allCost[nextIndex];
            if (newCost < costValue.allCost) {
                var list = new List<int>(wayList.ToArray());
                list.Add(nextIndex);
                costValue.SetNewCost(newCost, list);
            }
        } else {
            var list = new List<int>(wayList.ToArray());
            list.Add(nextIndex);
            costValue = new CostValue(nextIndex, newCost, list);
            allCost.Add(nextIndex, costValue);
        }
    }
}