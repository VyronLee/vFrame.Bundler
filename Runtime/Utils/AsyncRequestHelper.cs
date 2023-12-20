using System;
using System.Collections;
using vFrame.Bundler.Base.Coroutine;
using vFrame.Bundler.Exception;
using vFrame.Bundler.Interface;
using Logger = vFrame.Bundler.Logs.Logger;

namespace vFrame.Bundler.Utils
{
    internal static class AsyncRequestHelper
    {
        public static void Setup(CoroutinePool pool, IAsyncProcessor processor) {
            if (null == pool) {
                throw new ArgumentNullException("pool");
            }
            if (processor.IsSetup) {
                throw new AsyncRequestAlreadySetupException("Async processor cannot setup twice!");
            }
            processor.IsSetup = true;
            processor.ThreadHandle = pool.StartCoroutine(ProcessAsync(processor));
            Logger.LogVerbose("Coroutine setup, processor: {0}, handle: {1}", processor, processor.ThreadHandle);
        }

        public static void Uninstall(CoroutinePool pool, IAsyncProcessor processor) {
            if (null == pool) {
                throw new ArgumentNullException("pool");
            }
            if (processor.ThreadHandle > 0) {
                pool.StopCoroutine(processor.ThreadHandle);
            }
            processor.ThreadHandle = 0;
            processor.IsSetup = false;
        }

        private static IEnumerator ProcessAsync(IAsyncProcessor processor) {
            Logger.LogVerbose("ProcessAsync begin: {0}", processor);
            yield return processor.OnAsyncProcess();
            processor.ThreadHandle = 0;
            Logger.LogVerbose("ProcessAsync end: {0}", processor);
        }
    }
}