using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class SAT {
    public static bool CalcCollision(Rect rectA, Rect rectB) {
        var shapeA = new Shape(rectA);
        var shapeB = new Shape(rectB);
        return SatTestHalf(shapeA, shapeB) && SatTestHalf(shapeB, shapeA);
    }

    static bool SatTestHalf(Shape shapeA, Shape shapeB) {
        // 计算shape本身在边那一侧
        var side = GetWitchSide(shapeA.Get(0), shapeA.Get(1), shapeA.Get(2));
        var n = shapeA.Count;
        for (var i = 0; i < n; ++i) {
            var edge = new Edge {
                start = shapeA.Get(i), 
                end = shapeA.Get((i + 1) % n),
            };

            if (IsInDifferentSide(edge, side, shapeB)) {
                return false;
            }
        }

        return true;
    }

    /// 判读shape是否在edge的另一侧
    static bool IsInDifferentSide(Edge edge, int side, Shape shape) {
        foreach (var vertex in shape.vertices) {
            var s = GetWitchSide(edge.start, edge.end, vertex);
            if (s * side > 0) {
                // 在同一侧，也就是说edge无法将shape分离开
                return false;
            }
        }
        var dor = edge.end - edge.start;
        Debug.DrawLine(edge.start - dor * 2, edge.end + dor * 2, Color.white);
        return true;
    }

    /// 判断点c在ab的哪一侧。二维向量叉乘
    public static int GetWitchSide(Vector2 a, Vector2 b, Vector2 c) {
        var ab = b - a;
        var ac = c - a;
        var cross = ab.x * ac.y - ab.y * ac.x;
        return cross > 0? 1 : (cross < 0? -1 : 0);
    }
}

public class Edge {
    public Vector2 start;
    public Vector2 end;
}

public class Shape {
    public List<Vector2> vertices = new List<Vector2>();

    public int Count => vertices.Count;

    public Shape(Rect rect) {
        this.vertices = new List<Vector2> {
            new Vector2(rect.xMin, rect.yMin), 
            new Vector2(rect.xMin, rect.yMax), 
            new Vector2(rect.xMax, rect.yMax), 
            new Vector2(rect.xMax, rect.yMin),
        };
    }
    
    public Vector2 Get(int index) {
        return vertices[index];
    }

    // 多边形是否包含点
    public bool Contains(Vector2 point) {
        var n = vertices.Count;
        if (n < 3) {
            return false;
        }

        // 先计算出内部的方向
        var innerSide = SAT.GetWitchSide(vertices[0], vertices[1], vertices[2]);

        // 通过判断点是否均在三条边的内侧，来判定单形体是否包含点
        for (var i = 0; i < n; ++i) {
            var iNext = (i + 1) % n;
            var side = SAT.GetWitchSide(vertices[i], vertices[iNext], point);

            if (side * innerSide < 0) // 在外部
            {
                return false;
            }
        }

        return true;
    }
}