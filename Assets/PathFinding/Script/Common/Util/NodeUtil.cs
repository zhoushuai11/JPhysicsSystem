using System.Collections.Generic;
using UnityEngine;

public static class NodeUtil {
    public static int xMax;
    public static int yMax;
    public static float scale;
    public static float offset;

    public static void Init(int x, int y, float s, float o) {
        xMax = x;
        yMax = y;
        scale = s;
        offset = o;
    }

    public static int GetIndexByXAndY(Node node) {
        return GetIndexByXAndY(node.x, node.y);
    }

    public static int GetIndexByXAndY(int x, int y) {
        return x * xMax + y;
    }

    public static void GetXAndYByIndex(int index, ref int x, ref int y) {
        x = index / yMax;
        y = index % yMax;
    }

    public static Vector2 GetPosByXAndY(int x, int y) {
        var xcenter = x - xMax / 2;
        var ycenter = y - yMax / 2;
        var xPos = xcenter * (scale * 1.03f + offset);
        var yPos = ycenter * (scale * 1.03f + offset);
        return new Vector2(yPos, xPos);
    }

    public static Vector2 GetPosByXAndYNoOffset(int x, int y) {
        var xcenter = x - xMax / 2;
        var ycenter = y - yMax / 2;
        var xPos = xcenter * (scale * 1.03f);
        var yPos = ycenter * (scale * 1.03f);
        return new Vector2(yPos, xPos);
    }

    public static Vector2 GetPosByIndex(int index) {
        var x = 0;
        var y = 0;
        GetXAndYByIndex(index, ref x, ref y);
        return GetPosByXAndY(x, y);
    }

    // 获取当前节点周围的可走结点
    public static List<int> GetAroundTableNode(int x, int y) {
        var index = GetIndexByXAndY(x, y);
        var setXMin = 0;
        var setXMax = yMax * xMax - 1;
        var upIndex = index - xMax;
        var downIndex = index + xMax;
        var rightIndex = index + 1;
        var leftIndex = index - 1;

        var list = new List<int>(4);
        if (upIndex >= 0) {
            list.Add(upIndex);
        }

        if (downIndex <= setXMax) {
            list.Add(downIndex);
        }

        if (y % yMax != 0) {
            list.Add(leftIndex);
        }

        if ((y + 1) % yMax != 0) {
            list.Add(rightIndex);
        }

        return list;
    }

    public static bool CanGoNode(Node node) {
        return node.nodeType != NodeType.Wall;
    }
}