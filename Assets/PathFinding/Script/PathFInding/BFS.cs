using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFS : PathFindingBase {
    private List<int> travelList = new List<int>();
    private List<Node> wayList = new List<Node>();
    public override void Find() {
        Debug.LogError("StartBFSFind");
        // DFSTravel(startIndex);
        
        BFSSearch();
        DelayShowWayPoint();
        DelayShowWayParent();
    }

    private void BFSSearch() {
        travelList.Clear();
        wayList.Clear();
        var queue = new Queue<int>();
        queue.Enqueue(startIndex);
        var lastIndex = startIndex;
        while (queue.Count > 0) {
            var value = queue.Dequeue();
            if (travelList.Contains(value)) {
                continue;
            }
            travelList.Add(value);
            var node = nodeIndexDic[value];
            var list = listTableDic[value];
            wayList.Add(node);
            if (node.nodeType == NodeType.End) {
                // 结束
                return;
            }
            for (int i = 0; i < list.Count; i++) {
                var nextValue = list[i];
                if (!travelList.Contains(nextValue)) {
                    queue.Enqueue(nextValue);
                    nodeIndexDic[nextValue].ParentNodeIndex = value;
                }
            }
        }
    }

    private bool isDFSOver = false;
    private void DFSTravel(int index) {
        if (travelList.Contains(index) || isDFSOver) {
            return;
        }
        var aroundList = listTableDic[index];
        var node = nodeIndexDic[index];
        if (aroundList == null || aroundList.Count <= 0) {
            if (wayList.Contains(node)) {
                wayList.Remove(node);
            }
            return;
        }
        Debug.LogError($"{node.x},{node.y} index:{index}");
        travelList.Add(index);
        wayList.Add(node);
        if (node.nodeType == NodeType.End) {
            isDFSOver = true;
            return;
        }

        var wayCount = wayList.Count;
        for (var i = 0; i < aroundList.Count; i++) {
            var nowIndex = aroundList[i];
            DFSTravel(nowIndex);
            if (!isDFSOver) {
                var nowCount = wayList.Count - 1;
                var offset = nowCount - wayCount;
                for (int j = 0; j < offset; j++) {
                    wayList.RemoveAt(nowCount--);
                }
            }
        }
    }

    private void DelayShowWayPoint() {
        for (int i = 0; i < wayList.Count; i++) {
            wayList[i].ChangeNodeType(NodeType.FinalWay);
            wayList[i].SetText(i.ToString());
        }
    }

    private void DelayShowWayParent() {
        var parentIndex = endIndex;
        var m = 0;
        while (parentIndex != -1) {
            var parentNode = nodeIndexDic[parentIndex];
            parentNode.ChangeNodeType(NodeType.FinalWay);
            parentNode.SetText(m++.ToString());
            parentIndex = parentNode.ParentNodeIndex;
        }
    }
}
