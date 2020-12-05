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
    internal class CoroutineBehaviour : MonoBehaviour
    {
        private IEnumerator _task;

        [SerializeField]
        private CoroutineState _state;

        public Action OnFinished;

        public void Pause() {
            _state |= CoroutineState.Paused;
        }

        public void UnPause() {
            _state &= ~CoroutineState.Paused;
        }

        public void CoStart(IEnumerator task) {
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
            _task = null;
        }

        public IEnumerator RunTask() {
            var task = _task;
            while (IsRunning()) {
                if (IsStopped()) {
                    break;
                }

                if (IsPause()) {
                    yield return null;
                }

                if (task != null && task.MoveNext()) {
                    yield return task.Current;
                }
                else {
                    _state |= CoroutineState.Finished;
                    break;
                }
            }

            CoStop();

            if (null != OnFinished) {
                OnFinished();
            }
        }
    }
}