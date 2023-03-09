using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class ConfigManager : Singleton<ConfigManager> {
    // 预加载配置
    public void StartUp() {
       
    }

    public void ClearCache() {
    }

    public static string[] GetConfigRawDatas(string fileName) {
        // if (IsUseBundle) {
        //     if (LocalConfig.IsEncryptConfigs(fileName)) {
        //         try {
        //             return GetEncryptConfigRawDatas(fileName);
        //         } catch (Exception e) {
        //             DebugEx.LogError(e);
        //         }
        //     }
        // }
        
        var relativePath = Path.Combine("Txt", fileName);
        var content = AssetManager.ReadAllText(relativePath);
        var lines = content.Split(new[] {
            "\r\n",
            "\n"
        }, StringSplitOptions.RemoveEmptyEntries);
        
        return lines;
    }

    private static string[] GetEncryptConfigRawDatas(string fileName) {
        var relativePath = Path.Combine("Txt", fileName);
        var bytes = AssetManager.ReadAllBytes(relativePath);
        // if (GlobalDefine.IsUseBundle) {
        //     bytes = XXTEA.Decrypt(bytes, Assets.encryptKey);
        // }
        var content = Encoding.UTF8.GetString(bytes);
        var lines = content.Split(new[] {
            "\r\n",
            "\n"
        }, StringSplitOptions.RemoveEmptyEntries);
        return lines;
    }

    public static bool TryGetConfigRawDatas(string fileName, out string[] lines) {
        var content = AssetManager.ReadAllText(Path.Combine("Txt", fileName));
        if (string.IsNullOrEmpty(content)) {
            lines = null;
            return false;
        }

        lines = content.Split(new[] {
            "\r\n",
            "\n"
        }, StringSplitOptions.RemoveEmptyEntries);
        return true;
    }
}