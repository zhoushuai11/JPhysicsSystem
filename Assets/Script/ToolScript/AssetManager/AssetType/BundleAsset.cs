using System.Collections;
using System.IO;

using UnityEngine;

public class BundleAsset {
    private AssetBundleRequest assetBundleRequest;
    private AssetAsyncOperation asyncOperation;
    private Coroutine asyncCoroutine;
    private Object asset;
    private State state = State.None;
    public readonly string bundleName;
    public readonly string assetName;

    enum State {
        None,
        Loading,
        Success,
        Fail
    }

    public BundleAsset(string bundleName, string assetName) {
        this.bundleName = bundleName;
        this.assetName = assetName;
    }

    public BundleAsset(string bundleName, string assetName, Object asset) : this(bundleName, assetName) {
        this.asset = asset;
    }

    public T LoadAsset<T>() where T : Object {
        if (state == State.Success) {
            return asset as T;
        }

        if (assetBundleRequest != null) {
            asset = assetBundleRequest.asset;
        }

        if (asset == null) {
            var assetBundle = AssetManager.LoadAssetBundle(bundleName);
            if (assetBundle == null) {
                return null;
            }

            asset = assetBundle.LoadAsset<T>(assetName);
            assetBundleRequest = null;
        }

        if (asset == null) {
            var path = Path.Combine("Assets", bundleName, assetName);
            AssetManager.ReportAssetBundleError(bundleName, $"加载资源失败：{path}");
            state = State.Fail;
        } else {
            state = State.Success;
        }

        return asset as T;
    }

    public AssetAsyncOperationHandle LoadAssetAsync<T>() where T : Object {
        if (asyncOperation == null) {
            asyncOperation = new AssetAsyncOperation();
        }

        if (state == State.Success) {
            asyncOperation.Complete(asset);
            return asyncOperation.GetAssetAsyncOperationHandle();
        }

        if (state == State.Loading) {
            return asyncOperation.GetAssetAsyncOperationHandle();
        }

        state = State.Loading;
        asyncCoroutine = CoroutineUtil.Instance.StartCoroutine(OnLoadAsync<T>());
        return asyncOperation.GetAssetAsyncOperationHandle();
    }

    private IEnumerator OnLoadAsync<T>() where T : Object {
        UpdateProgress(0f);
        var handle = AssetManager.LoadAssetBundleAsync(bundleName);
        while (!handle.IsDone) {
            yield return null;
            UpdateProgress(handle.Progress / 2);
        }

        if (asset == null) {
            if (assetBundleRequest == null) {
                var assetBundle = handle.Acquire<AssetBundle>();
                if (assetBundle == null) {
                    state = State.Fail;
                    asyncOperation?.Complete(null);
                    yield break;
                }

                assetBundleRequest = assetBundle.LoadAssetAsync<T>(assetName);
            }

            while (assetBundleRequest != null && !assetBundleRequest.isDone) {
                yield return null;
                UpdateProgress(0.5f + assetBundleRequest.progress / 2);
            }

            if (assetBundleRequest != null) {
                asset = assetBundleRequest.asset;
                assetBundleRequest = null;
            }
        }

        if (asset == null) {
            var path = Path.Combine("Assets", bundleName, assetName);
            AssetManager.ReportAssetBundleError(bundleName, $"加载资源失败：{path}");
            state = State.Fail;
        } else {
            state = State.Success;
        }

        asyncOperation?.Complete(asset);
    }

    private void UpdateProgress(float progress) {
        if (asyncOperation != null) {
            asyncOperation.UpdateProgress(progress);
        }
    }

    public void Unload() {
        state = State.None;
        if (asyncCoroutine != null) {
            CoroutineUtil.Instance.StopCoroutine(asyncCoroutine);
            asyncCoroutine = null;
        }

        if (asyncOperation != null) {
            asyncOperation.Destroy();
            asyncOperation = null;
        }

        if (asset != null && asset is GameObject == false) {
            Resources.UnloadAsset(asset);
        }

        if (assetBundleRequest != null) {
            if (assetBundleRequest.asset != null && assetBundleRequest.asset is GameObject == false) {
                Resources.UnloadAsset(assetBundleRequest.asset);
            }

            assetBundleRequest = null;
        }

        asset = null;
    }
}