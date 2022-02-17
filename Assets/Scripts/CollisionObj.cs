using J2P;
using UnityEngine;

namespace DefaultNamespace {
    public class CollisionObj : MonoBehaviour,IQuadTreeItem {
        public Vector2 size { get; }
        public Vector2 center { get; }
        public Rect rect { get; }
        public Collider2D selfCollider { get; }
        public PositionInQuadTree lastPosInQuadTree { get; set; }
        public PositionInQuadTree currentPosInQuadTree { get; set; }
    }
}