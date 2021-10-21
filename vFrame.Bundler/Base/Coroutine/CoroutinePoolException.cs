namespace vFrame.Bundler.Base.Coroutine
{
    public class CoroutinePoolException : System.Exception
    {
        public CoroutinePoolException() {

        }

        public CoroutinePoolException(string message) : base(message) {

        }
    }

    public class CoroutinePoolInvalidStateException : CoroutinePoolException
    {
        public CoroutinePoolInvalidStateException(string message) : base(message) {

        }
    }

    public class CoroutineRunnerExistInIdleListException : CoroutinePoolException
    {
        public CoroutineRunnerExistInIdleListException(string message) : base(message) {

        }
    }
}