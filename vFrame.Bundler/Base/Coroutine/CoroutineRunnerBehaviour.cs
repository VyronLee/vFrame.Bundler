//------------------------------------------------------------
//        File:  CoroutineBehaviour.cs
//       Brief:  Coroutine behaviour
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-09-08 22:09
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System;
using System.Collections;
using UnityEngine;

namespace vFrame.Bundler.Base.Coroutine
{
    internal class CoroutineRunnerBehaviour : MonoBehaviour
    {
        [SerializeField]
        private CoroutineTask _task;

        [SerializeField]
        private CoroutineState _state;

        [SerializeField]
        private int _runnerId;

        public Action<CoroutineTask> OnFinished = null;

        public int RunnerId {
            get { return _runnerId; }
            set { _runnerId = value; }
        }

        public int TaskHandle {
            get { return _task.Handle; }
        }

        public void Pause() {
            _state |= CoroutineState.Paused;
        }

        public void UnPause() {
            _state &= ~CoroutineState.Paused;
        }

        public void CoStart(CoroutineTask task) {
            if (IsRunning()) {
                throw new CoroutinePoolInvalidStateException("Coroutine is running, cannot start another task!");
            }

            Reset();

            _task = task;
            _state |= CoroutineState.Running;

            StartCoroutine(RunTask());
        }

        public void CoStop() {
            _state |= CoroutineState.Stopped;
            _state &= ~CoroutineState.Running;

            StopAllCoroutines();
        }

        public bool IsPause() {
            return (_state & CoroutineState.Paused) > 0;
        }

        public bool IsRunning() {
            return (_state & CoroutineState.Running) > 0;
        }

        public bool IsFinished() {
            return (_state & CoroutineState.Finished) > 0;
        }

        public bool IsStopped() {
            return (_state & CoroutineState.Stopped) > 0;
        }

        private void Reset() {
            _state = 0;
        }

        private IEnumerator RunTask() {
            var taskContext = _task;
            while (IsRunning()) {
                if (IsStopped()) {
                    break;
                }

                if (IsPause()) {
                    yield return null;
                    continue;
                }

                yield return taskContext.Task;

                _state |= CoroutineState.Finished;
                break;
            }

            _state &= ~CoroutineState.Running;
            _state |= CoroutineState.Stopped;
            _task = default(CoroutineTask);

            if (null != OnFinished) {
                OnFinished(taskContext);
            }
        }
    }
}