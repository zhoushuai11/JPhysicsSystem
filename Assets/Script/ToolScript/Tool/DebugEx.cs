// Decompiled with JetBrains decompiler
// Type: DebugEx
// Assembly: DebugEx, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5B53D3FE-71FA-43C2-B8A7-F42B6C0ED65C
// Assembly location: D:\Doc\sausage-man\Assets\Plugins\DebugEx.dll

using UnityEngine;

public class DebugEx
{
    public static bool EnableLog = true;
    public static bool EnableLogWarning = true;
    public static bool EnableLogError = true;
    public static bool EnableNetLog = true;

    public static void Log(object message)
    {
        if (!DebugEx.EnableLog)
            return;
        Debug.Log(message);
    }

    public static void LogFormat(string message, params object[] _objs)
    {
        if (!DebugEx.EnableLog)
            return;
        Debug.LogFormat(message, _objs);
    }

    public static void LogError(object message)
    {
        if (!DebugEx.EnableLogError)
            return;
        Debug.LogError(message);
    }

    public static void LogErrorFormat(string message, params object[] _objs)
    {
        if (!DebugEx.EnableLogError)
            return;
        Debug.LogErrorFormat(message, _objs);
    }

    public static void LogWarning(object message)
    {
        if (!DebugEx.EnableLogWarning)
            return;
        Debug.LogWarning(message);
    }

    public static void LogWarningFormat(string message, params object[] _objs)
    {
        if (!DebugEx.EnableLogWarning)
            return;
        Debug.LogWarningFormat(message, _objs);
    }

    public static void NetLog(object message)
    {
        if (!DebugEx.EnableNetLog)
            return;
        Debug.Log(message);
    }

    public static void NetLogFormat(string message, params object[] _objs)
    {
        if (!DebugEx.EnableNetLog)
            return;
        Debug.LogFormat(message, _objs);
    }
}