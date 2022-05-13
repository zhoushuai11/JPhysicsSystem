using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 图片Mesh 切割，基于 PolygonCollider2D
/// </summary>
public class Mesh2D : MonoBehaviour {
    private PolygonCollider2D pc;
    private SpriteRenderer sr;

    private void Awake() {
        pc = GetComponent<PolygonCollider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 通过 Collider 获取顶点世界坐标
    /// </summary>
    /// <returns></returns>
    public Vector2[] GetVerticesFromPolygonCollider() {
        var vertices = pc.points;
        for (int i = 0; i < vertices.Length; i++) {
            var ver = vertices[i];
            var localScale = transform.localScale;
            ver = new Vector2(ver.x * localScale.x, ver.y * localScale.y);
            var position = transform.position;
            ver = new Vector2(ver.x + position.x, ver.y + position.y);
            vertices[i] = ver;
        }
        return vertices;
    }
}
