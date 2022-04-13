using System.Collections.Generic;
using UnityEngine;

public class ObjectPool {
    private static ObjectPool instance;

    public static ObjectPool Instance {
        get {
            if (null == instance) {
                instance = new ObjectPool();
            }

            return instance;
        }
    }

    private Dictionary<ObjSign, List<GameObject>> objDic = new Dictionary<ObjSign, List<GameObject>>();
    private Dictionary<ObjSign, Object> objPrefab = new Dictionary<ObjSign, Object>();

    public void Init(SOGameMeshData gameMeshData) {
        objPrefab.Add(ObjSign.Node, gameMeshData.defaultObj);
        objPrefab.Add(ObjSign.NodeTxt, gameMeshData.txtObj);
    }

    public GameObject GetObjBySign(ObjSign sign) {
        if (!objDic.ContainsKey(sign)) {
            objDic.Add(sign, new List<GameObject>());
        }

        foreach (var obj in objDic[sign]) {
            if (!obj.activeSelf) {
                obj.SetActive(true);
                return obj;
            }
        }

        var newObj = (GameObject)GameObject.Instantiate(objPrefab[sign]);
        objDic[sign].Add(newObj);
        return newObj;
    }
}

public enum ObjSign {
    Node,
    NodeTxt
}