using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class CreateConfigClassFile {
    const string retract1 = "\t";
    const string retract2 = "\t\t";
    const string retract3 = "\t\t\t";
    const string retract4 = "\t\t\t\t";
    const string retract5 = "\t\t\t\t\t";
    
    public static string filedContent = string.Empty;
    public static string readContent = string.Empty;
    public static string propertyContent = string.Empty;

    [UnityEditor.MenuItem("Assets/生成文本配置/生成配置解析类型")]
    public static void GenerateConfigClass() {
        if (Selection.objects != null) {
            foreach (var o in Selection.objects) {
                var path = AssetDatabase.GetAssetPath(o.GetInstanceID());
                if (path.EndsWith(".txt") || path.EndsWith(".TXT")) {
                    CreateConfigClass(new FileInfo(path));
                }
            }

            AssetDatabase.Refresh();
        }
    }

    public static void CreateConfigClass(FileInfo fileInfo) {
        var lines = File.ReadAllLines(fileInfo.FullName);
        if (lines.Length > 2) {
            var typeLine = lines[0];
            var fieldLine = lines[1];
            var types = typeLine.Split('\t');
            var fields = fieldLine.Split('\t');
            var min = Mathf.Min(types.Length, fields.Length);
            var fieldFulls = new List<string>();
            var readFulls = new List<string>();
            var propertyFulls = new List<string>();

            int index = 0;
            for (int j = 0; j < min; j++) {
                var type = types[j];
                var field = fields[j];
                var fieldstring = GetField(type, field);
                var readString = GetRead(type, field, index, j);
                var propertyString = GetProperty(type, field, j);
                if (!string.IsNullOrEmpty(fieldstring)) {
                    fieldFulls.Add(fieldstring);
                }

                if (!string.IsNullOrEmpty(readString)) {
                    index++;
                    readFulls.Add(readString);
                }

                if (!string.IsNullOrEmpty(propertyString)) {
                    propertyFulls.Add(propertyString);
                }
            }

            filedContent = string.Join("\r\n\t", fieldFulls.ToArray());
            readContent = string.Join("\r\n\t\t\t", readFulls.ToArray());
            propertyContent = string.Join("\r\n\t", propertyFulls.ToArray());
            CreatNewConfigClass(fileInfo.Name.Substring(0, fileInfo.Name.IndexOf('.')));
        }

    }

    public static string GetField(string type, string field) {
        field = field.Replace(" ", "");
        if (type.Contains("int[]")) {
            return StringUtil.Concat("public readonly int[] ", field.Trim(), ";");
        } else if (type.Contains("Int2[]")) {
            return StringUtil.Concat("public readonly Int2[] ", field.Trim(), ";");
        } else if (type.Contains("Int3[]")) {
            return StringUtil.Concat("public readonly Int3[] ", field.Trim(), ";");
        } else if (type.Contains("float[]")) {
            return StringUtil.Concat("public readonly float[] ", field.Trim(), ";");
        } else if (type.Contains("string[]")) {
            return StringUtil.Concat("public readonly string[] ", field.Trim(), ";");
        } else if (type.Contains("Vector3[]")) {
            return StringUtil.Concat("public readonly Vector3[] ", field.Trim(), ";");
        } else if (type.Contains("int")) {
            return StringUtil.Concat("public readonly int ", field.Trim(), ";");
        } else if (type.Contains("long")) {
            return StringUtil.Concat("public readonly long ", field.Trim(), ";");
        } else if (type.Contains("long[]")) {
            return StringUtil.Concat("public readonly long[] ", field.Trim(), ";");
        } else if (type.Contains("float")) {
            return StringUtil.Concat("public readonly float ", field.Trim(), ";");
        } else if (type.Contains("string")) {
            return StringUtil.Concat("public readonly string ", field, ";");
        } else if (type.Contains("Vector3")) {
            return StringUtil.Concat("public readonly Vector3 ", field.Trim(), ";");
        } else if (type.Contains("bool")) {
            return StringUtil.Concat("public readonly bool ", field.Trim(), ";");
        } else if (type.Contains("Int2")) {
            return StringUtil.Concat("public readonly Int2 ", field.Trim(), ";");
        } else if (type.Contains("Int3")) {
            return StringUtil.Concat("public readonly Int3 ", field.Trim(), ";");
        }else if (type.Contains("MagicFloat")) {
            return StringUtil.Concat("public readonly MagicType.MagicFloat ", field.Trim(), ";");
        } else if (type.Contains("MagicInt")) {
            return StringUtil.Concat("public readonly MagicType.MagicInt ", field.Trim(), ";");
        } else {
            return string.Empty;
        }
    }

    public static string GetRead(string type, string field, int index, int dataIndex) {
        field = field.Replace(" ", "");
        if (type.Contains("int[]")) {
            var line1 = StringUtil.Concat("string[] ", field, "StringArray", " = ", "tables", "[", index, "]", ".Trim().Split(StringUtil.splitSeparator,StringSplitOptions.RemoveEmptyEntries);", "\n");
            var line2 = StringUtil.Concat(retract3, field, " = ", "new int", "[", field, "StringArray.Length]", ";", "\n");
            var line3 = StringUtil.Concat(retract3, "for (int i=0;i<", field, "StringArray", ".Length", ";", "i++", ")", "\n");
            var line4 = StringUtil.Concat(retract3, "{\n");
            var line5 = StringUtil.Concat(retract4, " int.TryParse(", field, "StringArray", "[i]", ",", "out ", field, "[i]", ")", ";", "\n");
            var line6 = StringUtil.Concat(retract3, "}");

            return StringUtil.Concat(line1, line2, line3, line4, line5, line6);
        } else if (type.Contains("Int2[]")) {
            var line1 = StringUtil.Concat("string[] ", field, "StringArray", " = ", "tables", "[", index, "]", ".Trim().Split(StringUtil.splitSeparator,StringSplitOptions.RemoveEmptyEntries);", "\n");
            var line2 = StringUtil.Concat(retract3, field, " = ", "new Int2", "[", field, "StringArray.Length]", ";", "\n");
            var line3 = StringUtil.Concat(retract3, "for (int i=0;i<", field, "StringArray", ".Length", ";", "i++", ")", "\n");
            var line4 = StringUtil.Concat(retract3, "{\n");
            var line5 = StringUtil.Concat(retract4, " Int2.TryParse(", field, "StringArray", "[i]", ",", "out ", field, "[i]", ")", ";", "\n");
            var line6 = StringUtil.Concat(retract3, "}");

            return StringUtil.Concat(line1, line2, line3, line4, line5, line6);
        } else if (type.Contains("Int3[]")) {
            var line1 = StringUtil.Concat("string[] ", field, "StringArray", " = ", "tables", "[", index, "]", ".Trim().Split(StringUtil.splitSeparator,StringSplitOptions.RemoveEmptyEntries);", "\n");
            var line2 = StringUtil.Concat(retract3, field, " = ", "new Int3", "[", field, "StringArray.Length]", ";", "\n");
            var line3 = StringUtil.Concat(retract3, "for (int i=0;i<", field, "StringArray", ".Length", ";", "i++", ")", "\n");
            var line4 = StringUtil.Concat(retract3, "{\n");
            var line5 = StringUtil.Concat(retract4, " Int3.TryParse(", field, "StringArray", "[i]", ",", "out ", field, "[i]", ")", ";", "\n");
            var line6 = StringUtil.Concat(retract3, "}");

            return StringUtil.Concat(line1, line2, line3, line4, line5, line6);
        } else if (type.Contains("float[]")) {
            var line1 = StringUtil.Concat("string[] ", field, "StringArray", " = ", "tables", "[", index, "]", ".Trim().Split(StringUtil.splitSeparator,StringSplitOptions.RemoveEmptyEntries);", "\n");
            var line2 = StringUtil.Concat(retract3, field, " = ", "new float", "[", field, "StringArray.Length", "]", ";", "\n");
            var line3 = StringUtil.Concat(retract3, "for (int i=0;i<", field, "StringArray", ".Length", ";", "i++", ")", "\n");
            var line4 = StringUtil.Concat(retract3, "{\n");
            var line5 = StringUtil.Concat(retract4, " float.TryParse(", field, "StringArray", "[i]", ",", "out ", field, "[i]", ")", ";", "\n");
            var line6 = StringUtil.Concat(retract3, "}");

            return StringUtil.Concat(line1, line2, line3, line4, line5, line6);
        } else if (type.Contains("string[]")) {
            var line1 = StringUtil.Concat(field, " = ", "tables", "[", index, "]", ".Trim().Split(StringUtil.splitSeparator,StringSplitOptions.RemoveEmptyEntries);");
            return line1;
        } else if (type.Contains("Vector3[]")) {
            var line1 = StringUtil.Concat("string[] ", field, "StringArray", " = ", "tables", "[", index, "]", ".Trim().Split(StringUtil.splitSeparator,StringSplitOptions.RemoveEmptyEntries);", "\n");
            var line2 = StringUtil.Concat(retract3, field, " = ", "new Vector3", "[", field, "StringArray.Length", "]", ";", "\n");
            var line3 = StringUtil.Concat(retract3, "for (int i=0;i<", field, "StringArray", ".Length", ";", "i++", ")", "\n");
            var line4 = StringUtil.Concat(retract3, "{\n");
            var line5 = StringUtil.Concat(retract4, field, "[i]", "=", field, "StringArray", "[i]", ".Vector3Parse()", ";", "\n");
            var line6 = StringUtil.Concat(retract3, "}");

            return StringUtil.Concat(line1, line2, line3, line4, line5, line6);
        } else if (type.Contains("int")) {
            return StringUtil.Concat("int.TryParse(tables", "[", index, "]", ",", "out ", field, ")", "; ");
        } else if (type.Contains("float")) {
            return StringUtil.Concat("float.TryParse(tables", "[", index, "]", ",", "out ", field, ")", "; ");
        } else if (type.Contains("string")) {
            return StringUtil.Concat(field, " = ", "tables", "[", index, "]", ";");
        } else if (type.Contains("Vector3")) {
            return StringUtil.Concat(field, "=", "tables", "[", index, "]", ".Vector3Parse()", ";");
        } else if (type.Contains("bool")) {
            var line1 = StringUtil.Concat("var ", field, "Temp", " = 0", ";", "\n");
            var line2 = StringUtil.Concat(retract3, "int.TryParse(tables", "[", index, "]", ",", "out ", field, "Temp", ")", "; ", "\n");
            var line3 = StringUtil.Concat(retract3, field, "=", field, "Temp", "!=0", ";");
            return StringUtil.Concat(line1, line2, line3);
        } else if (type.Contains("Int2")) {
            return StringUtil.Concat("Int2.TryParse(tables", "[", index, "]", ",", "out ", field, ")", "; ");
        } else if (type.Contains("Int3")) {
            return StringUtil.Concat("Int3.TryParse(tables", "[", index, "]", ",", "out ", field, ")", "; ");
        } else if (type.Contains("MagicFloat")) {
            return StringUtil.Concat("MagicType.MagicFloat.TryParse(tables", "[", index, "]", ",", "out ", field, ")", "; ");
        } else if (type.Contains("MagicInt")) {
            return StringUtil.Concat("MagicType.MagicInt.TryParse(tables", "[", index, "]", ",", "out ", field, ")", "; ");
        }else if (type.Contains("long")) {
            return StringUtil.Concat("long.TryParse(tables", "[", index, "]", ",", "out ", field, ")", "; ");
        } else {
            return string.Empty;
        }
    }

    public static string GetProperty(string type, string field, int dataIndex) {
        // if (type.Contains("string")) {
        //     if (currentLanguageColumns != null && currentLanguageColumns.columns.Contains(dataIndex)) {
        //         var propertyField = StringUtil.Concat(field.Substring(0, 1).ToUpper(), field.Substring(1));
        //         var line1 = StringUtil.Concat("public string ", propertyField, $" => Language.Get({field});\n");
        //         return line1;
        //     }
        // }
        return string.Empty;
    }
    
    static string configClassPath = "Assets" + "/" + "Script/Config";
    static string templatePath = "Assets/ToolScript/Editor/Template/ConfigTemplate.txt";

    public static void CreatNewConfigClass(string name) {
        var newConfigPath = configClassPath + string.Format("/{0}Config.cs", name);
        AssetDatabase.DeleteAsset(newConfigPath);
        UnityEngine.Object o = CreateScriptAssetFromTemplate(newConfigPath, templatePath);
        ProjectWindowUtil.ShowCreatedAsset(o);
    }

    internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile) {
        var fullPath = Path.GetFullPath(pathName);

        var streamReader = new StreamReader(resourceFile);
        var text = streamReader.ReadToEnd();
        streamReader.Close();
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
        text = Regex.Replace(text, "#ClassName*#", fileNameWithoutExtension);
        text = Regex.Replace(text, "#DateTime#", System.DateTime.Now.ToLongDateString());
        text = Regex.Replace(text, "#Field#", CreateConfigClassFile.filedContent);
        text = Regex.Replace(text, "#Read#", CreateConfigClassFile.readContent);
        text = Regex.Replace(text, "#Property#", CreateConfigClassFile.propertyContent);
        text = Regex.Replace(text, "#FileName#", fileNameWithoutExtension.Substring(0, fileNameWithoutExtension.Length - 6));

        var encoderShouldEmitUTF8Identifier = true;
        var throwOnInvalidBytes = false;
        var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
        var append = false;
        var streamWriter = new StreamWriter(fullPath, append, encoding);
        streamWriter.Write(text);
        streamWriter.Close();
        AssetDatabase.ImportAsset(pathName);
        return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
    }
}