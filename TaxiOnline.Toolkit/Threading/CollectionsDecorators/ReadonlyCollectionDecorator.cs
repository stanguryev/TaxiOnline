using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.Toolkit.Collections.Special;
using TaxiOnline.Toolkit.Patterns;
using TaxiOnline.Toolkit.Threading.Lock;

namespace TaxiOnline.Toolkit.Threading.CollectionsDecorators
{
    public class ReadonlyCollectionDecorator<TItem> : DisposableObject
    {
        protected readonly ReadWriteBox _itemsLocker = new ReadWriteBox();
        protected volatile bool _itemsAreModified;
        protected volatile ReadOnlyCollection<TItem> _stableItems;
        protected volatile ObservableCollection<TItem> _items = new ObservableCollection<TItem>();
        private RegistryBase<TItem> _registry;

        /// <summary>
        /// текущее состояние коллекции
        /// </summary>
        public ReadOnlyCollection<TItem> Items
        {
            get { return GetItems(); }
        }

        /// <summary>
        /// изменения в коллекции
        /// </summary>
        public event NotifyCollectionChangedEventHandler ItemsCollectionChanged;

        public ReadonlyCollectionDecorator()
        {
            _items.CollectionChanged += Items_CollectionChanged;
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

        public void AddToRegistry(RegistryBase<TItem> registry)
        {
            _registry = registry;
        }

        public void ModifyCollection(Action<IList<TItem>> modificationDelegate)
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

        protected virtual void OnItemsCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler handler = ItemsCollectionChanged;
            if (handler != null)
                handler(this, e);
        }
    }
}
