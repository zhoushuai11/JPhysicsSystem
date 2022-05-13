using UnityEngine;
/// <summary>
/// 切割模型
/// 1、屏幕点击划线（LineRenderer）
/// 2、物品根据划线切割
/// </summary>
public class MeshSplit : MonoBehaviour {
    private LineRenderer lr;
    private Vector3[] lrPoints = new Vector3[2];
    private bool isBtnDown = false;
    private Vector3 touchStartPos;
    
    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            OnMouseBtnDown();
        }
        if (Input.GetMouseButton(0)) {
            OnMouseBtnStay();
        }

        if (Input.GetMouseButtonUp(0)) {
            OnMouseBtnUp();
            // gameObject.GetComponent<MeshFilter>().mesh.bone
        }
    }

    private void OnMouseBtnDown() {
        isBtnDown = true;
        var mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        var newMousePos = new Vector2(mousePos.x, mousePos.y);
        touchStartPos = mousePos;
        lrPoints[0] = mousePos;
    }

    private void OnMouseBtnStay() {
        if (!isBtnDown) {
            return;
        }

        var mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        var newMousePos = new Vector2(mousePos.x, mousePos.y);
        lrPoints[1] = mousePos;
        DrawLine();
    }

    private void OnMouseBtnUp() {
        if (!isBtnDown) {
            return;
        }
        isBtnDown = false;
        Check2DCollider();
    }
    
    /// <summary>
    /// 计算绘制直线方程 y = k * x + b
    /// </summary>
    private void CalcLineFunction() {
        var startPoint = lrPoints[0];
        var endPoint = lrPoints[1];
    }

    /// <summary>
    /// 检测当前划线碰到的物体
    /// </summary>
    private void Check2DCollider() {
        
        var all2DColliders = FindObjectOfType<PolygonCollider2D>();
        
    }
    
    private void DrawLine() {
        if (null == lr) {
            var obj = new GameObject("LineRenderObj");
            obj.transform.position = Vector3.back;
            lr = obj.AddComponent<LineRenderer>();
            return;
        }

        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.useWorldSpace = false;
        lr.positionCount = lrPoints.Length;
        lr.SetPositions(lrPoints);
    }

    private class LineFunction {
        public float k;
        public float b;

        public LineFunction(Vector3 startPoint, Vector3 endPoint) {
            k = CalcK(startPoint, endPoint);
            b = CalcB(startPoint, k);
        }

        /// <summary>
        /// 检测对应线段是否与此线段相交
        /// </summary>
        /// <param name="Line">Line[2]</param>
        /// <returns></returns>
        public bool CalcIsTouch(Line[] lines) {
            foreach (var line in lines) {
                var lineK = CalcK(line.startPoint, line.endPoint);
                var lineB = CalcB(line.startPoint, lineK);
                if (Mathf.Abs(lineK - this.k) <= 0.01f) {
                    if (Mathf.Abs(lineB - this.b) <= 0.01f) {
                        return true;
                    }
                } else {
                    
                }
            }
            return false;
        }

        private float CalcK(Vector3 startPoint, Vector3 endPoint) {
            var k = 0.0f;
            if (startPoint.x == endPoint.x) {
                k = 0;
            } else {
                k = (startPoint.y - endPoint.y) / (startPoint.x - endPoint.x);
            }

            return k;
        }

        private float CalcB(Vector3 startPoint,float k) {
            var b = startPoint.y - k * startPoint.x;
            return b;
        }
    }

    private struct Line {
        public Vector3 startPoint;
        public Vector3 endPoint;
    }
}