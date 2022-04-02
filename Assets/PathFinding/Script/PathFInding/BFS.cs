using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFS : IPathFinding {
    private List<Node> nodeList;
    
    public void Init(List<Node> nodeList) {
        this.nodeList = nodeList;
    }

    public void Find() {
        throw new System.NotImplementedException();
    }
}
