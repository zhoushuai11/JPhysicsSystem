using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZsPhysics {
    [RequireComponent(typeof(ZPhysicsBody))]
    public class ZPhysicsWorld {
        private static Vector3 Gravity = new Vector3(0, -9.8f, 0);
        private static List<ZPhysicsBody> rigidBodyList;
        private static List<ZPhysicsCollider> colliderList;

        public static void Init() {
            rigidBodyList = new List<ZPhysicsBody>();
            colliderList = new List<ZPhysicsCollider>();
        }

        public static void Tick(float dt) {
            // gravity
            var gravityImpulse = Gravity * dt;
            foreach (var body in rigidBodyList) {
                body.LinearVelocity += gravityImpulse * body.GravityScale;
            }

            // pos and rota
            foreach (var body in rigidBodyList) {
                body.Integrate(dt);
            }
        }

        public static void Register(ZPhysicsCollider b) {
            colliderList.Add(b);
        }

        public static void UnRegister(ZPhysicsCollider b) {
            colliderList.Remove(b);
        }

        public static void Register(ZPhysicsBody b) {
            rigidBodyList.Add(b);
        }

        public static void UnRegister(ZPhysicsBody b) {
            rigidBodyList.Remove(b);
        }
    }
}