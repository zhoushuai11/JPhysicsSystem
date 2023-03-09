using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class AssetBundleAsset {
    private readonly string assetPath;
    private readonly string relativePath;
    private readonly string bundleName;
    private readonly ushort[] dependencies;
    private AssetBundle assetBundle;
    private AssetAsyncOperation asyncOperation;
    private AssetBundleCreateRequest assetBundleCreateRequest;
    private Coroutine asyncCoroutine;
    private State state = State.None;
    private ulong offset = 0;
    private bool isLoadFromPersistentDataPath = true;

    enum State {
        None,
        Loading,
        Success,
        Fail
    }
    
#if !CLOSE_TESTTOOL
    public static bool loadRecordEnable = false;
    public static Dictionary<string, string> loadTimeRecordDic;
    private DateTime startTime;
#endif

    public AssetBundleAsset(string bundleName, ushort[] dependencies) : base() {
        this.bundleName = Path.ChangeExtension(bundleName, ".unity3d");
        this.dependencies = dependencies;
        relativePath = Path.Combine("AssetBundle", this.bundleName);
        assetPath = Path.Combine(URI.persistentDataPath, relativePath);
        isLoadFromPersistentDataPath = true;
        if (!File.Exists(assetPath)) {
            isLoadFromPersistentDataPath = false;
#if BUILD_SERVER || !UNITY_ANDROID
            assetPath = Path.Combine(URI.streamingAssetsPath, "AssetBundle", this.bundleName);
#else
            if (PlayAssetDeliveryManager.Exists(Path.Combine("AssetBundle", this.bundleName), out var location)) {
                assetPath = location.Path;
                offset = location.Offset;
            } else {
                assetPath = Path.Combine(URI.streamingAssetsPath, "AssetBundle", this.bundleName);
            }
#endif
        }
    }

    public AssetBundle LoadAssetBundle(bool includeDependence) {
        if (state == State.Success) {
            return assetBundle;
        }

        if (includeDependence && dependencies != null) {
            foreach (var dependence in dependencies) {
                AssetManager.LoadDependenceAssetBundle(dependence);
            }
        }


        if (assetBundleCreateRequest != null) {
            assetBundle = assetBundleCreateRequest.assetBundle;
        }

        if (assetBundle == null) {
            try {
                SetBeginLoadBundleTime();
                assetBundle = AssetBundle.LoadFromFile(assetPath, 0, offset);
                SetEndLoadBundleTime();
                assetBundleCreateRequest = null;
            } catch (Exception e) {
            }
        }

        if (assetBundle == null) {
            ReportAssetBundleError($"同步加载 AssetBundle 失败: {bundleName}出现错误");
            state = State.Fail;
        } else {
            state = State.Success;
        }

        return assetBundle;
    }

    public AssetAsyncOperationHandle LoadAssetBundleAsync(bool includeDependence) {
        if (asyncOperation == null) {
            asyncOperation = new AssetAsyncOperation();
        }

        if (state == State.Success) {
            asyncOperation.Complete(assetBundle);
            return asyncOperation.GetAssetAsyncOperationHandle();
        }

        if (state == State.Loading) {
            return asyncOperation.GetAssetAsyncOperationHandle();
        }
        
// #if !BUILD_SERVER
//         VersionManager.Instance.CheckSensitiveAssetValid(relativePath);
// #endif

        state = State.Loading;
        asyncCoroutine = CoroutineUtil.Instance.StartCoroutine(OnLoadAssetBundleAsync(includeDependence));
        return asyncOperation.GetAssetAsyncOperationHandle();
    }

    private IEnumerator OnLoadAssetBundleAsync(bool includeDependence) {
        UpdateProgress(0f);
        var singleTaskProgress = 1f;
        if (includeDependence && dependencies != null) {
            var handles = new List<AssetAsyncOperationHandle>(dependencies.Length);
            for (int i = 0; i < dependencies.Length; i++) {
                var dependence = dependencies[i];
                var handle = AssetManager.LoadDependenceAssetBundleAsync(dependence);
                handles.Add(handle);
            }

            var totalCount = dependencies.Length;
            var completedCount = 0;
            singleTaskProgress = (float)1 / (totalCount + 1);
            while (completedCount < totalCount) {
                yield return null;
                completedCount = 0;
                var progress = 0f;
                for (int i = 0; i < handles.Count; i++) {
                    if (handles[i].IsDone) {
                        completedCount++;
                    }

                    progress += handles[i].Progress;
                }

                UpdateProgress(progress / (totalCount + 1));
            }

            handles.Clear();
        }

        if (assetBundle == null) {
            try {
                if (assetBundleCreateRequest == null) {
                    assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(assetPath, 0, offset);
                }
            } catch (Exception e) {
            }

            while (assetBundleCreateRequest != null && !assetBundleCreateRequest.isDone) {
                UpdateProgress(assetBundleCreateRequest.progress * singleTaskProgress + 1 - singleTaskProgress);
                yield return null;
            }

            if (assetBundleCreateRequest != null) {
                assetBundle = assetBundleCreateRequest.assetBundle;
                assetBundleCreateRequest = null;
            }
        }
        
        if (assetBundle == null) {
            state = State.Fail;
            ReportAssetBundleError($"异步加载 AssetBundle 失败: {bundleName}出现错误");
        } else {
            state = State.Success;
        }

        asyncOperation?.Complete(assetBundle);
    }

    private void UpdateProgress(float progress) {
        if (asyncOperation != null) {
            asyncOperation.UpdateProgress(progress);
        }
    }

    public void Unload(bool unloadAll) {
        state = State.None;
        if (asyncCoroutine != null) {
            CoroutineUtil.Instance.StopCoroutine(asyncCoroutine);
            asyncCoroutine = null;
        }

        if (assetBundleCreateRequest != null) {
            assetBundleCreateRequest.assetBundle.Unload(true);
            assetBundleCreateRequest = null;
        }

        if (assetBundle != null) {
            assetBundle.Unload(unloadAll);
            assetBundle = null;
        }

        if (asyncOperation != null) {
            asyncOperation.Destroy();
            asyncOperation = null;
        }
    }

    private void SetBeginLoadBundleTime()
    {
#if !CLOSE_TESTTOOL
        if (GlobalDefine.IsDevelopment) {
            if (loadRecordEnable) {
                if (loadTimeRecordDic == null) {
                    loadTimeRecordDic = new Dictionary<string, string>();
                }
                startTime = System.DateTime.Now;
            }
        }
#endif
    }

    private void SetEndLoadBundleTime()
    {
#if !CLOSE_TESTTOOL
        if (GlobalDefine.IsDevelopment) {
            if (loadRecordEnable && ReferenceEquals(assetBundle, null) == false) {
                var endTime = System.DateTime.Now;
                var startSpan = new System.TimeSpan(startTime.Ticks);
                var endSpan = new System.TimeSpan(endTime.Ticks);
                var span = startSpan.Subtract(endSpan).Duration();
                if (loadTimeRecordDic.ContainsKey(assetPath) == false) {
                    loadTimeRecordDic[assetPath] = $"{startTime:O};{endTime:O};{span:G}";
                }
            }
        }
#endif
    }

    public void ReportAssetBundleError(string logInfo) {
        if (!isLoadFromPersistentDataPath || !File.Exists(assetPath)) {
            DebugEx.LogErrorFormat(logInfo);
            return;
        }
        
        var md5 = Funny.IO.SFFile.GetMD5HashFromFile(assetPath);
        var log = string.Concat(logInfo, " | MD5: ", md5);
        DebugEx.LogErrorFormat(log);
    }
}