using System;
using CjLib;
using PhysicsConstraints;
using UnityEngine;

public class TestPointWithRotationConstraint : MonoBehaviour {
    public float Beta = 0.02f;
    public Transform Object;
    public Transform Target;

    public Vector3 Gravity = new Vector3(0.0f, -20.0f, 0.0f);
    
    private float mass;
    private float massInv;

    private Matrix3x3 inertiaLs;
    private Matrix3x3 inertiaInvLs;
    private Vector3 rLocal = Vector3.zero; // corner offset
    private Vector3 v = Vector3.zero; // linear velocity
    private Vector3 a = Vector3.zero; // angular velocity
    private void Start() {
        mass = 1.0f;
        massInv = 1.0f / mass;
        
        inertiaLs = Matrix3x3.Identity;
        inertiaInvLs = inertiaLs.Inverted;
        rLocal = 0.5f * Vector3.one; // 右上角，相对坐标偏移

        var t = transform;
        var x1 = t.TransformVector(new Vector3(1.0f, 0.0f, 0.0f));
        Debug.LogError($"X1:{x1}");
        var x2 = t.TransformVector(new Vector3(0.0f, 1.0f, 0.0f));
        Debug.LogError($"X2:{x2}");
        var x3 = t.TransformVector(new Vector3(0.0f, 0.0f, 1.0f));
        Debug.LogError($"X3:{x3}");
        var world2Local = Matrix3x3.FromRows(
            t.TransformVector(new Vector3(1.0f, 0.0f, 0.0f)),
            t.TransformVector(new Vector3(0.0f, 1.0f, 0.0f)),
            t.TransformVector(new Vector3(0.0f, 0.0f, 1.0f)));
        
        var inertiaInvWs = world2Local.Transposed * inertiaInvLs * world2Local;
        Debug.LogError($"Row0:{inertiaInvWs.Row0}");
        Debug.LogError($"Row1:{inertiaInvWs.Row1}");
        Debug.LogError($"Row2:{inertiaInvWs.Row2}");
    }

    private void FixedUpdate() {
        if (null == Object || null == Target) {
            return;
        }

        var dt = Time.fixedDeltaTime;
        var r = Object.rotation * rLocal; // 乘上物体旋转

        var world2Local = Matrix3x3.FromRows(
            Object.TransformVector(new Vector3(1.0f, 0.0f, 0.0f)),
            Object.TransformVector(new Vector3(0.0f, 1.0f, 0.0f)),
            Object.TransformVector(new Vector3(0.0f, 0.0f, 1.0f)));
        

        // gravity
        v += Gravity * dt;
        
        // C=(X+R)-P
        var cPos = (Object.transform.position + r) - Target.transform.position;
        // Cv=v + w x r; 
        var cVel = v + Vector3.Cross(a, r);

        // S
        var s = Matrix3x3.Skew(-r);
        // 有效质量
        var k = massInv * Matrix3x3.Identity + s * inertiaInvLs * s.Inverted;
        var effectiveMass = k.Inverted;
        // lambda
        // J = [E S]， J x V = Vector3.Cross(v, Matrix3x3.Identity) + Vector3.Cross(a, r) = cVel
        var lambda = effectiveMass * (-cVel + (Beta / dt) * cPos);
        
        // velocity 
        // v *= 1/m * lambda
        // a *= I^-1 * S^T * lambda
        v += massInv * lambda;
        a += (inertiaInvLs * s.Inverted * lambda);
        v *= 0.98f; // temp magic
        a *= 0.98f; // temp magic

        // integration
        Object.transform.position += v * dt;
        Object.transform.rotation = QuaternionUtil.Integrate(Object.transform.rotation, a, dt);
    }
}