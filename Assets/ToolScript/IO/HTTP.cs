using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace Funny.IO {
    /// <summary>
    /// 封装System.Net.WebClient
    /// </summary>
    public class HTTP {
        public const string TIME_OUT = "The request timed out.";

        private static int id = 0;
        private static Dictionary<int, HTTP> cache = new Dictionary<int, HTTP>();
        private static List<int> finishedIds = new List<int>();

        private int _id = 0;
        private long _byteLoaded = 0;
        private long _byteTotal = 0;
        private string _error = "";
        private bool _oneCompleted = false;
        private bool _allCompleted = false;
        private byte[] _bytes = null;

        public Action<int, float> onProgress;
        public Action<int, byte[]> onOneFinish;
        public Action<WebClient[]> onFinish;
        public Action<int, string, string> onError;

        private string[] _urls;

        public WebClient[] wcs { get; private set; }
        private int _index = -1;
        private string _url = null;
        private WebClient _wc = null;

        public HTTP() {
            _index = -1;

            id++;
            _id = id;

            cache[_id] = this;
        }

        public void Load(string url) {
            LoadList(new string[] { url });
        }

        public void LoadList(string[] urls) {
            _urls = urls;
            wcs = new WebClient[urls.Length];

            LoadOne();
        }

        private void LoadOne() {
            ++_index;
            _url = _urls[_index];
            _wc = new WebClient();
            wcs[_index] = _wc;

            _wc.DownloadProgressChanged += DownloadProgressChanged;
            _wc.DownloadDataCompleted += DownloadDataCompleted;

            _byteLoaded = 0;
            _wc.DownloadDataAsync(new System.Uri(_url));
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            _byteTotal = e.TotalBytesToReceive;
            _byteLoaded += e.BytesReceived;
        }

        private void DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e) {
            _oneCompleted = true;
            _bytes = (byte[])e.Result;

            if (e.Error != null)
                _error = e.Error.ToString();

            DebugEx.Log(_url + ", " + e.Error + ", " + (byte[])e.Result + ", " + _error);
        }

        private void InnerUpdate() {
            if (_wc != null) {
                if (onProgress != null) {
                    onProgress(_index, (float)((double)_byteLoaded / (double)_byteTotal));
                }

                if (_oneCompleted) {
                    _wc = null;
                    _oneCompleted = false;

                    if (onOneFinish != null) {
                        onOneFinish(_index, _bytes);
                        _bytes = null;
                    }

                    if (_index + 1 < _urls.Length) {
                        LoadOne();
                    } else if (onFinish != null) {
                        onFinish(wcs);

                        finishedIds.Add(_id);
                    }
                }
            }
        }

        public void ResumeLoad(int index) {
            _index = index - 1;
            LoadOne();
        }

        public void Destroy() {
            int len = wcs.Length;
            for (int i = 0; i < len; i++) {
                WebClient wc = wcs[i];
                if (wc != null) {
                    wc.Dispose();
                }
            }
            wcs = null;
        }

        #region

        public static void Update() {
            List<int> list = new List<int>(cache.Keys);
            foreach (int k in list) {
                cache[k].InnerUpdate();
            }

            int len = finishedIds.Count;
            if (len > 0) {
                for (int i = 0; i < len; i++) {
                    cache.Remove(finishedIds[i]);
                }
                finishedIds.Clear();
            }
        }

        #endregion
    }
}