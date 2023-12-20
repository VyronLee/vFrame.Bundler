//------------------------------------------------------------
//        File:  CoroutinePool.cs
//       Brief:  Coroutine pool.
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-09-08 22:09
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vFrame.Bundler.Extension;
using Object = UnityEngine.Object;

namespace vFrame.Bundler.Base.Coroutine
{
    internal class CoroutinePool
    {
        private static int _index;

        private static GameObject _parent;

        private static GameObject PoolParent {
            get {
                if (_parent)
                    return _parent;

                _parent = new GameObject("CoroutinePools(Bundler)");
                if (Application.isPlaying) {
                    Object.DontDestroyOnLoad(_parent);
                }
                return _parent;
            }
        }

        internal readonly int Capacity;
        internal readonly List<CoroutineRunnerBehaviour> RunnerList;
        internal readonly List<CoroutineTask> TasksWaiting;

        private readonly GameObject _holder;
        private int _taskHandle;

        public CoroutinePool(string name = null, int capacity = int.MaxValue) {
            Capacity = capacity;
            RunnerList = new List<CoroutineRunnerBehaviour>();

            TasksWaiting = new List<CoroutineTask>(16);

            _holder = new GameObject(string.Format("Pool_{0}({1})", ++_index, name ?? "Unnamed"));
            _holder.AddComponent<CoroutinePoolBehaviour>().Pool = this;
            _holder.transform.SetParent(PoolParent.transform);
        }

        public void Destroy() {
            foreach (var runner in RunnerList) {
                if (!runner) {
                    continue;
                }
                runner.CoStop();
                runner.gameObject.DestroyEx();
            }
            RunnerList.Clear();
            TasksWaiting.Clear();

            if (_holder) {
                _holder.DestroyEx();
            }
        }

        public int StartCoroutine(IEnumerator task) {
            var handle = GenerateTaskHandle();
            var context = new CoroutineTask {Handle = handle, Task = task};
            var runnerId = FindIdleRunner();
            if (runnerId >= 0) {
                RunTask(context, runnerId);
            }
            else {
                TasksWaiting.Add(context);
            }

            return handle;
        }

        public void StopCoroutine(int handle) {
            // Remove from waiting list
            foreach (var context in TasksWaiting) {
                if (context.Handle != handle)
                    continue;
                TasksWaiting.Remove(context);
                break;
            }

            // Remove from running list
            foreach (var runner in RunnerList) {
                if (!runner.IsRunning()) {
                    continue;
                }
                if (runner.TaskHandle != handle) {
                    continue;
                }
                runner.CoStop();
                break;
            }
        }

        public void PauseCoroutine(int handle) {
            foreach (var runner in RunnerList) {
                if (runner.TaskHandle != handle) {
                    continue;
                }
                runner.Pause();
                break;
            }
        }

        public void UnPauseCoroutine(int handle) {
            foreach (var runner in RunnerList) {
                if (runner.TaskHandle != handle) {
                    continue;
                }
                runner.UnPause();
                break;
            }
        }

        private int GenerateTaskHandle() {
            return ++_taskHandle;
        }

        private void RunTask(CoroutineTask context, int runnerId) {
            context.RunnerId = runnerId;

            var runner = GetOrSpawnRunner(runnerId);
            runner.CoStart(context);
        }

        private CoroutineRunnerBehaviour GetOrSpawnRunner(int runnerId) {
            if (runnerId < 0 || runnerId >= Capacity) {
                throw new IndexOutOfRangeException(string.Format("Runner id must between [0, {0})", Capacity));
            }
            if (runnerId < RunnerList.Count) {
                return RunnerList[runnerId];
            }

            var idx = RunnerList.Count;
            while (idx <= runnerId) {
                var runner = new GameObject("Coroutine_" + idx).AddComponent<CoroutineRunnerBehaviour>();
                runner.transform.SetParent(_holder.transform);
                runner.RunnerId = idx;
                runner.CoStop();
                RunnerList.Add(runner);
                ++idx;
            }

            return RunnerList[runnerId];
        }

        private int FindIdleRunner() {
            foreach (var runner in RunnerList) {
                if(runner.IsStopped()) {
                    return runner.RunnerId;
                }
            }
            if (RunnerList.Count < Capacity) {
                return RunnerList.Count;
            }
            return -1;
        }

        private bool TryPopupAndRunNext() {
            if (TasksWaiting.Count <= 0)
                return false;

            var runnerId = FindIdleRunner();
            if (runnerId < 0) {
                return false;
            }

            var context = TasksWaiting[0];
            TasksWaiting.RemoveAt(0);

            RunTask(context, runnerId);
            return true;
        }

        internal void OnUpdate() {
            while (TryPopupAndRunNext()) {
                // nothing to do
            }
        }

    }
}