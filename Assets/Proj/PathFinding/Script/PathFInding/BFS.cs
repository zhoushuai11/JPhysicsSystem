using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFS : PathFindingBase {
    public override void Find() {
        base.Find();
        // DFSTravel(startIndex);

        BFSSearch();
        DelayShowWayParent();
    }

    private void BFSSearch() {
        finalIndex = 0;
        travelList.Clear();
        var openList = new List<int> {
            startIndex
        };
        while (openList.Count > 0 && !isFindingOver) {
            var value = SelectPointInList(openList);
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