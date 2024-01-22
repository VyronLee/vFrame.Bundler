// ------------------------------------------------------------
//         File: Loader.cs
//        Brief: Loader.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-2 22:24
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using vFrame.Bundler.Exception;

namespace vFrame.Bundler
{
    internal abstract class Loader : BundlerReferenceObject, ITask, IJsonSerializable
    {
        private readonly LoaderContexts _loaderContexts;

        protected Loader(BundlerContexts bundlerContexts, LoaderContexts loaderContexts) : base(bundlerContexts) {
            _loaderContexts = loaderContexts;
        }

        protected LoaderContexts LoaderContexts => _loaderContexts;

        [JsonSerializableProperty]
        public TaskState TaskState { get; private set; }

        protected override void OnDestroy() {
            Facade.GetSystem<LogSystem>().LogInfo("Loader destroyed: {0}", this);
            Stop();
        }

        public void Start() {
            if (TaskState != TaskState.NotStarted) {
                return;
            }
            TaskState = TaskState.Processing;
            OnStart();
        }

        public void Stop() {
            if (TaskState != TaskState.Processing && TaskState != TaskState.Finished) {
                return;
            }
            TaskState = TaskState.NotStarted;
            OnStop();
        }

        public void Update() {
            if (TaskState != TaskState.Processing) {
                return;
            }
            OnUpdate();
        }

        public bool IsDone => TaskState == TaskState.Finished;
        public bool IsError => TaskState == TaskState.Error;

        [JsonSerializableProperty]
        public abstract float Progress { get; }

        protected void Abort() {
            TaskState = TaskState.Error;
            Facade.GetSystem<LogSystem>().LogError("Loader abort: {0}", this);
        }

        protected void Finish() {
            TaskState = TaskState.Finished;
            Facade.GetSystem<LogSystem>().LogInfo("Loader finished: {0}", this);
        }

        protected void ThrowIfNotFinished() {
            if (TaskState != TaskState.Finished) {
                throw new BundleAssetNotReadyException($"Loader has not finished: {this}");
            }
        }

        public void ForceComplete() {
            while (true) {
                switch (TaskState) {
                    case TaskState.NotStarted:
                        Start();
                        break;
                    case TaskState.Processing:
                        Facade.GetSystem<LogSystem>().LogInfo("Force loader complete: {0}", this);
                        OnForceComplete();
                        break;
                    case TaskState.Finished:
                        return;
                    default:
                        Facade.GetSystem<LogSystem>().LogWarning(
                            "Cannot force loading because task is not processing: {0}", this);
                        return;
                }
            }
        }

        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract void OnUpdate();
        protected abstract void OnForceComplete();

        public override void Retain() {
            LoaderContexts.ParentLoader?.Retain();
            base.Retain();
        }

        public override void Release() {
            LoaderContexts.ParentLoader?.Release();
            base.Release();
        }

        public override string ToString() {
            return $"[Type: {GetType().Name}, TaskState: {TaskState}]";
        }
    }
}