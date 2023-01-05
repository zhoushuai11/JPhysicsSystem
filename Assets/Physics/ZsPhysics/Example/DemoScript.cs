using System;
using UnityEngine;
using ZsPhysics;

namespace ZsPhysics.Example {
    public class DemoScript : MonoBehaviour {
        private void Awake() {
            ZPhysicsWorld.Init();
        }

        private void FixedUpdate() {
            ZPhysicsWorld.Tick(Time.fixedDeltaTime);
        }
    }
}