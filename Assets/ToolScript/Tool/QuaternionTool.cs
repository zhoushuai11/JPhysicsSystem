using System;
using UnityEngine;

public class QuaternionTool {
    public static Quaternion ToQuaternion(Vector3 v) {
        return ToQuaternion(v.y, v.x, v.z);
    }

    public static Quaternion ToQuaternion(float yaw, float pitch, float roll) {
        yaw *= Mathf.Deg2Rad;
        pitch *= Mathf.Deg2Rad;
        roll *= Mathf.Deg2Rad;
        var rollOver2 = roll * 0.5f;
        var sinRollOver2 = (float)Math.Sin(rollOver2);
        var cosRollOver2 = (float)Math.Cos(rollOver2);
        var pitchOver2 = pitch * 0.5f;
        var sinPitchOver2 = (float)Math.Sin(pitchOver2);
        var cosPitchOver2 = (float)Math.Cos(pitchOver2);
        var yawOver2 = yaw * 0.5f;
        var sinYawOver2 = (float)Math.Sin(yawOver2);
        var cosYawOver2 = (float)Math.Cos(yawOver2);
        Quaternion result;
        result.w = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
        result.x = cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2;
        result.y = sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2;
        result.z = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;

        return result;
    }

    public static Vector3 ToEuler(Quaternion q1) {
        var sqw = q1.w * q1.w;
        var sqx = q1.x * q1.x;
        var sqy = q1.y * q1.y;
        var sqz = q1.z * q1.z;
        var unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
        var test = q1.x * q1.w - q1.y * q1.z;
        Vector3 v;

        if (test > 0.4995f * unit) {
            // singularity at north pole
            v.y = 2f * Mathf.Atan2(q1.y, q1.x);
            v.x = Mathf.PI / 2;
            v.z = 0;
            return NormalizeAngles(v * Mathf.Rad2Deg);
        }

        if (test < -0.4995f * unit) {
            // singularity at south pole
            v.y = -2f * Mathf.Atan2(q1.y, q1.x);
            v.x = -Mathf.PI / 2;
            v.z = 0;
            return NormalizeAngles(v * Mathf.Rad2Deg);
        }

        var q = new Quaternion(q1.w, q1.z, q1.x, q1.y);
        v.y = (float)Math.Atan2(2f * q.x * q.w + 2f * q.y * q.z, 1 - 2f * (q.z * q.z + q.w * q.w)); // Yaw
        v.x = (float)Math.Asin(2f * (q.x * q.z - q.w * q.y)); // Pitch
        v.z = (float)Math.Atan2(2f * q.x * q.y + 2f * q.z * q.w, 1 - 2f * (q.y * q.y + q.z * q.z)); // Roll
        return NormalizeAngles(v * Mathf.Rad2Deg);
    }

    static Vector3 NormalizeAngles(Vector3 angles) {
        angles.x = NormalizeAngle(angles.x);
        angles.y = NormalizeAngle(angles.y);
        angles.z = NormalizeAngle(angles.z);
        return angles;
    }

    static float NormalizeAngle(float angle) {
        while (angle > 360)
            angle -= 360;
        while (angle < 0)
            angle += 360;
        return angle;
    } // 将欧拉角转为弧度后计算完成旋转后的四元数

    public Quaternion FromEulerRad(Vector3 euler, string order = "ZYX") {
        euler *= Mathf.Deg2Rad;
        var _x = euler.x * 0.5; // theta θ
        var _y = euler.y * 0.5; // psi ψ
        var _z = euler.z * 0.5; // phi φ

        float cX = (float)Math.Cos(_x);
        float cY = (float)Math.Cos(_y);
        float cZ = (float)Math.Cos(_z);

        float sX = (float)Math.Sin(_x);
        float sY = (float)Math.Sin(_y);
        float sZ = (float)Math.Sin(_z);

        return new Quaternion(sX * cY * cZ + cX * sY * sZ, cX * sY * cZ - sX * cY * sZ, cX * cY * sZ - sX * sY * cZ, cX * cY * cZ + sX * sY * sZ);
        // if (order == "ZXY") {
        //     return new Quaternion(
        //         cX * cY * cZ - sX * sY * sZ,
        //         sX * cY * cZ - cX * sY * sZ,
        //         cX * sY * cZ + sX * cY * sZ,
        //         cX * cY * sZ + sX * sY * cZ
        //         );
        // }

        // if (order == "XYZ") {
        //     return new Quaternion(
        //         cX * cY * cZ - sX * sY * sZ,
        //         sX * cY * cZ + cX * sY * sZ,
        //         cX * sY * cZ - sX * cY * sZ,
        //         cX * cY * sZ + sX * sY * cZ);
        // }

        // if (order == "YXZ") {
        //     return new Quaternion(
        //         cX * cY * cZ + sX * sY * sZ,
        //         sX * cY * cZ + cX * sY * sZ,
        //         cX * sY * cZ - sX * cY * sZ,
        //         cX * cY * sZ - sX * sY * cZ);
        // }

        // if (order == "ZYX") {
        // return new Quaternion(
        //     sX * cY * cZ + cX * sY * sZ, 
        //     cX * sY * cZ - sX * cY * sZ, 
        //     cX * cY * sZ - sX * sY * cZ, 
        //     cX * cY * cZ + sX * sY * sZ); 
        // }

        // if (order == "YZX") {
        //     return new Quaternion(
        //         cX * cY * cZ - sX * sY * sZ,
        //         sX * cY * cZ + cX * sY * sZ,
        //         cX * sY * cZ + sX * cY * sZ,
        //         cX * cY * sZ - sX * sY * cZ);
        // }

        // if (order == "XZY") {
        //     return new Quaternion(
        //         cX * cY * cZ + sX * sY * sZ,
        //         sX * cY * cZ - cX * sY * sZ,
        //         cX * sY * cZ - sX * cY * sZ,
        //         cX * cY * sZ + sX * sY * cZ);
        // }
        // return new Quaternion(0,0,0,0);
    }
}