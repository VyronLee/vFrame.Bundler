using System.Collections.Generic;
using UnityEngine;

namespace vFrame.Bundler.Base.Coroutine
{
    internal class CoroutinePoolBehaviour : MonoBehaviour
    {
        [SerializeField]
        private int _capacity;
        [SerializeField]
        private List<CoroutineTask> _tasksRunning;
        [SerializeField]
        private List<CoroutineTask> _tasksWaiting;
        [SerializeField]
        private List<CoroutineBehaviour> _coroutineList;

        public CoroutinePool Pool {
            set {
                _capacity = value.Capacity;
                _coroutineList = value.CoroutineList;
                _tasksRunning = value.TasksRunning;
                _tasksWaiting = value.TasksWaiting;
            }
        }
    }
}