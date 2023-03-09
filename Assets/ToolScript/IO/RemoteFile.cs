using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Funny.IO {
    /// <summary>
    /// 封装UnityEngine.WWW
    /// </summary>
    public class RemoteFile {
        public const string TIME_OUT = "The request timed out.";

        private static int id = 0;
        private static Dictionary<int, RemoteFile> rfCache = new Dictionary<int, RemoteFile>();
        private static List<int> finishedIds = new List<int>();

        private int _id = 0;

        public Action<int, float> onProgress;
        public Action<int, WWW> onOneFinish;
        public Action<WWW[]> onFinish;
        public Action<int, string, string> onError;

        private string[] _urls;

        public WWW[] wwws { get; private set; }
        private int _index = -1;
        private string _url = null;
        private WWW _www = null;

        public RemoteFile() {
            _index = -1;

            id++;
            _id = id;

            rfCache[_id] = this;
        }

        public void Load(string url) {
            LoadList(new string[] { url });
        }

        public void LoadList(string[] urls) {
            _urls = urls;
            wwws = new WWW[urls.Length];

            LoadOne();
        }

        private void LoadOne() {
            ++_index;
            _url = _urls[_index];
            _www = new WWW(_url);
            wwws[_index] = _www;
        }

        private void InnerUpdate() {
            if (_www != null) {
                if (onProgress != null) {
                    onProgress(_index, _www.progress);
                }

                if (_www.isDone) {
                    LoadedOne();
                } else if (_www.error == TIME_OUT) {
                    doError();
                }
            }
        }

        private void LoadedOne() {
            if (!String.IsNullOrEmpty(_www.error)) {
                doError();
            } else {
                if (onOneFinish != null) {
                    onOneFinish(_index, _www);
                    _www = null;
                }

                if (_index + 1 < _urls.Length) {
                    LoadOne();
                } else if (onFinish != null) {
                    onFinish(wwws);

                    finishedIds.Add(_id);
                }
            }
        }

        private void doError() {
            string error = _www.error;
            _www = null;
            finishedIds.Add(_id);

            if (onError != null) {
                onError(_index, _url, error);
            }
        }

        public void ResumeLoad(int index) {
            _index = index - 1;
            LoadOne();
        }

        public void Destroy() {
            WWW www;

            int len = wwws.Length;
            for (int i = 0; i < len; i++) {
                www = wwws[i];
                if (www != null) {
                    if (www.assetBundle != null) {
                        www.assetBundle.Unload(true);
                    }
                    www.Dispose();
                }
            }

            wwws = null;
        }

        #region

        public static void Update() {
            List<int> list = new List<int>(rfCache.Keys);
            foreach (int k in list) {
                rfCache[k].InnerUpdate();
            }

            int len = finishedIds.Count;
            if (len > 0) {
                for (int i = 0; i < len; i++) {
                    rfCache.Remove(finishedIds[i]);
                }
                finishedIds.Clear();
            }
        }

        #endregion
    }
}