using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {
    public int x;
    public int y;
    public NodeType nodeType;
    public int ParentNodeIndex { get; set; } = -1;
    
    public NodeValue NodeValue { get; set; }

    private SpriteRenderer render;
    private TextMesh textMesh;
    private Material[] mats;
    private const float TIMEOFFSET = 0.1f;

    public void Init(int setX, int setY, NodeType setNodeType, Material[] mats) {
        x = setX;
        y = setY;
        nodeType = setNodeType;
        render = transform.GetComponent<SpriteRenderer>();
        textMesh = transform.GetComponentInChildren<TextMesh>();
        this.mats = mats;
        SetText("");
        NodeValue = new NodeValue();
    }

    public void ChangeNodeType(NodeType nt) {
        if (nodeType == nt) {
            return;
        }

        nodeType = nt;
        render.material = mats[(int)nt];
        if (nt != NodeType.FinalWay) {
            SetText("");
        }

        if (nt == NodeType.Default) {
            ParentNodeIndex = -1;
        }
    }

    public void SetText(string t) {
        textMesh.text = t;
    }

    private bool isStartDelayShowPoint = false;
    private float delayShowPointTime = 0.0f;
    private bool isStartDelayShowFinalWay = false;
    private int finalPointIndex = 0;
    private float delayShowFinal = 0.0f;
    private float allWayCountTime = 0.0f;
    /// <summary>
    /// 演示搜索路径
    /// </summary>
    /// <param name="index">第几步</param>
    public void DelayShowCheckWay(int index) {
        delayShowPointTime = TIMEOFFSET * index;
        isStartDelayShowPoint = true;
    }

    /// <summary>
    /// 演示最终路径
    /// </summary>
    /// <param name="index">第几步</param>
    /// <param name="allWayPointCount">一共几部</param>
    public void DelayShowFinalWay(int index, int allWayPointCount) {
        allWayCountTime = allWayPointCount * TIMEOFFSET;
        finalPointIndex = index;
        delayShowFinal = index * TIMEOFFSET;
        isStartDelayShowFinalWay = true;
    }

    private void Update() {
        if (!isStartDelayShowPoint && !isStartDelayShowFinalWay) {
            return;
        }

        if (isStartDelayShowPoint) {
            delayShowPointTime -= Time.deltaTime;
            if (delayShowPointTime <= 0) {
                ChangeNodeType(NodeType.Checked);
                isStartDelayShowPoint = false;
            }
        }

        if (isStartDelayShowFinalWay) {
            if (allWayCountTime > 0) {
                allWayCountTime -= Time.deltaTime;
            } else {
                delayShowFinal -= Time.deltaTime;
                if (delayShowFinal < 0) {
                    isStartDelayShowFinalWay = false;
                    ChangeNodeType(NodeType.FinalWay);
                    SetText(finalPointIndex.ToString());
                }
            }
        }
    }
}

public class NodeValue {
    public List<int> AroundList = new List<int>(3);
    public Dictionary<int, int> ValueDic = new Dictionary<int, int>(3);

    public void AddAroundNode(List<int> aroundList) {
        AroundList = aroundList;
    }

    public void AddAroundNodeValue(int index, int random) {
        if (!AroundList.Contains(index) || ValueDic.ContainsKey(index)) {
            return;
        }
        ValueDic.Add(index, random);
    }
}

public struct NodePos {
    public int x;
    public int y;

    public NodePos(int x, int y) {
        this.x = x;
        this.y = y;
    }
}

public enum NodeType {
    Default,
    Start,
    End,
    Wall,
    Checked,
    FinalWay,
}