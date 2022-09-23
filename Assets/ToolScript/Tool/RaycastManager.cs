using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastManager {
    // 从摄像机屏幕中间发射射线
    public static Ray CameraShotCenter(Camera camera) {
        var centerX = Screen.width * 0.5f;
        var centerY = Screen.height * 0.5f;
        return camera.ScreenPointToRay(new Vector2(centerX, centerY));
    }
    
    public static int RaycastNonAlloc(Vector3 point, Vector3 dir, RaycastHit[] hits, float distance,
        int layerMask = -5) {
        Array.Clear(hits, 0, hits.Length);
        if (float.IsNaN(point.x) || float.IsNaN(point.y) || float.IsNaN(point.z)) {
            point = Vector3.zero;
        }

        if (float.IsNaN(dir.x) || float.IsNaN(dir.y) || float.IsNaN(dir.z)) {
            dir = Vector3.down;
        }

        if (float.IsNaN(distance)) {
            distance = 0;
        }

        return Physics.RaycastNonAlloc(point, dir, hits, distance, layerMask);
    }
}