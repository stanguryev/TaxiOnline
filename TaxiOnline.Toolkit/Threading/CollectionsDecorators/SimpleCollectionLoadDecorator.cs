using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using TaxiOnline.Toolkit.Threading.Lock;
using System.Threading.Tasks;
using TaxiOnline.Toolkit.Events;
using System.Collections.Specialized;
using TaxiOnline.Toolkit.Collections.Special;
using TaxiOnline.Toolkit.Patterns;
using System.Runtime.CompilerServices;

namespace TaxiOnline.Toolkit.Threading.CollectionsDecorators
{
    /// <summary>
    /// класс предназначен для асинхронной загрузки коллекции по запросу
    /// </summary>
    /// <typeparam name="TItem">тип элемента коллекции</typeparam>
    public class SimpleCollectionLoadDecorator<TItem> : DisposableObject
    {
        protected readonly Func<ActionResult<IEnumerable<TItem>>> _loadDelegate;
        protected readonly ReadWriteBox _itemsLocker = new ReadWriteBox();
        protected readonly object _replaceItemsLocker = new object();
        protected volatile bool _itemsAreModified;
        protected volatile ReadOnlyCollection<TItem> _stableItems;
        protected volatile ObservableCollection<TItem> _items;
        private RegistryBase<TItem> _registry;

        /// <summary>
        /// текущее состояние коллекции
        /// </summary>
        public ReadOnlyCollection<TItem> Items
        {
            get { return GetItems(); }
        }

        /// <summary>
        /// коллекция заменена
        /// </summary>
        public event EventHandler ItemsChanged;

        /// <summary>
        /// произошла ошибка в асинхронном запросе
        /// </summary>
        public event EventHandler RequestFailed;

        /// <summary>
        /// изменения в коллекции
        /// </summary>
        public event NotifyCollectionChangedEventHandler ItemsCollectionChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loadDelegate">метод для загрузки коллекции</param>
        public SimpleCollectionLoadDecorator(Func<ActionResult<IEnumerable<TItem>>> loadDelegate)
        {
            _loadDelegate = loadDelegate;
        }

        /// <summary>
        /// освободить ресурсы
        /// </summary>
        protected override void DisposeUnmanagedResources()
        {
            base.DisposeUnmanagedResources();
            _itemsLocker.Dispose();
            if (_registry != null)
                _registry.Dispose();
        }

        /// <summary>
        /// заполнить коллекцию
        /// </summary>
        public void FillItemsList()
        {
            ActionResult<IEnumerable<TItem>> loadResult = _loadDelegate();
            if (!loadResult.IsValid)
            {
                OnRequestFailed();
                return;
            }
            ReplaceItemsCollection(new ObservableCollection<TItem>(loadResult.Result));
            OnItemsChanged();
        }

        public void ModifyCollection(Action<IList<TItem>> modificationDelegate)
        {
            lock (_replaceItemsLocker)
            {
                IDisposable safeUsageOperation = EnterSafeUsage();
                if (safeUsageOperation == null)
                    return;
                using (safeUsageOperation)
                    if (_items != null)
                        using (_itemsLocker.EnterWriteLock())
                        {
                            _itemsAreModified = true;
                            modificationDelegate(_items);
                        }
            }
        }

        public void AddToRegistry(RegistryBase<TItem> registry)
        {
            _registry = registry;
        }

        protected virtual void OnItemsChanged()
        {
            EventHandler handler = ItemsChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnRequestFailed()
        {
            EventHandler handler = RequestFailed;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnItemsCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler handler = ItemsCollectionChanged;
            if (handler != null)
                handler(this, e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReplaceItemsCollection(ObservableCollection<TItem> items)
        {
            RegistryBase<TItem> registry = _registry;
            lock (_replaceItemsLocker)
            {
                if (_items != null)
                    _items.CollectionChanged -= Items_CollectionChanged;
                if (registry != null && _items != null)
                    _registry.RemoveCollectionFromTrace(_items);
                _items = items;
                if (registry != null && _items != null)
                    _registry.AddCollectionToTrace(items);
                if (items != null)
                    items.CollectionChanged += Items_CollectionChanged;
                IDisposable safeUsageOperation = EnterSafeUsage();
                if (safeUsageOperation == null)
                    return;
                using (safeUsageOperation)
                using (_itemsLocker.EnterWriteLock())
                    _itemsAreModified = true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected ReadOnlyCollection<TItem> GetItems()
        {
            ObservableCollection<TItem> items = _items;
            if (items == null)
                return null;
            IDisposable safeUsageOperation = EnterSafeUsage();
            if (safeUsageOperation == null)
                return _stableItems;
            using (safeUsageOperation)
                if (_itemsAreModified)
                    using (_itemsLocker.EnterReadLock())
                    {
                        _stableItems = new ReadOnlyCollection<TItem>(items);
                        _itemsAreModified = false;
                    }
            return _stableItems;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnItemsCollectionChanged(e);
        }
    }
}
