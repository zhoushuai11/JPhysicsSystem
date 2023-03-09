using System.Drawing;
using System.Runtime.InteropServices;
using UnityEngine;

public class LimitMouse : MonoBehaviour {
    [DllImport("User32")]
    private static extern bool SetCursorPos(int x, int y);

    [DllImport("User32")]
    private static extern bool GetCursorPos(out Point pt);

    private float defaultScreenX;
    private float defaultScreenY;

    private float inputRadius = 120;

    private bool isShowCircle;
    private Point lastPoint; // 上一帧鼠标位置
    private Point nowPoint; // 当前鼠标位置
    private Point mouseCenterPoint; // 限制范围中心（鼠标点锁定的位置 全屏）
    private Vector3 mouseTouchPoint; // Unity 界面，鼠标的位置


    void Start() {
        defaultScreenX = Screen.width; // 获取当前屏幕长宽
        defaultScreenY = Screen.height;
        DebugEx.LogError(SOWepEquipDataConfig.Get("AngledForegrip").SkinSign);
    }

    void Update() {
        // 获取当前鼠标坐标
        GetCursorPos(out nowPoint);
        if (isShowCircle) {
            // 判断是否在规定的圆形内，如果超出了，就把鼠标限制在上一帧的数据位置上
            var centerV2 = new Vector2(mouseCenterPoint.X, mouseCenterPoint.Y);
            var nowV2 = new Vector2(nowPoint.X, nowPoint.Y);
            if (Vector2.Distance(centerV2, nowV2) > inputRadius) {
                // 超出边界
                SetCursorPos(lastPoint.X, lastPoint.Y);
            } else {
                // 更新上一帧鼠标位置
                lastPoint = nowPoint;
            }
        }
        
        // 增加退出条件，摁下 ESC
        if (Input.GetKey(KeyCode.Escape)) {
            isShowCircle = false;
        }
    }

    private void OnGUI() {
        if (GUI.Button(new Rect(0.5f * defaultScreenX - 50, 0.5f * defaultScreenY - 30, 100, 50), "锁定！")) {
            mouseTouchPoint = Input.mousePosition;
            GetCursorPos(out mouseCenterPoint);
            isShowCircle = true;
        }

        if (GUI.Button(new Rect(0.5f * defaultScreenX - 50, 0.5f * defaultScreenY + 30, 100, 50), "取消锁定！")) {
            isShowCircle = false;
        }
        inputRadius = GUI.HorizontalSlider(new Rect(0.5f * defaultScreenX - 50, 0.5f * defaultScreenY + 100, 100, 30), inputRadius, 100, 1000f);
        
        GUI.Label(new Rect(0.5f * defaultScreenX - 50, 0.5f * defaultScreenY + 80,100, 50), isShowCircle? $"锁定状态：{inputRadius}" : "未锁定");

        if (isShowCircle) {
            // 这里模拟一下圆的生成，相当于画了很多小方块组成圆的形状
            var segments = Mathf.FloorToInt(4 * inputRadius); // 这里稍微设置下精度
            var vertexs = GetVerticesFromCenter(mouseTouchPoint, segments, inputRadius);
            var sizeValue = Mathf.Clamp01(0.2f + inputRadius / 100); // 每个方块的大小
            var size = new Vector2(sizeValue, sizeValue);
            for (var i = 0; i < vertexs.Length; i++) {
                var point = vertexs[i];
                GUI.Box(new Rect(point, size), "");
            }
        }
    }
    
    /// <summary>
    ///  根据圆心获取获取所有点的数组
    /// </summary>
    /// <param name="center">中心点</param>
    /// <param name="segments">精度，多少个点也就是</param>
    /// <param name="radius">半径</param>
    /// <returns></returns>
    private static Vector3[] GetVerticesFromCenter(Vector3 center, int segments, float radius) {
        var points = new Vector3[segments];
        //每一份的角度
        var angle = Mathf.Deg2Rad * 360f / segments;

        for (var i = 0; i < segments; i++) {
            var inX = center.x + radius * Mathf.Sin(angle * i);
            var inY = center.y + radius * Mathf.Cos(angle * i);
            points[i] = new Vector3(inX, inY, center.z);
        }

        return points;
    }
}