using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;

public class AssetManager {
    public enum AssetType {
        ScriptableObject,
        Prefab,
        JPG,
        Material,
        SpriteAtlas,
        Sprite,
        Shader,
        Png,
        AnimatorController,
        Font,
    }

    public static readonly Dictionary<string, string[]> ASSET_CATEGORY = new Dictionary<string, string[]>() {
        {
            "AssetBundle", new[] {
                ".unity3d",
                ".txt"
            }
        }, {
            "Scripts", new[] {
                ".lua",
                ".bytes"
            }
        }, {
            "Txt", new[] {
                ".txt"
            }
        }, {
            "Audio", new[] {
                "bnk",
                ".wem"
            }
        }, {
            "Video", new[] {
                ".mp4"
            }
        }
    };

    public static int LocalVersionCode { get; private set; }
    private static bool inited = false;

    private static readonly List<string> bundleNames = new List<string>();
    private static readonly Dictionary<string, ushort> bundleNameMap = new Dictionary<string, ushort>();
    private static readonly Dictionary<ushort, ushort[]> dependencies = new Dictionary<ushort, ushort[]>();

    private static readonly Dictionary<string, AssetBundleAsset> assetBundleAssets = new Dictionary<string, AssetBundleAsset>();
    private static readonly Dictionary<string, Dictionary<string, BundleAsset>> assets = new Dictionary<string, Dictionary<string, BundleAsset>>();
    private static readonly Dictionary<string, Dictionary<string, LocalAsset>> localAssets = new Dictionary<string, Dictionary<string, LocalAsset>>();
    private static readonly Dictionary<string, Dictionary<string, UNetAsset>> uNetAssets = new Dictionary<string, Dictionary<string, UNetAsset>>();
    private static readonly Dictionary<string, string> bundleNamePool = new Dictionary<string, string>(256);

    public static void Initialize() {
        if (inited) {
            return;
        }

        if (GlobalDefine.IsUseBundle) {
            CreatePersistentDirectories();
            InitializeDependencies();
            try {
                var tempString = ReadAllText("Txt/VersionCode");
                if (!string.IsNullOrEmpty(tempString)) {
                    LocalVersionCode = int.Parse(tempString);
                }
            } catch (Exception e) {
                LocalVersionCode = 0;
            }
        }

        inited = true;
    }

    public static void ReInitialize() {
        if (GlobalDefine.IsUseBundle) {
            InitializeDependencies();
        }

        inited = true;
    }

    private static void CreatePersistentDirectories() {
        foreach (var item in ASSET_CATEGORY) {
            var directory = Path.Combine(URI.persistentDataPath, item.Key);
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
        }
    }

    private static void InitializeDependencies() {
        dependencies.Clear();
        bundleNames.Clear();
        bundleNameMap.Clear();
        var assetCategories = new[] {
            "Main",
            "CombatIsland",
            "RainbowIsland",
            "SupernovaStar",
            "SausageClub",
            "StarfishIsland"
        };
        ushort categoryStartIndex = 0;
        foreach (var category in assetCategories) {
            string[] lines = null;
            var relativePath = $"AssetBundle/{category}.txt";
            var path = Path.Combine(URI.persistentDataPath, relativePath);
            if (File.Exists(path)) {
                lines = File.ReadAllLines(path);
            } else {
                path = Path.Combine(URI.streamingAssetsPath, relativePath);
                if (GlobalDefine.IsAndroidMobile) {
                    var www = new WWW(path);
                    while (!www.isDone) {
                    }

                    lines = www.text.Split(new[] {
                        "\r\n",
                        "\n"
                    }, StringSplitOptions.RemoveEmptyEntries);
                    www.Dispose();
                } else {
                    lines = File.ReadAllLines(path);
                }
            }

            var splits = lines[0].Split(new[] {
                "|"
            }, StringSplitOptions.RemoveEmptyEntries);
            bundleNames.AddRange(splits);
            for (var i = 1; i < lines.Length; i++) {
                var lineSplit = lines[i].Split('\t');
                var indexStrings = lineSplit[1].Split(new[] {
                    "|"
                }, StringSplitOptions.RemoveEmptyEntries);
                var indexes = new ushort[indexStrings.Length];
                for (var j = 0; j < indexes.Length; j++) {
                    var index = ushort.Parse(indexStrings[j]);
                    index += categoryStartIndex;
                    indexes[j] = index;
                }

                var key = ushort.Parse(lineSplit[0]);
                key += categoryStartIndex;
                dependencies[key] = indexes;
            }

            categoryStartIndex = (ushort)bundleNames.Count;
        }

        for (ushort i = 0; i < bundleNames.Count; i++) {
            bundleNameMap[bundleNames[i]] = i;
        }
    }

    public static AssetAsyncOperationHandle LoadAssetAsync<T>(string bundleName, string assetName, AssetType assetType) where T : UnityEngine.Object {
#if UNITY_EDITOR
        if (!GlobalDefine.IsUseBundle) {
            var localAsset = GetLocalAsset(bundleName, assetName, assetType);
            return localAsset.LoadAssetAsync<T>();
        }
#endif
        if (GlobalDefine.IsBuildServer) {
            var uNetAsset = GetUNetAsset(bundleName, assetName);
            return uNetAsset.LoadAssetAsync<T>();
        }

        AdjustBundleName(ref bundleName);
        var bundleAsset = GetBundleAsset(bundleName, assetName);
        return bundleAsset.LoadAssetAsync<T>();
    }

    public static T LoadAsset<T>(string bundleName, string assetName, AssetType assetType) where T : UnityEngine.Object {
        var profilerInfo = assetName;
        profilerInfo = StringUtil.Concat("LoadAsset:", bundleName, "/", assetName);
#if UNITY_EDITOR
        if (!GlobalDefine.IsUseBundle) {
            var localAsset = GetLocalAsset(bundleName, assetName, assetType);
            var asset = localAsset.LoadAsset<T>();
            return asset;
        }
#endif
        if (GlobalDefine.IsBuildServer) {
            var uNetAsset = GetUNetAsset(bundleName, assetName);
            var asset = uNetAsset.LoadAsset<T>();
            return asset;
        }

        AdjustBundleName(ref bundleName);
        var bundleAsset = GetBundleAsset(bundleName, assetName);
        var bAsset = bundleAsset.LoadAsset<T>();
        return bAsset;
    }

    public static void UnloadAsset(string bundleName, string assetName) {
        if (assetName == null) {
            return;
        }

        LocalAsset localAsset;
        if (localAssets.ContainsKey(bundleName) && localAssets[bundleName].TryGetValue(assetName, out localAsset)) {
            localAsset.Unload();
        }

        UNetAsset uNetAsset;
        if (uNetAssets.ContainsKey(bundleName) && uNetAssets[bundleName].TryGetValue(assetName, out uNetAsset)) {
            uNetAsset.Unload();
        }

        AdjustBundleName(ref bundleName);
        if (!assets.ContainsKey(bundleName)) {
            return;
        }

        BundleAsset bundleAsset;
        if (assets.ContainsKey(bundleName) && assets[bundleName].TryGetValue(assetName, out bundleAsset)) {
            bundleAsset.Unload();
        }
    }

    public static UnityEngine.Object[] LoadAllAssets(string bundleName) {
        AdjustBundleName(ref bundleName);
        var assetBundle = LoadAssetBundle(bundleName);
        if (assetBundle != null) {
            var objects = assetBundle.LoadAllAssets();
            foreach (var asset in objects) {
                if (!assets.ContainsKey(bundleName)) {
                    assets[bundleName] = new Dictionary<string, BundleAsset>();
                }

                if (!assets[bundleName].ContainsKey(asset.name)) {
                    assets[bundleName][asset.name] = new BundleAsset(bundleName, asset.name, asset);
                }
            }

            return objects;
        }

        return null;
    }

    public static bool ExitAssetBundle(string bundleName) {
        AdjustBundleName(ref bundleName);
        var filePath = Path.Combine(URI.persistentDataPath, "AssetBundle", bundleName);
        return File.Exists(filePath);
    }

    public static void ReportAssetBundleError(string bundleName, string logInfo) {
        var assetBundleAsset = GetAssetBundleAsset(bundleName);
        assetBundleAsset.ReportAssetBundleError(logInfo);
    }

    public static AssetBundle LoadAssetBundle(string name) {
        var assetBundleAsset = GetAssetBundleAsset(name);
        return assetBundleAsset.LoadAssetBundle(true);
    }

    public static AssetBundle LoadDependenceAssetBundle(ushort index) {
        var bundleName = bundleNames[index];
        var assetBundleAsset = GetAssetBundleAsset(bundleName);
        var assetBundle = assetBundleAsset.LoadAssetBundle(false);
        if (bundleName.Contains("tobundle/builtin/sprites")) {
            var objects = assetBundle.LoadAllAssets<SpriteAtlas>();
            foreach (var asset in objects) {
                if (!assets.ContainsKey(bundleName)) {
                    assets[bundleName] = new Dictionary<string, BundleAsset>();
                }

                if (!assets[bundleName].ContainsKey(asset.name)) {
                    assets[bundleName][asset.name] = new BundleAsset(bundleName, asset.name, asset);
                }
            }
        }
        return assetBundle;
    }

    public static AssetAsyncOperationHandle LoadAssetBundleAsync(string name) {
        var assetBundleAsset = GetAssetBundleAsset(name);
        return assetBundleAsset.LoadAssetBundleAsync(true);
    }

    public static AssetAsyncOperationHandle LoadDependenceAssetBundleAsync(ushort index) {
        var assetBundleAsset = GetAssetBundleAsset(bundleNames[index]);
        return assetBundleAsset.LoadAssetBundleAsync(false);
    }

    public static void UnloadAssetBundle(string bundleName, bool unloadAsset) {
        if (!GlobalDefine.IsUseBundle) {
            return;
        }

        AdjustBundleName(ref bundleName);
        if (!assetBundleAssets.ContainsKey(bundleName)) {
            DebugEx.LogWarningFormat("UnloadAssetBundle(): 要卸载的包不在缓存中或者已经被卸载 => {0}. ", bundleName);
            return;
        }

        var assetBundleAsset = assetBundleAssets[bundleName];
        assetBundleAsset.Unload(unloadAsset);
        assetBundleAssets.Remove(bundleName);
        if (unloadAsset) {
            if (!assets.TryGetValue(bundleName, out var bundleAssets)) {
                return;
            }

            if (bundleAssets != null) {
                foreach (var bundleAsset in bundleAssets.Values) {
                    bundleAsset.Unload();
                }
            }

            assets.Remove(bundleName);
        }
    }

    public static void UnloadUnPersistentAssets() {
        var assetKeys = new List<string>(assets.Keys);
        for (var i = assetKeys.Count - 1; i >= 0; i--) {
            var key = assetKeys[i];
            if (IsPersistentAsset(key)) {
                continue;
            }

            UnloadAssetBundle(key, true);
        }

        var bundleKeys = new List<string>(assetBundleAssets.Keys);
        for (var i = bundleKeys.Count - 1; i >= 0; i--) {
            var key = bundleKeys[i];
            if (IsPersistentAsset(key)) {
                continue;
            }

            UnloadAssetBundle(key, true);
        }
    }

    public static void UnloadAllAssets() {
        foreach (var bundleName in localAssets.Keys) {
            foreach (var item in localAssets[bundleName]) {
                item.Value.Unload();
            }
        }

        foreach (var bundleName in uNetAssets.Keys) {
            foreach (var item in uNetAssets[bundleName]) {
                item.Value.Unload();
            }
        }

        foreach (var bundleName in assets.Keys) {
            foreach (var item in assets[bundleName]) {
                item.Value.Unload();
            }
        }

        foreach (var item in assetBundleAssets) {
            UnloadAssetBundle(item.Key, true);
        }

        assetBundleAssets.Clear();
    }

    private static ushort[] GetDependentBundles(ushort index) {
        if (dependencies.ContainsKey(index)) {
            return dependencies[index];
        } else {
            return null;
        }
    }

    private static void AdjustBundleName(ref string bundleName) {
        if (string.IsNullOrEmpty(bundleName)) {
            bundleName = "";
        }
        if (bundleName.CustomEndsWith("/")) {
            bundleName = bundleName.TrimEnd('/');
        }

        if (!bundleNamePool.ContainsKey(bundleName)) {
            var lowerName = bundleName.ToLower();
            bundleNamePool.Add(bundleName, lowerName);
        }

        bundleName = bundleNamePool[bundleName];
    }

    public static string GetAssetBundlePath(string url) {
        var relativePath = Path.Combine("AssetBundle", url);
        var path = Path.Combine(URI.persistentDataPath, relativePath);
        if (!File.Exists(path)) {
#if !BUILD_SERVER && UNITY_ANDROID
            if (PlayAssetDeliveryManager.Exists(Path.Combine("AssetBundle", url), out var location)) {
                return location.Path;
            }
#endif
            path = Path.Combine(URI.streamingAssetsPath, relativePath);
        }

        return path;
    }

    public static string ReadAllText(string relativePath) {
        relativePath = string.Concat(relativePath, ".txt");
        string path;
        if (Application.isEditor) {
            path = Path.Combine(URI.configPath, relativePath);
            if (!File.Exists(path)) {
                path = Path.Combine(URI.soundConfigPath, relativePath);
            }

            if (File.Exists(path)) {
                return File.ReadAllText(path);
            }
        }

        path = Path.Combine(URI.persistentDataPath, relativePath);
        if (File.Exists(path)) {
            return File.ReadAllText(path);
        }

        path = Path.Combine(URI.streamingAssetsPath, relativePath);
        if (GlobalDefine.IsAndroidMobile) {
            var www = new WWW(path);
            while (!www.isDone) {
            }

            return www.text;
        } else {
            if (File.Exists(path)) {
                return File.ReadAllText(path);
            }
        }

        return null;
    }

    public static byte[] ReadAllBytes(string relativePath) {
        relativePath = string.Concat(relativePath, ".txt");
        string path;
        if (Application.isEditor) {
            path = Path.Combine(URI.configPath, relativePath);
            if (!File.Exists(path)) {
                path = Path.Combine(URI.soundConfigPath, relativePath);
            }

            if (File.Exists(path)) {
                return ReadFileRawData(path);
            }
        }

        path = Path.Combine(URI.persistentDataPath, relativePath);
        if (File.Exists(path)) {
            return ReadFileRawData(path);
        }

        path = Path.Combine(URI.streamingAssetsPath, relativePath);
        if (GlobalDefine.IsAndroidMobile) {
            var www = new WWW(path);
            while (!www.isDone) {
            }

            if (!string.IsNullOrEmpty(www.error)) {
                www.Dispose();
                return null;
            }

            var src = www.bytes;
            var dst = new byte[src.Length];
            Array.Copy(src, 0, dst, 0, src.Length);
            www.Dispose();
            return dst;
        } else {
            if (File.Exists(path)) {
                return ReadFileRawData(path);
            }
        }

        return null;
    }

    public static string GetAssetPostfix(AssetType assetType) {
        switch (assetType) {
            case AssetType.ScriptableObject:
                return ".asset";
            case AssetType.Prefab:
                return ".prefab";
            case AssetType.JPG:
                return ".jpg";
            case AssetType.Material:
                return ".mat";
            case AssetType.SpriteAtlas:
                return ".spriteatlas";
            case AssetType.Sprite:
                return ".png";
            case AssetType.Shader:
                return ".shader";
            case AssetType.Png:
                return ".png";
            case AssetType.AnimatorController:
                return ".controller";
            case AssetType.Font:
                return ".ttf";
            default:
                return string.Empty;
        }
    }

    private static bool IsPersistentAsset(string bundleName) {
        if (bundleName.Contains("shader")) {
            return true;
        }

        if (bundleName.Contains("ugui/font")) {
            return true;
        }

        if (bundleName.Contains("tobundle/builtin")) {
            return true;
        }

        if (bundleName.Contains("tobundle/ugui/texture/loadingbackground")) {
            return true;
        }

        return false;
    }

    public struct AssetBundlePath {
        public string bundleName;
        public string assetName;
    }

    public static AssetBundlePath ConvertURLToBundlePath(string url) {
        var lastSeparatorIndex = url.LastIndexOf("/", StringComparison.Ordinal);
        return new AssetBundlePath() {
            bundleName = url.Remove(lastSeparatorIndex),
            assetName = url.Substring(lastSeparatorIndex + 1, url.Length - lastSeparatorIndex - 1)
        };
    }

    private static LocalAsset GetLocalAsset(string bundleName, string assetName, AssetType assetType) {
        Dictionary<string, LocalAsset> bundleData;
        if (localAssets.TryGetValue(bundleName, out bundleData) == false) {
            bundleData = new Dictionary<string, LocalAsset>();
            localAssets[bundleName] = bundleData;
        }

        LocalAsset localAsset = null;
        if (!bundleData.TryGetValue(assetName, out localAsset) || localAsset == null) {
            var path = Path.Combine("Assets", bundleName, assetName) + GetAssetPostfix(assetType);
            localAsset = new LocalAsset(path);
            bundleData[assetName] = localAsset;
        }

        return localAsset;
    }

    private static UNetAsset GetUNetAsset(string bundleName, string assetName) {
        UNetAsset uNetAsset = null;
        if (!uNetAssets.ContainsKey(bundleName)) {
            uNetAssets[bundleName] = new Dictionary<string, UNetAsset>();
        }

        if (!uNetAssets[bundleName].TryGetValue(assetName, out uNetAsset) || uNetAsset == null) {
            uNetAsset = new UNetAsset(bundleName, assetName);
            uNetAssets[bundleName][assetName] = uNetAsset;
        }

        return uNetAsset;
    }

    private static AssetBundleAsset GetAssetBundleAsset(string name) {
        AdjustBundleName(ref name);
        if (!assetBundleAssets.TryGetValue(name, out var assetBundleAsset) || assetBundleAsset == null) {
            if (!bundleNameMap.TryGetValue(name, out var index)) {
                throw new Exception($"AssetBundle({name}) not found in the dependency file.");
            }

            assetBundleAsset = new AssetBundleAsset(name, GetDependentBundles(index));
            assetBundleAssets[name] = assetBundleAsset;
        }

        return assetBundleAsset;
    }

    private static BundleAsset GetBundleAsset(string bundleName, string assetName) {
        AdjustBundleName(ref bundleName);
        BundleAsset bundleAsset = null;
        Dictionary<string, BundleAsset> assetData;
        if (assets.TryGetValue(bundleName, out assetData) == false) {
            assetData = new Dictionary<string, BundleAsset>();
            assets[bundleName] = assetData;
        }

        if (!assetData.TryGetValue(assetName, out bundleAsset) || bundleAsset == null) {
            bundleAsset = new BundleAsset(bundleName, assetName);
            assetData[assetName] = bundleAsset;
        }

        return bundleAsset;
    }

    private static byte[] ReadFileRawData(string filename) {
        if (File.Exists(filename)) {
            using (var stream = File.OpenRead(filename)) {
                var data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                return data;
            }
        }

        return null;
    }
}