using UnityEngine;

using System.IO;
using System.Text;
using System.Collections.Generic;

using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace Funny.IO {
    public static class SFFile {
        public static bool isAndroid = false;

        /// <summary>
        /// Gets the content of the file.
        /// </summary>
        /// <returns>The file content.</returns>
        /// <param name="path">Path: Assets/a, Assets/a/b</param>
        public static List<string> GetFileContent(string path) {
            if (!File.Exists(path)) return null;
            List<string> lines = new List<string>();
            StreamReader sr = File.OpenText(path);
            string line;
            while ((line = sr.ReadLine()) != null) {
                lines.Add(line);
            }

            sr.Close();
            return lines;
        }

        /// <summary>
        /// Gets the content of the file.
        /// </summary>
        /// <returns>The file content.</returns>
        /// <param name="path">Path: Assets/a, Assets/a/b</param>
        public static string GetFileContent1(string path) {
            if (!File.Exists(path)) return "";
            StreamReader sr = File.OpenText(path);
            string content = sr.ReadToEnd();
            sr.Close();
            return content;
        }

        public static void PutFileContent(string path, string content) {
            var directory = Path.GetDirectoryName(path);
            if (Directory.Exists(directory)) {
                File.WriteAllText(path, content);
            }
        }

        public static void AppendFileContent(string path, string content) {
            byte[] arr = new UTF8Encoding(true).GetBytes(content);
            FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
            fs.Write(arr, 0, arr.Length);
            fs.Close();
        }

        public static string GetFileContentFromStreamingAssets(string path) {
            if (Application.isEditor) {
                path = Application.dataPath + "/Script/Lua/" + path;
            } else {
                path = URI.streamingAssetsPath + "/" + path;
            }

            if (isAndroid) {
                WWW www = new WWW(path);
                while (!www.isDone) ;

                // 没有错误
                if (www.error == "" || www.error == null) {
                    return www.text;
                }

                // 出错，返回空
                else {
                    return "";
                }
            } else {
                return SFFile.GetFileContent1(path);
            }
        }

        public static void DeleteFile(string path) {
            if (File.Exists(path))
                File.Delete(path);
        }

        public static bool FileExists(string path) {
            return File.Exists(path);
        }

        public static void CreateFolder(string path) {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
        }

        public static void DeleteFolder(string path, bool recursive) {
            if (Directory.Exists(path)) {
                Directory.Delete(path, recursive);
            }
        }

        public static bool FolderExists(string path) {
            return Directory.Exists(path);
        }

        public static string[] GetDirectories(string path) {
            return Directory.GetDirectories(path);
        }

        public static string[] GetFiles(string path) {
            return Directory.GetFiles(path);
        }

        public static void WriteBytes(string path, WWW www) {
            WriteBytes(path, www.bytes);
        }

        public static void WriteBytes(string path, byte[] bytes) {
            File.WriteAllBytes(path, bytes);
        }

        public static byte[] ReadBytes(string path) {
            return File.ReadAllBytes(path);
        }

        public static void MoveFile(string formPath, string toPath) {
            System.IO.File.Move(formPath, toPath);
        }

        public static string GetMD5HashFromFile(string fileName) {
            try {
                var file = new FileStream(fileName, FileMode.Open);
                var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                var retVal = md5.ComputeHash(file);
                file.Close();
                var sb = new StringBuilder();
                for (var i = 0; i < retVal.Length; i++) {
                    sb.Append(retVal[i].ToString("x2"));
                }

                return sb.ToString();
            } catch (Exception ex) {
                DebugEx.LogErrorFormat("GetMD5HashFromFile() fail, error: {0}", ex.Message);
                return "";
            }
        }

        public static void CopyDirectoryMultiThreading(string from, string to, string search, string[] excludeExtensions, ref float progress) {
            if (!Directory.Exists(to)) {
                Directory.CreateDirectory(to);
            }

            var files = new List<string>();
            files.AddRange(Directory.GetFiles(from, search, SearchOption.AllDirectories));
            var copyTasks = new List<CopyTask>();
            foreach (var file in files) {
                if (excludeExtensions != null && excludeExtensions.Length > 0) {
                    var extension = Path.GetExtension(file);
                    if (Array.Exists(excludeExtensions, x => x == extension)) {
                        continue;
                    }
                }

                var fileTo = file.Replace(from, to);
                copyTasks.Add(new CopyTask() {
                    fileFrom = file,
                    fileTo = fileTo
                });
            }

            foreach (var task in copyTasks) {
                var directoryTo = Path.GetDirectoryName(task.fileTo);
                if (!Directory.Exists(directoryTo)) {
                    Directory.CreateDirectory(directoryTo);
                }
            }

            foreach (var task in copyTasks) {
                ThreadPool.QueueUserWorkItem(x => {
                    if (!File.Exists(task.fileTo)) {
                        File.Copy(task.fileFrom, task.fileTo, true);
                    } else {
                        var fromFileMD5 = GetMD5HashFromFile(task.fileFrom);
                        var fileToMD5 = GetMD5HashFromFile(task.fileTo);
                        if (fromFileMD5 != fileToMD5) {
                            File.Copy(task.fileFrom, task.fileTo, true);
                        }
                    }

                    task.isDone = true;
                });
            }

            var completedCount = 0;
            var total = copyTasks.Count;
            while (completedCount < total) {
                completedCount = 0;
                foreach (var task in copyTasks) {
                    completedCount += task.isDone ? 1 : 0;
                }

                progress = (float)completedCount / total;
            }

            progress = 1f;
        }

        public static void GetFileCount(string directory, string pattern, ref int count) {
            if (!Directory.Exists(directory)) {
                return;
            }

            try {
                var files = Directory.GetFiles(directory, pattern, SearchOption.TopDirectoryOnly);
                count += files.Length;
                var directories = Directory.GetDirectories(directory);
                foreach (var s in directories) {
                    GetFileCount(s, pattern, ref count);
                }
            } catch (Exception e) {
                DebugEx.LogError($"查找文件：{directory} 失败");
            }
        }

        private class CopyTask {
            public string fileFrom;
            public string fileTo;
            public bool isDone;
        }
    }
}