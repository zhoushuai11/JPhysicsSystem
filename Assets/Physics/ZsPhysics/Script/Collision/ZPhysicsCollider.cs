using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZsPhysics {
    [RequireComponent(typeof(ZPhysicsBody))]
    public class ZPhysicsCollider : MonoBehaviour {
        public enum ShapeType {
            Sphere,
            Plane,
            Custom
        }

        private void OnEnable() {
            ZPhysicsWorld.Register(this);
        }

        private void OnDisable() {
            ZPhysicsWorld.UnRegister(this);
        }
    }
}