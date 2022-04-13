using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFS : PathFindingBase {
    public override void Find() {
        base.Find();
        Debug.LogError("StartBFSFind");
        DFSTravel(startIndex);

        // BFSSearch();
        DelayShowWayParent();
    }

    private void BFSSearch() {
        finalIndex = 0;
        travelList.Clear();
        var queue = new Queue<int>();
        queue.Enqueue(startIndex);
        while (queue.Count > 0 && !isFindingOver) {
            var value = queue.Dequeue();
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
            ReSortAroundList(list, endIndex);
            for (int i = 0; i < list.Count; i++) {
                var nextValue = list[i];
                if (!travelList.Contains(nextValue)) {
                    isAddIndex = true;
                    queue.Enqueue(nextValue);
                    nodeIndexDic[nextValue].ParentNodeIndex = value;
                    nodeIndexDic[nextValue].DelayShowCheckWay(finalIndex);
                }
            }

            if (isAddIndex) {
                finalIndex++;
            }
        }
    }

    /// <summary>
    /// 根据启发式函数来重新排序访问顺序。。
    /// </summary>
    /// <param name="aroundList"></param>
    /// <param name="endIndex"></param>
    /// <returns></returns>
    private void ReSortAroundList(List<int> aroundList, int endIndex) {
        aroundList.Sort((x, y) => {
            var xDistance = GetManhattanDistance(x, endIndex);
            var yDistance = GetManhattanDistance(y, endIndex);
            if (xDistance > yDistance) {
                return 1;
            } else if (xDistance < yDistance) {
                return -1;
            } else {
                return 0;
            }
        });
    }

    private int GetManhattanDistance(int nowIndex, int target) {
        var startPosX = 0;
        var startPosY = 0;
        NodeUtil.GetXAndYByIndex(nowIndex, ref startPosX, ref startPosY);
        var endPosX = 0;
        var endPosY = 0;
        NodeUtil.GetXAndYByIndex(target, ref endPosX, ref endPosY);
        return (Mathf.Abs(startPosX - endPosX) + Mathf.Abs(startPosY - endPosY));
    }

    private void DFSTravel(int index) {
        if (travelList.Contains(index) || isFindingOver) {
            return;
        }

        var aroundList = listTableDic[index];
        var node = nodeIndexDic[index];
        if (aroundList == null || aroundList.Count <= 0) {
            return;
        }

        travelList.Add(index);
        if (index != startIndex) {
            node.DelayShowCheckWay(finalIndex++);
        }

        if (node.nodeType == NodeType.End) {
            isFindingOver = true;
            return;
        }

        ReSortAroundList(aroundList,endIndex);
        for (var i = 0; i < aroundList.Count; i++) {
            var nextIndex = aroundList[i];
            if (!travelList.Contains(nextIndex)) {
                var nextNode = nodeIndexDic[nextIndex];
                nextNode.ParentNodeIndex = index;
                DFSTravel(nextIndex);
            }
        }
    }
}