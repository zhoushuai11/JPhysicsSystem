using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public struct AssetAsyncOperationHandle {
    private AssetAsyncOperation assetAsyncOperation;
    private ulong id;

    public event Action<AssetAsyncOperationHandle> Completed {
        add {
            if (IsDone) {
                value?.Invoke(this);
                return;
            }
            assetAsyncOperation?.AddListener(id, value);
        }
        remove {
            assetAsyncOperation?.RemoveListener(id, value);
        }
    }

    public bool IsDone {
        get {
            return assetAsyncOperation != null && assetAsyncOperation.IsDone;
        }
    }

    public float Progress {
        get {
            if (assetAsyncOperation == null) {
                return 0f;
            }
            return assetAsyncOperation.Progress;
        }
    }

    public bool IsValid() {
        return assetAsyncOperation != null;
    }

    public AssetAsyncOperationHandle(AssetAsyncOperation assetAsyncOperation, ulong id) {
        this.assetAsyncOperation = assetAsyncOperation;
        this.id = id;
    }

    public T Acquire<T>() where T : UnityEngine.Object {
        return assetAsyncOperation?.Result as T;
    }

    public void Release() {
        if (assetAsyncOperation != null) {
            assetAsyncOperation.RemoveAllListeners(id);
            assetAsyncOperation = null;
        }
        id = 0;
    }
}