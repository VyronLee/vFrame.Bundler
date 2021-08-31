//------------------------------------------------------------
//        File:  CoroutinePool.cs
//       Brief:  Coroutine pool.
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-09-08 22:09
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vFrame.Bundler.Base.Coroutine
{
    internal class CoroutinePool
    {
        private static int _index;

        private static GameObject _parent;

        private static GameObject PoolParent {
            get {
                if (_parent) {
                    return _parent;
                }
                _parent = new GameObject("BundleCoroutinePools");
                Object.DontDestroyOnLoad(_parent);
                return _parent;
            }
        }

        internal readonly int Capacity;
        internal readonly List<CoroutineBehaviour> CoroutineList;
        internal readonly List<CoroutineTask> TasksRunning;
        internal readonly List<CoroutineTask> TasksWaiting;

        private readonly GameObject _holder;
        private int _taskHandle;

        public CoroutinePool(string name = null, int capacity = int.MaxValue) {
            Capacity = capacity;
            CoroutineList = new List<CoroutineBehaviour>();

            TasksRunning = new List<CoroutineTask>(16);
            TasksWaiting = new List<CoroutineTask>(16);

            _holder = new GameObject(string.Format("Pool_{0}({1})", ++_index, name ?? "Unnamed"));
            _holder.AddComponent<CoroutinePoolBehaviour>().Pool = this;
            _holder.transform.SetParent(PoolParent.transform);
        }

        public void Destroy() {
            foreach (var task in TasksRunning) {
                var runner = CoroutineList[task.RunnerId];
                runner.CoStop();
                Object.Destroy(runner.gameObject);
            }
            TasksRunning.Clear();
            TasksWaiting.Clear();

            Object.Destroy(_holder);
        }

        public int StartCoroutine(IEnumerator task) {
            var handle = GenerateTaskHandle();

            var context = new CoroutineTask {Handle = handle, Task = task};

            if (TasksRunning.Count < Capacity) {
                RunTask(context);
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
            foreach (var task in TasksRunning) {
                if (task.Handle != handle)
                    continue;

                var runner = CoroutineList[task.RunnerId];
                if (runner) {
                    runner.CoStop();
                }

                TasksRunning.Remove(task);
                break;
            }
        }

        public void PauseCoroutine(int handle) {
            foreach (var task in TasksRunning) {
                if (task.Handle != handle)
                    continue;

                var runner = CoroutineList[task.RunnerId];
                runner.Pause();
                break;
            }
        }

        public void UnPauseCoroutine(int handle) {
            foreach (var task in TasksRunning) {
                if (task.Handle != handle)
                    continue;

                var runner = CoroutineList[task.RunnerId];
                runner.UnPause();
                break;
            }
        }

        private int GenerateTaskHandle() {
            return ++_taskHandle;
        }

        private void RunTask(CoroutineTask context) {
            context.RunnerId = FindIdleRunner();
            TasksRunning.Add(context);

            var runner = GetOrSpawnRunner(context.RunnerId);
            runner.CoStart(context);

            if (!runner.IsRunning()) {
                TasksRunning.Remove(context);
            }
        }

        private CoroutineBehaviour GetOrSpawnRunner(int runnerId) {
            if (runnerId < CoroutineList.Count) {
                return CoroutineList[runnerId];
            }

            var runner = new GameObject("Coroutine_" + runnerId).AddComponent<CoroutineBehaviour>();
            runner.OnFinished = OnFinished;
            runner.transform.SetParent(_holder.transform);

            CoroutineList.Add(runner);
            return runner;
        }

        private int FindIdleRunner() {
            for (var i = 0; i < Capacity; i++) {
                var running = false;
                foreach (var task in TasksRunning) {
                    if (task.RunnerId != i)
                        continue;
                    running = true;
                    break;
                }

                if (!running)
                    return i;
            }

            throw new System.IndexOutOfRangeException("No idling runner now.");
        }

        private void OnFinished(CoroutineTask context) {
            TasksRunning.Remove(context);
            PopupAndRunNext();
        }

        private void PopupAndRunNext() {
            if (TasksWaiting.Count <= 0)
                return;

            var context = TasksWaiting[0];
            TasksWaiting.RemoveAt(0);
            RunTask(context);
        }
    }
}