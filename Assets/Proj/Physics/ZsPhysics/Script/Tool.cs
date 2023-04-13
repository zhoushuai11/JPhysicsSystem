using UnityEngine;

namespace ZsPhysics {
    public static class Tool {
        public static Vector3 Integrate(Vector3 x, Vector3 v, float dt) {
            return x + v * dt;
        }

        public static Quaternion Integrate(Quaternion q, Vector3 omega, float dt) {
            dt *= 0.5f;
            return Normalize(new Quaternion(omega.x * dt, omega.y * dt, omega.z * dt, 1.0f) * q);
        }

        public static Quaternion Normalize(Quaternion q) {
            float magInv = 1.0f / Magnitude(q);
            return new Quaternion(magInv * q.x, magInv * q.y, magInv * q.z, magInv * q.w);
        }

        public static float Magnitude(Quaternion q) {
            return Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
        }
    }
}