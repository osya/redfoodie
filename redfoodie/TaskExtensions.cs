using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace redfoodie
{
    /// <summary>
    /// Copied from Microsoft.AspNet.Identity.TaskExtensions to be used with overrided PasswordSignInAsync in ApplicationSignInManager
    /// </summary>
    internal static class TaskExtensions
    {
        public static CultureAwaiter<T> WithCurrentCulture<T>(this Task<T> task)
        {
            return new CultureAwaiter<T>(task);
        }

        public static CultureAwaiter WithCurrentCulture(this Task task)
        {
            return new CultureAwaiter(task);
        }

        public struct CultureAwaiter<T> : ICriticalNotifyCompletion
        {
            private readonly Task<T> _task;

            public CultureAwaiter(Task<T> task)
            {
                _task = task ?? throw new ArgumentNullException(nameof(task));
            }

            public CultureAwaiter<T> GetAwaiter()
            {
                return this;
            }

            public bool IsCompleted => _task.IsCompleted;

            public T GetResult()
            {
                if (_task == null) throw new ArgumentNullException(nameof(_task));
                return _task.GetAwaiter().GetResult();
                
            }

            public void OnCompleted(Action continuation)
            {
                throw new NotImplementedException();
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                var currentCulture = Thread.CurrentThread.CurrentCulture;
                var currentUiCulture = Thread.CurrentThread.CurrentUICulture;
                _task.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(() =>
                {
                    var currentCulture1 = Thread.CurrentThread.CurrentCulture;
                    var currentUiCulture1 = Thread.CurrentThread.CurrentUICulture;
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                    Thread.CurrentThread.CurrentUICulture = currentUiCulture;
                    try
                    {
                        continuation();
                    }
                    finally
                    {
                        Thread.CurrentThread.CurrentCulture = currentCulture1;
                        Thread.CurrentThread.CurrentUICulture = currentUiCulture1;
                    }
                });
            }
        }

        public struct CultureAwaiter : ICriticalNotifyCompletion
        {
            private readonly Task _task;

            public bool IsCompleted => _task.IsCompleted;

            public CultureAwaiter(Task task)
            {
                _task = task;
            }

            public CultureAwaiter GetAwaiter()
            {
                return this;
            }

            public void GetResult()
            {
                _task.GetAwaiter().GetResult();
            }

            public void OnCompleted(Action continuation)
            {
                throw new NotImplementedException();
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                var currentCulture = Thread.CurrentThread.CurrentCulture;
                var currentUiCulture = Thread.CurrentThread.CurrentUICulture;
                _task.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(() =>
                {
                    var currentCulture1 = Thread.CurrentThread.CurrentCulture;
                    var currentUiCulture1 = Thread.CurrentThread.CurrentUICulture;
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                    Thread.CurrentThread.CurrentUICulture = currentUiCulture;
                    try
                    {
                        continuation();
                    }
                    finally
                    {
                        Thread.CurrentThread.CurrentCulture = currentCulture1;
                        Thread.CurrentThread.CurrentUICulture = currentUiCulture1;
                    }
                });
            }
        }
    }
}