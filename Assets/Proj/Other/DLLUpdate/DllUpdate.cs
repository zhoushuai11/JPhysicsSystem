using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class DllUpdate : MonoBehaviour {
    [MenuItem("Tools/Build Android DLL")]
    public static void BuildAssetsAndroidDll() {
        var fileName = "TestUnityUpdateProj.byte";
        Debug.LogError($"FileIsExit:{File.Exists($"Assets/Dll/{fileName}")}");
        Object mainAsset = AssetDatabase.LoadMainAssetAtPath($"Assets/Dll/{fileName}");
        BuildPipeline.BuildAssetBundle(mainAsset, null, 
            Application.dataPath + "\\Dll\\myassets.android", BuildAssetBundleOptions.None, BuildTarget.Android);
    }

    private void Start() {
        var url = Application.streamingAssetsPath + "/TestUnityUpdateProj.dll";
        var assembly = Assembly.LoadFile(url);
        var typeArray = assembly.GetTypes();
        foreach (var t in typeArray) {
            Debug.Log("Type: " + t.Name);
            var obj = assembly.CreateInstance(t.Name);
            var methods = t.GetMethods();
            foreach (var m in methods) {
                Debug.Log("Method " + m.Name);
                if (m.Name == "GetTestInfo") {
                    var q = m.Invoke(obj, new object[0]);
                    Debug.LogError($"MethodReflection: {q.ToString()}");
                }
            }
        }

        // var nowAssembly = Assembly.GetExecutingAssembly(); // 获取当前程序集
        // Debug.Log("NewNew: " + nowAssembly.FullName);
        // foreach (var t in nowAssembly.GetTypes()) {
        //     Debug.Log("NowAssembly : " + t.Name);
        // }
    }

    // Use this for initialization
    IEnumerator StartLoad() {
        yield return new WaitForSeconds(2);
        var url = Application.streamingAssetsPath + "/myassets.android";

        Debug.Log("url: " + url);
        Debug.Log(File.Exists(url));
        UnityEngine.AssetBundle.LoadFromFile(url);

        WWW www = new WWW(url);

        yield return www;

        if (www.error != null) {
            Debug.Log("加载 出错");
        }

        if (www.isDone) {
            Debug.Log("加载完毕");
            AssetBundle ab = www.assetBundle;

            try {
                //先把DLL以TextAsset类型取出来,在把bytes给Assembly.Load方法读取准备进入反射操作
                Assembly aly = Assembly.Load(((TextAsset)www.assetBundle.mainAsset).bytes);

                //获取DLL下全部的类型
                foreach (var i in aly.GetTypes()) {
                    //调试代码
                    Debug.Log(i.Name);
                    // str += "\r\n" + i.Name;
                    //
                    // //添加组件到当前GameObject下面
                    // Component c = this.gameObject.AddComponent(i);
                    //
                    // //然后类名是MyClass,就把文本引用赋值给MyClass.platefaceText属性.
                    // if (i.Name == "MyClass") 
                    // {
                    //     FieldInfo info = c.GetType().GetField("platefaceText");
                    //     info.SetValue(c, myAgeText);
                    // }
                }
            } catch (Exception e) {
                Debug.Log("加载DLL出错");
                Debug.Log(e.Message);
            }
        }
    }
}