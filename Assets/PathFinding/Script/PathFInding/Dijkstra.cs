using System.Collections.Generic;

public class Dijkstra : PathFindingBase {
    private Dictionary<int, int> allCost = new Dictionary<int, int>();
    private List<int> nowWay = new List<int>();

    public class CostValue {
        public int cost = 0;
        public List<int> wayList = new List<int>();
    }

    public override void Find() {
        base.Find();
        DijkstraTravel();
        DelayShowWayParent();
    }

    /// <summary>
    /// 获取当前列表里 cost 最低的节点
    /// </summary>
    /// <returns></returns>
    private int GetMinCostIndex() {
        
    }

    /// <summary>
    /// 更新当前节点周围可环绕节点的 cost 值
    /// </summary>
    /// <param name="index"></param>
    /// <param name="cost"></param>
    private void UpdateNodeCost(int index, int cost) {
        if (!allCost.ContainsKey(index)) {
            allCost.Add(index, cost);
        }
    }

    /// <summary>
    /// 采用广度优先搜索的思想，每次更新最低消耗。
    /// 迪杰斯特拉只关心起点到目标点的消耗，是基于贪心策略的。
    /// </summary>
    private void DijkstraTravel() {
        finalIndex = 0;
        travelList.Clear();
        var openList = new List<int> {
            startIndex
        };
        while (openList.Count > 0 && !isFindingOver) {
            var value = openList[0];
            openList.Remove(value);
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

            var isAddIndex = false;
            for (int i = 0; i < list.Count; i++) {
                var nextValue = list[i];
                if (!travelList.Contains(nextValue)) {
                    isAddIndex = true;
                    // queue.Enqueue(nextValue);
                    openList.Add(nextValue);
                    nodeIndexDic[nextValue].ParentNodeIndex = value;
                    nodeIndexDic[nextValue].DelayShowCheckWay(finalIndex);
                }
            }

            if (isAddIndex) {
                finalIndex++;
            }
        }
    }
}