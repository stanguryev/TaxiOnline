using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections;
using TaxiOnline.Toolkit.Threading.Lock;
using System.Collections.Specialized;
using TaxiOnline.Toolkit.Collections.Helpers;
using TaxiOnline.Toolkit.Patterns;

namespace TaxiOnline.Toolkit.Collections.Special
{
    public class RegistryBase<T> : DisposableObject, IList<T>, ICollection, INotifyCollectionChanged
    {
        private readonly ReadWriteBox _readWriteLocker = new ReadWriteBox();
        private readonly ObservableCollection<T> _internalCollection = new ObservableCollection<T>();

        public int IndexOf(T item)
        {
            IDisposable safeUsageOperation = EnterSafeUsage();
            if (safeUsageOperation == null)
                return -1;
            using (safeUsageOperation)
            using (_readWriteLocker.EnterReadLock())
                return _internalCollection.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            IDisposable safeUsageOperation = EnterSafeUsage();
            if (safeUsageOperation == null)
                return;
            using (safeUsageOperation)
            using (_readWriteLocker.EnterWriteLock())
                _internalCollection.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            IDisposable safeUsageOperation = EnterSafeUsage();
            if (safeUsageOperation == null)
                return;
            using (safeUsageOperation)
            using (_readWriteLocker.EnterWriteLock())
                _internalCollection.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                IDisposable safeUsageOperation = EnterSafeUsage();
                if (safeUsageOperation == null)
                    return default(T);
                using (safeUsageOperation)
                using (_readWriteLocker.EnterReadLock())
                    return _internalCollection[index];
            }
            set
            {
                IDisposable safeUsageOperation = EnterSafeUsage();
                if (safeUsageOperation == null)
                    return;
                using (safeUsageOperation)
                using (_readWriteLocker.EnterReadLock())
                    _internalCollection[index] = value;
            }
        }

        public void Add(T item)
        {
            IDisposable safeUsageOperation = EnterSafeUsage();
            if (safeUsageOperation == null)
                return;
            using (safeUsageOperation)
            using (_readWriteLocker.EnterWriteLock())
                _internalCollection.Add(item);
        }

        public void Clear()
        {
            IDisposable safeUsageOperation = EnterSafeUsage();
            if (safeUsageOperation == null)
                return;
            using (safeUsageOperation)
            using (_readWriteLocker.EnterWriteLock())
                _internalCollection.Clear();
        }

        public bool Contains(T item)
        {
            IDisposable safeUsageOperation = EnterSafeUsage();
            if (safeUsageOperation == null)
                return false;
            using (safeUsageOperation)
            using (_readWriteLocker.EnterReadLock())
                return _internalCollection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _internalCollection.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                using (_readWriteLocker.EnterReadLock())
                    return _internalCollection.Count;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            using (_readWriteLocker.EnterWriteLock())
                return _internalCollection.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _internalCollection.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _internalCollection.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            using (_readWriteLocker.EnterWriteLock())
                _internalCollection.CopyTo(array.Cast<T>().ToArray(), index);
        }

        public bool IsSynchronized
        {
            get { return true; }
        }

        public object SyncRoot
        {
            get { return ((ICollection)_internalCollection).SyncRoot; }
        }

        protected override void DisposeUnmanagedResources()
        {
            base.DisposeUnmanagedResources();
            _readWriteLocker.Dispose();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { _internalCollection.CollectionChanged += value; }
            remove { _internalCollection.CollectionChanged -= value; }
        }

        public IDisposable EnterReadLock()
        {
            return _readWriteLocker.EnterReadLock();
        }

        public void ApplyChanges(NotifyCollectionChangedEventArgs args)
        {
            ObservableCollectionHelper.ApplyChangesByObjects<T>(args, _internalCollection);
        }

        public void AddCollectionToTrace(INotifyCollectionChanged collection)
        {
            IDisposable safeUsageOperation = EnterSafeUsage();
            if (safeUsageOperation == null)
                return;
            IEnumerable<T> enumerableCollection = collection as IEnumerable<T>;
            using (safeUsageOperation)
                if (enumerableCollection != null)
                    using (_readWriteLocker.EnterWriteLock())
                        foreach (T item in enumerableCollection)
                            _internalCollection.Add(item);
            collection.CollectionChanged += Collection_CollectionChanged;
        }

        public void RemoveCollectionFromTrace(INotifyCollectionChanged collection)
        {
            IDisposable safeUsageOperation = EnterSafeUsage();
            if (safeUsageOperation == null)
                return;
            IEnumerable<T> enumerableCollection = collection as IEnumerable<T>;
            using (safeUsageOperation)
                if (enumerableCollection != null)
                    using (_readWriteLocker.EnterWriteLock())
                        foreach (T item in enumerableCollection)
                            _internalCollection.Remove(item);
            collection.CollectionChanged -= Collection_CollectionChanged;
        }

        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IDisposable safeUsageOperation = EnterSafeUsage();
            if (safeUsageOperation == null)
                return;
            using (safeUsageOperation)
            using (_readWriteLocker.EnterWriteLock())
                ApplyChanges(e);
        }
    }
}
