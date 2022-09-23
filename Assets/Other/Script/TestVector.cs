using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TestVector {
    public static Vector3 CustomLocalToWorld(this Transform transform, Transform parent) {
        // 缩放矩阵
        var scaleMatrix = CustomMatrix4X4(
            new Vector4(parent.localScale.x, 0, 0, 0), 
            new Vector4(0, parent.localScale.y, 0, 0), 
            new Vector4(0, 0, parent.localScale.z, 0), 
            new Vector4(0, 0, 0, 1));
        // 旋转矩阵
        var eulerAngles = parent.eulerAngles;
        var rotZ = CustomMatrix4X4(
            new Vector4(Mathf.Cos(eulerAngles.z * Mathf.PI / 180), -Mathf.Sin(eulerAngles.z * Mathf.PI / 180), 0, 0),
            new Vector4(Mathf.Sin(eulerAngles.z * Mathf.PI / 180), Mathf.Cos(eulerAngles.z * Mathf.PI / 180), 0, 0),
            new Vector4(0, 0, 1, 0),
            new Vector4(0, 0, 0, 1));
 
        var rotX = CustomMatrix4X4(
            new Vector4(1, 0, 0, 0),
            new Vector4(0, Mathf.Cos(eulerAngles.x * Mathf.PI / 180), -Mathf.Sin(eulerAngles.x * Mathf.PI / 180), 0),
            new Vector4(0, Mathf.Sin(eulerAngles.x * Mathf.PI / 180), Mathf.Cos(eulerAngles.x * Mathf.PI / 180), 0),
            new Vector4(0, 0, 0, 1));
 
        var rotY = CustomMatrix4X4(
                new Vector4(Mathf.Cos(eulerAngles.y * Mathf.PI / 180), 0, Mathf.Sin(eulerAngles.y * Mathf.PI / 180), 0),
                new Vector4(0, 1, 0, 0),
                new Vector4(-Mathf.Sin(eulerAngles.y * Mathf.PI / 180), 0, Mathf.Cos(eulerAngles.y * Mathf.PI / 180), 0),
                new Vector4(0, 0, 0, 1));
        // 位移矩阵
        var moveMatrix = CustomMatrix4X4(
            new Vector4(1, 0, 0, parent.position.x), 
            new Vector4(0, 1, 0, parent.position.y), 
            new Vector4(0, 0, 1, parent.position.z), 
            new Vector4(0,0,0, 1));
        
        // 获取变换矩阵
        var mLocalToWorld = (CustomMatrix4X4(
                new Vector4(1, 0, 0, 0),
                new Vector4(0, 1, 0, 0),
                new Vector4(0, 0, 1, 0),
                new Vector4(0, 0, 0, 1)
            )) * moveMatrix * rotY * rotX * rotZ * scaleMatrix;
        
        PrintMatrix(mLocalToWorld);
        PrintMatrix(parent.localToWorldMatrix);
        PrintMatrix(Matrix4x4.TRS(parent.position, parent.rotation, parent.localScale));
        var pLocal = new Vector4(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z, 1);
        var pWorld = parent.localToWorldMatrix * pLocal;
        Debug.LogError($"Out: {pWorld}");
        // transform.position = pLocal;
        return pWorld;
    }

    public static Vector3 CustomWorldToLocal(this Transform transform, Transform local) {
        // 位移矩阵，注意是反向位移
        var moveMatrix = CustomMatrix4X4(
            new Vector4(1, 0, 0, -local.localPosition.x), 
            new Vector4(0, 1, 0, -local.localPosition.y), 
            new Vector4(0, 0, 1, -local.localPosition.z), 
            new Vector4(0,0,0, 1));
        // 缩放矩阵，注意是要缩小
        var scaleMatrix = CustomMatrix4X4(
            new Vector4(1 / local.localScale.x, 0, 0, 0), 
            new Vector4(0, 1 / local.localScale.y, 0, 0), 
            new Vector4(0, 0, 1 / local.localScale.z, 0),
            new Vector4(0, 0, 0, 1));
        // 旋转矩阵，注意要加负号
        var eulerAngles = -local.eulerAngles;
        var rotZ = CustomMatrix4X4(
            new Vector4(Mathf.Cos(eulerAngles.z * Mathf.PI / 180), -Mathf.Sin(eulerAngles.z * Mathf.PI / 180), 0, 0),
            new Vector4(Mathf.Sin(eulerAngles.z * Mathf.PI / 180), Mathf.Cos(eulerAngles.z * Mathf.PI / 180), 0, 0),
            new Vector4(0, 0, 1, 0),
            new Vector4(0, 0, 0, 1));
 
        var rotX = CustomMatrix4X4(
            new Vector4(1, 0, 0, 0),
            new Vector4(0, Mathf.Cos(eulerAngles.x * Mathf.PI / 180), -Mathf.Sin(eulerAngles.x * Mathf.PI / 180), 0),
            new Vector4(0, Mathf.Sin(eulerAngles.x * Mathf.PI / 180), Mathf.Cos(eulerAngles.x * Mathf.PI / 180), 0),
            new Vector4(0, 0, 0, 1));
 
        var rotY = CustomMatrix4X4(
                new Vector4(Mathf.Cos(eulerAngles.y * Mathf.PI / 180), 0, Mathf.Sin(eulerAngles.y * Mathf.PI / 180), 0),
                new Vector4(0, 1, 0, 0),
                new Vector4(-Mathf.Sin(eulerAngles.y * Mathf.PI / 180), 0, Mathf.Cos(eulerAngles.y * Mathf.PI / 180), 0),
                new Vector4(0, 0, 0, 1));
        
        // 获取变换矩阵
        var mWorldToLocal = (CustomMatrix4X4(
                new Vector4(1, 0, 0, 0),
                new Vector4(0, 1, 0, 0),
                new Vector4(0, 0, 1, 0),
                new Vector4(0, 0, 0, 1)
            )) * scaleMatrix * rotZ * rotX * rotY * moveMatrix;
        
        PrintMatrix(mWorldToLocal);
        PrintMatrix(local.worldToLocalMatrix);
        var pWorld = new Vector4(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z, 1);
        var pLocal = local.worldToLocalMatrix * pWorld;
        Debug.LogError($"Out: {pLocal}");
        return pLocal;
    }

    public static void PrintMatrix(Matrix4x4 m) {
        Debug.Log(m.m00 + "  " + m.m01 + "  " + m.m02 + "  " + m.m03);
        Debug.Log(m.m10 + "  " + m.m11 + "  " + m.m12 + "  " + m.m13);
        Debug.Log(m.m20 + "  " + m.m21 + "  " + m.m22 + "  " + m.m23);
        Debug.Log(m.m30 + "  " + m.m31 + "  " + m.m32 + "  " + m.m33);
        Debug.Log("==============================");
    }

    /// <summary>
    /// 按照顺序构建矩阵
    /// </summary>
    /// <returns></returns>
    public static Matrix4x4 CustomMatrix4X4(Vector4 column0, Vector4 column1, Vector4 column2, Vector4 column3) {
        var matrix = new Matrix4x4();
        matrix.m00 = column0.x;
        matrix.m01 = column0.y;
        matrix.m02 = column0.z;
        matrix.m03 = column0.w;
        matrix.m10 = column1.x;
        matrix.m11 = column1.y;
        matrix.m12 = column1.z;
        matrix.m13 = column1.w;
        matrix.m20 = column2.x;
        matrix.m21 = column2.y;
        matrix.m22 = column2.z;
        matrix.m23 = column2.w;
        matrix.m30 = column3.x;
        matrix.m31 = column3.y;
        matrix.m32 = column3.z;
        matrix.m33 = column3.w;
        return matrix;
    }
}