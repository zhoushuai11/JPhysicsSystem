using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalAsset {
    public readonly string assetPath;
    private UnityEngine.Object asset;
    private State state = State.None;
    private AssetAsyncOperation asyncOperation;
    private Coroutine asyncCoroutine;
    
    enum State {
        None,
        Loading,
        Success,
        Fail
    }

    public LocalAsset(string assetPath) {
        this.assetPath = assetPath;
    }

    public T LoadAsset<T>() where T : UnityEngine.Object {
        if (state == State.Success) {
            return asset as T;
        }
#if UNITY_EDITOR
        asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(this.assetPath);
#endif
        if (asset == null) {
            DebugEx.LogErrorFormat("加载资源失败：{0}", this.assetPath);
            state = State.Fail;
        } else {
            state = State.Success;
        }
        return asset as T;
    }

    public AssetAsyncOperationHandle LoadAssetAsync<T>() where T : UnityEngine.Object {
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
#if UNITY_EDITOR
        asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(this.assetPath);
#endif
        yield return new WaitForSeconds(0.001f);
        if (asset == null) {
            DebugEx.LogErrorFormat("加载资源失败：{0}", this.assetPath);
            state = State.Fail;
        } else {
            state = State.Success;
        }
        asyncOperation.Complete(asset);
    }

    public void Unload() {
        state = State.None;
        if (asyncOperation != null) {
            asyncOperation.Destroy();
            asyncOperation = null;
        }
        if (asyncCoroutine != null) {
            CoroutineUtil.Instance.StopCoroutine(asyncCoroutine);
            asyncCoroutine = null;
        }
        if (asset != null && asset is GameObject == false) {
            Resources.UnloadAsset(asset);
        }
        asset = null;
    }
}