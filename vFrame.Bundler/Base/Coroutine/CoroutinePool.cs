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
using Object = UnityEngine.Object;

namespace vFrame.Bundler.Base.Coroutine
{
    internal class CoroutinePool
    {
        private static int _index;

        private static GameObject _parent;
        private static bool _parentCreated;

        private static GameObject PoolParent {
            get {
                if (_parentCreated)
                    return _parent;

                _parent = new GameObject("CoroutinePools(Bundler)");
                Object.DontDestroyOnLoad(_parent);

                _parentCreated = true;
                return _parent;
            }
        }

        internal readonly int Capacity;
        internal readonly HashSet<int> IdleSlots;
        internal readonly List<CoroutineRunnerBehaviour> CoroutineList;
        internal readonly List<CoroutineTask> TasksRunning;
        internal readonly List<CoroutineTask> TasksWaiting;

        private readonly GameObject _holder;
        private int _taskHandle;

        private readonly Action<CoroutineTask> _onTaskFinished;

        public CoroutinePool(string name = null, int capacity = int.MaxValue) {
            Capacity = capacity;
            IdleSlots = new HashSet<int>();
            CoroutineList = new List<CoroutineRunnerBehaviour>();

            TasksRunning = new List<CoroutineTask>(16);
            TasksWaiting = new List<CoroutineTask>(16);

            _holder = new GameObject(string.Format("Pool_{0}({1})", ++_index, name ?? "Unnamed"));
            _holder.AddComponent<CoroutinePoolBehaviour>().Pool = this;
            _holder.transform.SetParent(PoolParent.transform);

            _onTaskFinished = OnTaskFinished;
        }

        public void Destroy() {
            foreach (var task in TasksRunning) {
                var runner = CoroutineList[task.RunnerId];
                if (!runner) {
                    continue;
                }
                runner.CoStop();
                Object.Destroy(runner.gameObject);
            }
            TasksRunning.Clear();
            TasksWaiting.Clear();

            if (_holder) {
                Object.Destroy(_holder);
            }
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
                runner.CoStop();

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
        }

        private CoroutineRunnerBehaviour GetOrSpawnRunner(int runnerId) {
            if (runnerId < 0 || runnerId >= Capacity) {
                throw new IndexOutOfRangeException(string.Format("Runner id must between [0, {0})", Capacity));
            }
            if (runnerId < CoroutineList.Count) {
                return CoroutineList[runnerId];
            }

            var idx = CoroutineList.Count;
            while (idx <= runnerId) {
                var runner = new GameObject("Coroutine_" + idx).AddComponent<CoroutineRunnerBehaviour>();
                runner.transform.SetParent(_holder.transform);
                runner.RunnerId = idx;
                runner.OnFinished = _onTaskFinished;
                CoroutineList.Add(runner);
                ++idx;
            }

            return CoroutineList[runnerId];
        }

        private int FindIdleRunner() {
            foreach (var slot in IdleSlots) {
                IdleSlots.Remove(slot);
                return slot;
            }

            if (TasksRunning.Count < Capacity) {
                return TasksRunning.Count;
            }

            throw new IndexOutOfRangeException("No idling runner now.");
        }

        private void OnTaskFinished(CoroutineTask task) {
            if (!IdleSlots.Add(task.RunnerId)) {
                throw new CoroutineRunnerExistInIdleListException(
                    string.Format("Runner(id: {0}) exist in idle list.", task.RunnerId));
            }

            if (!TasksRunning.Remove(task)) {
                throw new CoroutinePoolException(
                    string.Format("Task does not exist in running list, runnerId: {0}, handle: {1}", task.RunnerId, task.Handle));
            }
        }

        private bool TryPopupAndRunNext() {
            if (TasksWaiting.Count <= 0 || TasksRunning.Count >= Capacity)
                return false;

            var context = TasksWaiting[0];
            TasksWaiting.RemoveAt(0);

            RunTask(context);
            return true;
        }

        internal void OnUpdate() {
            while (TryPopupAndRunNext()) {
                // nothing to do
            }
        }
    }
}