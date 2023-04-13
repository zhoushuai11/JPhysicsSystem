using System.Collections.Generic;

namespace PathFinding.Script.Framework {
    public class GameWorldManager {
        public static GameWorldManager Instance => instance;
        private static GameWorldManager instance;

        private List<GameWorld> gameWorldList;

        public void Init() {
            instance = this;
            gameWorldList = new List<GameWorld>();
            var isStartServer = false; // 是否服务器模式
            if (isStartServer) {
                new GameWorldServerManager().Init(this);
            } else {
                new GameWorldClientManager().Init(this);
            }
        }

        public void Clear() {
            gameWorldList = null;
        }

        public void OnUpdate() {
            
        }

        public void OnLateUpdate() {
            
        }

        public void OnFixedUpdate() {
            
        }

        public GameWorld AddGameWorld() {
            var world = new GameWorld();
            world.Init();
            return world;
        }
    }
}