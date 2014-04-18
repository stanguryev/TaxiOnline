using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaxiOnline.Toolkit.Patterns;

namespace TaxiOnline.Toolkit.Threading.Patterns
{
    public class TasksPool : DisposableObject
    {
        private readonly HashSet<Task> _runningTasks = new HashSet<Task>();
        private readonly Queue<Task> _awaitingTasks = new Queue<Task>();
        private readonly EventWaitHandle _waitNewTasks = new EventWaitHandle(true, EventResetMode.AutoReset);
        private readonly Task _schedulingThread;
        private readonly CancellationTokenSource _finalizationCancellation = new CancellationTokenSource();
        private volatile bool _isActive = true;
        private int _size;
        private readonly object _schedulingLocker = new object();

        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public TasksPool(int size)
        {
            _size = size;
            _schedulingThread = new Task(SchedulingLoop);
            _schedulingThread.Start();
        }

        protected override void DisposeUnmanagedResources()
        {
            base.DisposeUnmanagedResources();
            _isActive = false;
            _finalizationCancellation.Cancel(false);
            _waitNewTasks.Set();
            _schedulingThread.Wait();
            _waitNewTasks.Dispose();
            _finalizationCancellation.Dispose();
        }

        public void QueueTask(Task task)
        {
            if (_isDisposing)
                return;
            lock (_schedulingLocker)
                _awaitingTasks.Enqueue(task);
            _waitNewTasks.Set();
        }

        private void SchedulingLoop()
        {
            while (_isActive)
            {
                while (_awaitingTasks.Count > 0 || _runningTasks.Count > 0)
                {
                    Task[] runningTasks;
                    do
                    {
                        lock (_schedulingLocker)
                            runningTasks = _runningTasks.ToArray();
                        if (_isDisposing)
                            break;
                        try
                        {
                            Task.WaitAny(runningTasks, _finalizationCancellation.Token);
                        }
                        catch (OperationCanceledException) { }
                        Cleanup();
                    }
                    while (runningTasks.Length > 0);
                    if (!_isDisposing)
                        Push();
                    else
                        break;
                }
                _waitNewTasks.WaitOne();
            }
        }

        private void Cleanup()
        {
            lock (_schedulingLocker)
                _runningTasks.RemoveWhere(t => t.Status == TaskStatus.RanToCompletion || t.Status == TaskStatus.Faulted || t.Status == TaskStatus.Canceled);
            Push();
        }

        private void Push()
        {
            int size = _size;
            lock (_schedulingLocker)
                for (int i = 0; i < size - _runningTasks.Count; i++)
                {
                    if (_awaitingTasks.Count == 0)
                        break;
                    Task addingTask = _awaitingTasks.Dequeue();
                    switch (addingTask.Status)
                    {
                        case TaskStatus.RanToCompletion:
                        case TaskStatus.Faulted:
                        case TaskStatus.Canceled:
                            i--;
                            continue;
                        case TaskStatus.Created:
                            _runningTasks.Add(addingTask);
                            addingTask.Start();
                            break;
                        case TaskStatus.Running:
                        case TaskStatus.WaitingForActivation:
                        case TaskStatus.WaitingForChildrenToComplete:
                        case TaskStatus.WaitingToRun:
                            _runningTasks.Add(addingTask);
                            break;
                        default:
                            break;
                    }
                }
        }
    }
}
