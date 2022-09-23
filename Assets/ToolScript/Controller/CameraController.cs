using System;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public Transform Player;

    public float MouseSensitivity = 7f;
    public float RotationSmoothness = 0.05f;

    private SmoothRotation rotationXClass;
    private SmoothRotation rotationYClass;

    private float minVerticalAngle = -90f;

    private float maxVerticalAngle = 90f;

    private const string RotateX = "Mouse X";
    private const string RotateY = "Mouse Y";
    private float nowRotationX => Input.GetAxisRaw(RotateX) * MouseSensitivity;
    private float nowRotationY => Input.GetAxisRaw(RotateY) * MouseSensitivity;

    private void Start() {
        rotationXClass = new SmoothRotation(nowRotationX);
        rotationYClass = new SmoothRotation(nowRotationY);
    }

    private void Update() {
        var rotationX = rotationXClass.UpdateAngle(nowRotationX, RotationSmoothness);
        var rotationY = rotationYClass.UpdateAngle(nowRotationY, RotationSmoothness);
        var clampedY = RestrictVerticalRotation(rotationY);
        rotationYClass.SetAngle(clampedY);
        var worldUp = Player.InverseTransformDirection(Vector3.up);
        var rotation = Player.rotation * Quaternion.AngleAxis(rotationX, worldUp) * Quaternion.AngleAxis(clampedY, Vector3.left);
        // transform.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
        Player.rotation = rotation;
    }

    private float RestrictVerticalRotation(float mouseY) {
        var currentAngle = NormalizeAngle(Player.eulerAngles.x);
        var minY = minVerticalAngle + currentAngle;
        var maxY = maxVerticalAngle + currentAngle;
        return Mathf.Clamp(mouseY, minY + 0.01f, maxY - 0.01f);
    }
    
    private static float NormalizeAngle(float angleDegrees) {
        while (angleDegrees > 180f) {
            angleDegrees -= 360f;
        }

        while (angleDegrees <= -180f) {
            angleDegrees += 360f;
        }

        return angleDegrees;
    }

    private class SmoothRotation {
        private float current;
        private float currentVelocity;

        public SmoothRotation(float startAngle) {
            current = startAngle;
        }

        public float UpdateAngle(float target, float smoothTime) {
            var angle = Mathf.SmoothDampAngle(current, target, ref currentVelocity, smoothTime);
            current = angle;
            return current;
        }

        public void SetAngle(float yAngle) {
            current = yAngle;
        }
    }
}