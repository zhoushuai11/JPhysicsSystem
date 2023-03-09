using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UNetAsset {
    private readonly string bundleName;
    private readonly string assetName;
    private readonly string assetPath;
    private UnityEngine.Object asset;
    private AssetAsyncOperation asyncOperation;

    public UNetAsset(string bundleName, string assetName) {
        this.bundleName = bundleName;
        this.assetName = assetName;
        this.assetPath = bundleName.EndsWith("/") ? StringUtil.Concat(bundleName, assetName) : StringUtil.Concat(bundleName, "/", assetName);
    }

    public T LoadAsset<T>() where T : UnityEngine.Object {
        if (asset != null) {
            return asset as T;
        }
        asset = Resources.Load<T>(assetPath);
        if (asset == null) {
            DebugEx.LogErrorFormat("加载资源失败：{0}", this.assetPath);
        }
        return asset as T;
    }

    public AssetAsyncOperationHandle LoadAssetAsync<T>() where T : UnityEngine.Object {
        if (asyncOperation != null) {
            return asyncOperation.GetAssetAsyncOperationHandle();
        }
        asyncOperation = new AssetAsyncOperation();
        if (asset != null) {
            asyncOperation.Complete(asset);
        } else {
            asset = Resources.Load<T>(assetPath);
            if (asset == null) {
                DebugEx.LogErrorFormat("加载资源失败：{0}", this.assetPath);
            }
            asyncOperation.Complete(asset);
        }
        return asyncOperation.GetAssetAsyncOperationHandle();
    }
    
    public void Unload() {
        if (asyncOperation != null) {
            asyncOperation.Destroy();
            asyncOperation = null;
        }
        if (asset != null && asset is GameObject == false) {
            Resources.UnloadAsset(asset);
        }
        asset = null;
    }
}