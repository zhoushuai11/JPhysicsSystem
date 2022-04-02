using System.Collections.Generic;
using J2P;
using UnityEngine;

namespace QuadTree {
    public class QuadTree {
        private Rect worldRect;
        private int maxDepth;
        private Vector2[] gridSizes; //Store the grid size for each depth，index is depth
        private QuadTreeNode rootNode;
        private List<IQuadTreeItem> _cacheItemsFound = new List<IQuadTreeItem>();
        private Queue<QuadTreeNode> _traverseNodeQueue = new Queue<QuadTreeNode>();

        public QuadTree(Rect worldRect, int maxDepth) {
            this.worldRect = worldRect;
            this.maxDepth = maxDepth;
            this.gridSizes = new Vector2[maxDepth + 1];
            for (int i = 0; i <= maxDepth; i++) {
                var width = worldRect.width / (Mathf.Pow(2, i));
                var height = worldRect.height / (Mathf.Pow(2, i));
                gridSizes[i] = new Vector2(width, height);
            }

            rootNode = new QuadTreeNode(worldRect, 0, maxDepth);
        }

        /// <summary>
        /// 根据所给宽高获得深度
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public int GetDepth(Vector2 size) {
            for (int i = gridSizes.Length - 1; i >= 0; i--) {
                if (size.x <= gridSizes[i].x && size.y <= gridSizes[i].y) {
                    return i;
                }
            }

            Debug.LogError("Size is bigger than QuadTree Max Range");
            return -1;
        }

        public void UpdateItem(IQuadTreeItem item) {
            var newPosInfo = item.currentPosInQuadTree;
            GetPosInfo(item.size, item.center, ref newPosInfo);
            if (newPosInfo.Equals(item.lastPosInQuadTree)) {
                return;
            }

            var currentParent = rootNode;
            if (item.lastPosInQuadTree.posInDepths != null) {
                for (int i = 0; i < item.lastPosInQuadTree.storeDepth; i++) {
                    var currentDepthPosInfo = item.lastPosInQuadTree.posInDepths[i];
                    currentParent.totalItemsCount -= 1;
                    if (i == item.lastPosInQuadTree.storeDepth - 1) {
                        currentParent.childNodes[currentDepthPosInfo.rowIndex, currentDepthPosInfo.columnIndex].RemoveItem(item);
                    } else {
                        currentParent = currentParent.childNodes[currentDepthPosInfo.rowIndex, currentDepthPosInfo.columnIndex];
                    }
                }
            }

            Debug.Log("Remove item in:" + item.lastPosInQuadTree);
            Debug.Log("Add item in:" + newPosInfo);

            //item.currentPosInQuadTree.Copy( newPosInfo );
            var lastPos = item.lastPosInQuadTree;
            lastPos.Copy(newPosInfo);
            item.lastPosInQuadTree = lastPos;

            currentParent = rootNode;
            if (item.lastPosInQuadTree.inRoot) {
                rootNode.AddItem(item);
                return;
            }

            for (int i = 0; i < newPosInfo.storeDepth; i++) {
                var currentDepthPosInfo = newPosInfo.posInDepths[i];
                currentParent.totalItemsCount += 1;
                if (i == newPosInfo.storeDepth - 1) {
                    currentParent.childNodes[currentDepthPosInfo.rowIndex, currentDepthPosInfo.columnIndex].AddItem(item);
                } else {
                    currentParent = currentParent.childNodes[currentDepthPosInfo.rowIndex, currentDepthPosInfo.columnIndex];
                }
            }
        }

        private void GetPosInfo(Vector2 size, Vector2 center, ref PositionInQuadTree posInfo) {
            posInfo.Reset();

            var depth = GetDepth(size); // 获取当前深度
            if (depth == 0) {
                posInfo.inRoot = true;
                return;
            }

            var gridsize = gridSizes[depth]; // 根据深度获取行列数据
            int tempRow = Mathf.FloorToInt((center.y - worldRect.yMin) / gridsize.y);
            int tempColumn = Mathf.FloorToInt((center.x - worldRect.xMin) / gridsize.x);
            var storeDepth = 0;
            posInfo.storeDepth = depth;
            // 更新其他深度下的行列
            for (int i = depth - 1; i >= 0; i--) {
                int div = (int)Mathf.Pow(2, i);
                int rowIndex = tempRow / div;
                if (rowIndex > 1) {
                    rowIndex = 1;
                }

                int columnIndex = tempColumn / div;
                if (columnIndex > 1) {
                    columnIndex = 1;
                }

                posInfo.posInDepths[storeDepth].rowIndex = rowIndex;
                posInfo.posInDepths[storeDepth].columnIndex = columnIndex;

                tempRow %= div;
                tempColumn %= div;
                storeDepth++;
            }
        }
    }
}