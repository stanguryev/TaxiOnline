using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.Toolkit.Events;
using System.Collections.ObjectModel;

namespace TaxiOnline.Toolkit.Threading.CollectionsDecorators
{
    /// <summary>
    /// класс предназначен для асинхронной загрузки коллекции по запросу с возможностью отслеживания обновлений
    /// </summary>
    /// <typeparam name="TItem">тип элемента коллекции</typeparam>
    /// <typeparam name="TUpdateItem">тип элемента для обновления</typeparam>
    public class UpdatableCollectionLoadDecorator<TItem, TUpdateItem> : SimpleCollectionLoadDecorator<TItem>
    {
        private readonly Func<TItem, TUpdateItem, bool> _compareDelegate;
        private readonly Predicate<TUpdateItem> _filterDelegate;
        private readonly Func<TUpdateItem, TItem> _createDelegate;

        /// <summary>
        /// началось добавление элемента
        /// </summary>
        public event EventHandler<ValueEventArgs<TUpdateItem>> Adding;

        /// <summary>
        /// завершилось добавление элемента
        /// </summary>
        public event EventHandler<ValueEventArgs<TUpdateItem>> Added;

        /// <summary>
        /// началось удаление элемента
        /// </summary>
        public event EventHandler<ValueEventArgs<TItem>> Deleting;

        /// <summary>
        /// завершилось удаление элемента
        /// </summary>
        public event EventHandler<ValueEventArgs<TItem>> Deleted;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loadDelegate">метод для загрузки коллекции</param>
        /// <param name="compareDelegate">метод для сравнения добавляемого элемента с элементами коллекции</param>
        /// <param name="filterDelegate">метод фильтрации добавлеяемого элемента</param>
        /// <param name="createDelegate">метод создания элемента коллекции</param>
        public UpdatableCollectionLoadDecorator(Func<ActionResult<IEnumerable<TItem>>> loadDelegate, Func<TItem, TUpdateItem, bool> compareDelegate, Predicate<TUpdateItem> filterDelegate, Func<TUpdateItem, TItem> createDelegate)
            : base(loadDelegate)
        {
            _compareDelegate = compareDelegate;
            _filterDelegate = filterDelegate;
            _createDelegate = createDelegate;
        }

        /// <summary>
        /// внести изменения
        /// </summary>
        /// <param name="updateItem">элемент для изменений</param>
        public void Update(TUpdateItem updateItem)
        {
            ReadOnlyCollection<TItem> items = GetItems();
            if (items == null)
                return;
            if (_filterDelegate(updateItem))
                if (!items.Any(item => _compareDelegate(item, updateItem)))
                {
                    OnAdding(updateItem);
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
                                    _items.Add(_createDelegate(updateItem));
                                }
                    }
                    OnAdded(updateItem);
                }
            TItem itemToDelete = items.FirstOrDefault(item => _compareDelegate(item, updateItem) && !_filterDelegate(updateItem));
            if (itemToDelete != null)
            {
                OnDeleting(itemToDelete);
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
                                _items.Remove(itemToDelete);
                            }
                }
                OnDeleted(itemToDelete);
            }
        }

        protected virtual void OnAdding(TUpdateItem item)
        {
            EventHandler<ValueEventArgs<TUpdateItem>> handler = Adding;
            if (handler != null)
                handler(this, new ValueEventArgs<TUpdateItem>(item));
        }

        protected virtual void OnAdded(TUpdateItem item)
        {
            EventHandler<ValueEventArgs<TUpdateItem>> handler = Added;
            if (handler != null)
                handler(this, new ValueEventArgs<TUpdateItem>(item));
        }

        protected virtual void OnDeleting(TItem item)
        {
            EventHandler<ValueEventArgs<TItem>> handler = Deleting;
            if (handler != null)
                handler(this, new ValueEventArgs<TItem>(item));
        }

        protected virtual void OnDeleted(TItem item)
        {
            EventHandler<ValueEventArgs<TItem>> handler = Deleted;
            if (handler != null)
                handler(this, new ValueEventArgs<TItem>(item));
        }
    }
}
