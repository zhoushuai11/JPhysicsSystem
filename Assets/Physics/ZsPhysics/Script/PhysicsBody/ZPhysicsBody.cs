using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZsPhysics {
    public class ZPhysicsBody : MonoBehaviour {
        public Vector3 LinearVelocity = Vector3.zero;
        public Vector3 AngularVelocity = Vector3.zero;
        public float GravityScale = 1;
        public bool IsLockPosition = false;
        public bool IsLockRotation = false;

        private void Start() {
            ZPhysicsWorld.Register(this);
        }

        private void OnDisable() {
            ZPhysicsWorld.UnRegister(this);
        }

        public void Integrate(float dt) {
            if (!IsLockPosition) {
                transform.position = Tool.Integrate(transform.position, LinearVelocity, dt);
            }

            if (!IsLockRotation) {
                transform.rotation = Tool.Integrate(transform.rotation, AngularVelocity, dt);
            }
        }
    }
}