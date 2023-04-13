using UnityEngine;
using System;
using System.Collections;

namespace Funny.IO {
    public class LocalFile {
        private MonoBehaviour _mb;

        public Action<int, string> onError;
        public Action<ResourceRequest[]> onFinish;

        private string[] _list;

        public int index { get; private set; }
        public ResourceRequest[] files { get; private set; }

        public LocalFile(MonoBehaviour mb) {
            _mb = mb;
            index = -1;
        }

        public void Load(ref string path,
                          Action<ResourceRequest[]> finishHandler = null,
                          Action<int, string> errorHandler = null) {
            string[] paths = new string[] { path };
            LoadList(ref paths, finishHandler, errorHandler);
        }

        public void LoadList(ref string[] list,
                              Action<ResourceRequest[]> finishHandler = null,
                              Action<int, string> errorHandler = null) {
            _list = list;
            files = new ResourceRequest[list.Length];

            if (errorHandler != null)
                onError = errorHandler;
            if (finishHandler != null)
                onFinish = finishHandler;

            _mb.StartCoroutine(loadOne());
        }

        IEnumerator loadOne() {
            ++index;
            ResourceRequest request = Resources.LoadAsync(_list[index]);
            files[index] = request;
            yield return request;

            if (request.asset == null) {
                if (onError != null)
                    onError(index, _list[index]);
            } else {
                if (index + 1 < _list.Length)
                    _mb.StartCoroutine(loadOne());
                else if (onFinish != null)
                    onFinish(files);
            }
        }
    }
}