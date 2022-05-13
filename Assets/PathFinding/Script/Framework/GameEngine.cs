using System;
using UnityEngine;

namespace PathFinding.Script.Framework {
    public class GameEngine : MonoBehaviour {
        private GameWorldManager gameWorldManager;
        private void Start() {
            gameWorldManager = new GameWorldManager();
            gameWorldManager.Init();
        }

        private void OnDestroy() {
            gameWorldManager.Clear();
            gameWorldManager = null;
        }

        private void Update() {
            gameWorldManager.OnUpdate();
        }

        private void LateUpdate() {
            gameWorldManager.OnLateUpdate();
        }

        private void FixedUpdate() {
            gameWorldManager.OnFixedUpdate();
        }
    }
}