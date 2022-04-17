using System.Collections.Generic;

public class Dijkstra : PathFindingBase {
    public override void Find() {
        base.Find();
        DijkstraTravel();
        DelayShowWayParent();
    }

    private Dictionary<int, int> allCost = new Dictionary<int, int>();
    private List<int> nowWay = new List<int>();
    /// <summary>
    /// 采用广度优先搜索的思想，每次更新最低消耗
    /// </summary>
    private void DijkstraTravel() {
        finalIndex = 0;
        travelList.Clear();
        var openList = new List<int> {
            startIndex
        };
        while (openList.Count > 0 && !isFindingOver) {
            var value = SelectPointInList(openList);
            // var value = openList[0];
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