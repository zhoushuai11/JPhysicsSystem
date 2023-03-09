using UnityEngine;
using System.Text;
using System;
using System.Diagnostics;

public static class StringUtil {
    public static readonly string[] splitSeparator = {
        "|"
    };
    private static readonly StringBuilder stringBuilder = new StringBuilder();
    private static readonly object lockObject = new object();

    public static string Concat(object a, object b) {
        lock (lockObject) {
            stringBuilder.Length = 0;
            stringBuilder.Append(a);
            stringBuilder.Append(b);
            return stringBuilder.ToString();
        }
    }

    public static string Concat(object a, object b, object c) {
        lock (lockObject) {
            stringBuilder.Length = 0;
            stringBuilder.Append(a);
            stringBuilder.Append(b);
            stringBuilder.Append(c);
            return stringBuilder.ToString();
        }
    }
    
    public static string Concat(params object[] objects) {
        if (objects == null) {
            return string.Empty;
        }
        lock (lockObject) {
            stringBuilder.Length = 0;
            foreach (var item in objects) {
                if (item != null) {
                    stringBuilder.Append(item);
                }
            }
            return stringBuilder.ToString();
        }
    }

    public static bool CustomEndsWith(this string a, string b) {
        var ap = a.Length - 1;
        var bp = b.Length - 1;
        if (ap < bp) {
            return false;
        }
        while (ap >= 0 && bp >= 0 && a[ap] == b[bp]) {
            ap--;
            bp--;
        }
        return (bp < 0 && a.Length >= b.Length) || (ap < 0 && b.Length >= a.Length);
    }

    public static bool CustomStartsWith(this string a, string b) {
        var aLen = a.Length;
        var bLen = b.Length;
        if (aLen < bLen) {
            return false;
        }
        var ap = 0;
        var bp = 0;
        while (ap < aLen && bp < bLen && a[ap] == b[bp]) {
            ap++;
            bp++;
        }
        return (bp == bLen && aLen >= bLen) || (ap == aLen && bLen >= aLen);
    }

    public static string Substring(string input, int startIndex, int length) {
        if (string.IsNullOrEmpty(input)) {
            throw new NullReferenceException("空字符串无法执行这项操作");
        }
        return input.Substring(startIndex, length);
    }

    [Conditional("UNITY_EDITOR")]
    public static void RenameGameObject(GameObject gameObject, params object[] objects) {
        gameObject.name = Concat(objects);
    }
}