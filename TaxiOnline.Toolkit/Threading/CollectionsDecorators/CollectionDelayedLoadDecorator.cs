using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using TaxiOnline.Toolkit.Threading.Lock;
using TaxiOnline.Toolkit.Events;
using System.Runtime.CompilerServices;

namespace TaxiOnline.Toolkit.Threading.CollectionsDecorators
{
    /// <summary>
    /// класс предназначен для отложенной асинхронной загрузки модифицируемой коллекции
    /// </summary>
    /// <typeparam name="TItem">тип элемента коллекции</typeparam>
    public class CollectionDelayedLoadDecorator<TItem> : DelayedLoadDecorator<TItem>
    {
        private readonly object _replaceLocker = new object();
        private readonly ReadWriteBox _collectionLocker = new ReadWriteBox();
        private volatile bool _collectionIsModified;
        private volatile TItem[] _itemsArray;

        /// <summary>
        /// получить коллекцию элементов с полным запросом их актуального состояния. Исключает одновременную модификацию коллекции
        /// </summary>
        public override ObservableCollection<TItem> Items
        {
            get
            {
                IDisposable safeUsageOperation = EnterSafeUsage();
                if (safeUsageOperation == null)
                    return base.Items;
                using (safeUsageOperation)
                using (_collectionLocker.EnterReadLock())
                    return base.Items;
            }
        }

        /// <summary>
        /// ускоренное получение получение текущих элементов до последней модификации. Не исключает одновременную модификацию
        /// </summary>
        public ReadOnlyCollection<TItem> CurrentItems
        {
            get
            {
                TItem[] items = GetItems();
                return items == null ? null : new ReadOnlyCollection<TItem>(items);
            }
        }

        /// <summary>
        /// информация об изменении коллекции
        /// </summary>
        public event NotifyCollectionChangedEventHandler ItemsCollectionChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestDelegate">функция для запроса коллекции, вызываемая асинхронно по запросу</param>
        public CollectionDelayedLoadDecorator(Func<IEnumerable<TItem>> requestDelegate)
            : base(requestDelegate)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wrappedRequestDelegate">функция для запроса коллекции, вызываемая асинхронно по запросу</param>
        public CollectionDelayedLoadDecorator(Func<ActionResult<IEnumerable<TItem>>> wrappedRequestDelegate)
            : base(wrappedRequestDelegate)
        {

        }

        /// <summary>
        /// добавить элемент в коллекцию
        /// </summary>
        /// <param name="item">добавляемый элемент</param>
        public void Add(TItem item)
        {
            UpdateCollection(() => _items.Add(item));
        }

        /// <summary>
        /// добавить набор элементов в коллекцию
        /// </summary>
        /// <param name="items">добавляемые элементы</param>
        public void AddRange(IEnumerable<TItem> items)
        {
            UpdateCollection(() =>
            {
                foreach (TItem item in items)
                    _items.Add(item);
            });
        }

        /// <summary>
        /// удалить элемент из коллекции
        /// </summary>
        /// <param name="item">удаляемый элемент</param>
        public void Remove(TItem item)
        {
            UpdateCollection(() => _items.Remove(item));
        }

        /// <summary>
        /// удалить набор элементов из коллекции
        /// </summary>
        /// <param name="items">удаляемые элементы</param>
        public void RemoveRange(IEnumerable<TItem> items)
        {
            UpdateCollection(() =>
            {
                foreach (TItem item in items)
                    _items.Remove(item);
            });
        }

        protected virtual void OnItemsChanged(NotifyCollectionChangedEventArgs args)
        {
            NotifyCollectionChangedEventHandler handler = ItemsCollectionChanged;
            if (handler != null)
                handler(this, args);
        }

        protected override void DisposeUnmanagedResources()
        {
            base.DisposeUnmanagedResources();
            _collectionLocker.Dispose();
        }

        protected override void FillListCore()
        {
            lock (_replaceLocker)
            {
                UpdateBeforeReplace();
                base.FillListCore();
                UpdateAfterReplace();
            }
        }

        protected override ActionResult WrappedFillListCore()
        {
            ActionResult outResult;
            lock (_replaceLocker)
            {
                UpdateBeforeReplace();
                outResult = base.WrappedFillListCore();
                UpdateAfterReplace();
            }
            return outResult;
        }

#if DOTNET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void UpdateBeforeReplace()
        {
            if (_items != null)
                _items.CollectionChanged -= Items_CollectionChanged;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateAfterReplace()
        {
            if (_items != null)
                _items.CollectionChanged += Items_CollectionChanged;
            IDisposable safeUsageOperation = EnterSafeUsage();
            if (safeUsageOperation == null)
                return;
            using (safeUsageOperation)
            using (_collectionLocker.EnterWriteLock())
                _collectionIsModified = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateCollection(Action updateDelegate)
        {
            IDisposable safeUsageOperation = EnterSafeUsage();
            if (safeUsageOperation == null)
                return;
            using (safeUsageOperation)
                lock (_replaceLocker)
                {
                    if (_items != null)
                        using (_collectionLocker.EnterWriteLock())
                        {
                            _collectionIsModified = true;
                            updateDelegate();
                        }
                }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TItem[] GetItems()
        {
            ObservableCollection<TItem> items = _items;
            if (items == null)
                return null;
            IDisposable safeUsageOperation = EnterSafeUsage();
            if (safeUsageOperation == null)
                return null;
            using (safeUsageOperation)
                if (_collectionIsModified)
                    using (_collectionLocker.EnterReadLock())
                    {
                        _itemsArray = items.ToArray();
                        _collectionIsModified = false;
                    }
            return _itemsArray;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnItemsChanged(e);
        }
    }
}
