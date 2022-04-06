using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFS : PathFindingBase {
    private List<int> travelList = new List<int>();
    public override void Find() {
        BFSSearch();
    }

    private void BFSSearch() {
        Debug.LogError("StartFind");
        BFSTravel(startIndex);
    }

    private void BFSTravel(int index) {
        if (travelList.Contains(index)) {
            return;
        }
        var aroundList = listTableDic[index];
        if (aroundList == null || aroundList.Count <= 0) {
            return;
        }
        var node = nodeIndexDic[index];
        Debug.LogError($"{node.x},{node.y} index:{index}");
        travelList.Add(index);

        for (var i = 0; i < aroundList.Count; i++) {
            var nowIndex = aroundList[i];
            BFSTravel(nowIndex);
        }
    }
}
