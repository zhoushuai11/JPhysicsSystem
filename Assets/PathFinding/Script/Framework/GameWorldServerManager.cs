namespace PathFinding.Script.Framework {
    public class GameWorldServerManager {
        private GameWorldManager gameWorldManager;
        private GameWorld gameWorld;
        public void Init(GameWorldManager gameWorldManager) {
            this.gameWorldManager = gameWorldManager;
            this.gameWorld = gameWorldManager.AddGameWorld();
        }

        public void Clear() {
            
        }
    }
}