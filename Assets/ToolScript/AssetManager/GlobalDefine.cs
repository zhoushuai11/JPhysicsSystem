using UnityEngine;

public static class GlobalDefine {
    public static bool IsBuildServer = false;
    public static bool IsDevelopment = false;
    
    public static bool IsUseBundle = false;

    public static bool IsAndroidMobile {
        get {
            if (Application.isMobilePlatform && Application.platform == RuntimePlatform.Android) {
                return true;
            } else {
                return false;
            }
        }
    }
}