using UnityEngine;

public class LookAtCameraComponent : MonoBehaviour {
    private Transform mTransform;
    public float limitAngle = 302;
    public bool lockXaxis = false;
    public bool lockYaxis = false;
    public bool lockZaxis = false;
    
    private Vector3 originalLocalEulerAngle;
    private Transform roleTransform;
    private Camera targetCamera;
    private Vector3 lookAtRota;
    private float targetX;
    private float targetY;
    private float targetZ;
    public bool IsBreak = false;
    private bool isInit = false;
    // Use this for initialization
    void Start() {
        mTransform = transform;
        Init();
    }

    public void Init() {
        if (isInit) {
            return;
        }

        isInit = true;
        originalLocalEulerAngle = mTransform.localEulerAngles;
    }

    public void SetRoleAndCamera(Transform setTransform, Camera setCamera) {
        roleTransform = setTransform;
        targetCamera = setCamera;
    }

    private void OnDestroy() {
        mTransform = null;
    }
    private void OnEnable() {
        mTransform = transform;
        Init();
    }
    private void OnDisable() {
        roleTransform = null;
    }
    // Update is called once per frame
    void Update() {
        if (isInit == false) {
            Init();
        }
        
        if (IsBreak || 
            null == targetCamera ||
            null == mTransform) {
            return;
        }
        
        mTransform.LookAt(targetCamera.transform.position);

        // if (null != mRole && null != mRole.MyRoleLogic && mRole.MyRoleLogic.AutoRoleId == GameData.WarCamera.LockRole.MyRoleLogic.AutoRoleId &&
        //     mRole.MyRoleLogic.GetState(RoleLocalState.IsFreeLook) == false) {
        //     if (mTransform.localEulerAngles.y - 360 < limitAngle - 360 || mTransform.localEulerAngles.y > limitAngle) {
        //         mTransform.localEulerAngles = new Vector3(mTransform.localEulerAngles.x, 163, 0);
        //     }
        // }

        lookAtRota = mTransform.localEulerAngles;
        targetX = lookAtRota.x;
        targetY = lookAtRota.y;
        targetZ = lookAtRota.z;
        if (lockXaxis) {
            targetX = originalLocalEulerAngle.x;
        }

        if (lockYaxis) {
            targetY = originalLocalEulerAngle.y;
        }

        if (lockZaxis) {
            targetZ = originalLocalEulerAngle.z;
        }

        mTransform.localEulerAngles = new Vector3(targetX, targetY, targetZ);
    }
}
