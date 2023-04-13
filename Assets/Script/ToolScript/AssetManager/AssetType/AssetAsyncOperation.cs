using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetAsyncOperation {
    private Dictionary<ulong, Action<AssetAsyncOperationHandle>> completeActions = new Dictionary<ulong, Action<AssetAsyncOperationHandle>>();
    private static Dictionary<ulong, Action<AssetAsyncOperationHandle>> cacheCompleteActions = new Dictionary<ulong, Action<AssetAsyncOperationHandle>>(8);

    private ulong asyncOperationHandleID = 0;

    public UnityEngine.Object Result { get; private set; }

    public bool IsDone { get; private set; }

    public float Progress { get; private set; } = 0f;

    public void UpdateProgress(float progress) {
        this.Progress = progress;
    }

    public void Complete(UnityEngine.Object result) {
        this.Progress = 1f;
        this.Result = result;
        this.IsDone = true;
        if (completeActions.Count > 0) {
            cacheCompleteActions.Clear();
            foreach (var key in completeActions.Keys) {
                cacheCompleteActions.Add(key, completeActions[key]);
            }
            completeActions.Clear();
            foreach (var item in cacheCompleteActions) {
                item.Value?.Invoke(new AssetAsyncOperationHandle(this, item.Key));
            }
        }
    }

    public void AddListener(ulong id, Action<AssetAsyncOperationHandle> callback) {
        if (!completeActions.ContainsKey(id) || completeActions[id] == null) {
            completeActions[id] = callback;
        } else {
            completeActions[id] += callback;
        }
    }

    public void RemoveListener(ulong id, Action<AssetAsyncOperationHandle> callback) {
        if (callback == null) {
            return;
        }
        if (completeActions.ContainsKey(id) && completeActions[id] != null) {
            completeActions[id] -= callback;
        }
    }

    public void RemoveAllListeners(ulong id) {
        if (completeActions.ContainsKey(id)) {
            completeActions.Remove(id);
        }
    }

    public AssetAsyncOperationHandle GetAssetAsyncOperationHandle() {
        return new AssetAsyncOperationHandle(this, asyncOperationHandleID++);
    }

    public void Destroy() {
        this.Result = null;
        this.IsDone = false;
        this.Progress = 0f;
        completeActions.Clear();
    }
}