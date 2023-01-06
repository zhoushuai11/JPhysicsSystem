using UnityEngine;

namespace Unity.Mathematics {
    public struct float3 {
        public float x;
        public float y;
        public float z;

        public float3(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static float3 operator +(float3 a, float3 b) {
            return new float3(a.x + b.x, a.y + b.y, a.z + b.z);
        }
    }
}