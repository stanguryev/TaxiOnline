using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace TaxiOnline.Toolkit.Collections.Helpers
{
    /// <summary>
    /// класс предназначен для отображения изменений одной коллекции в другую с преобразованием объектов
    /// </summary>
    public static class ObservableCollectionHelper
    {
        /// <summary>
        /// отобразить изменения в другую коллекцию элементов того же типа
        /// </summary>
        /// <typeparam name="T">тип элементов</typeparam>
        /// <param name="e">информация об изменении</param>
        /// <param name="targetCollection">коллекция, в которую будут отображены изменения</param>
        public static void ApplyChanges<T>(NotifyCollectionChangedEventArgs e, ObservableCollection<T> targetCollection)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                        targetCollection.Insert(e.NewStartingIndex + i, (T)e.NewItems[i]);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (int i = 0; i < e.OldItems.Count; i++)
                        targetCollection.RemoveAt(e.OldStartingIndex + i);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    for (int i = 0; i < e.NewItems.Count; i++)
                        targetCollection[e.NewStartingIndex + i] = (T)e.NewItems[i];
                    break;
                case NotifyCollectionChangedAction.Move:
                    targetCollection.Move(e.OldStartingIndex, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    targetCollection.Clear();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// отобразить изменения в другую коллекцию элементов того же типа
        /// </summary>
        /// <typeparam name="T">тип элементов</typeparam>
        /// <param name="e">информация об изменении</param>
        /// <param name="targetCollection">коллекция, в которую будут отображены изменения</param>
        public static void ApplyChanges<T>(NotifyCollectionChangedEventArgs e, IList<T> targetCollection)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                        targetCollection.Insert(e.NewStartingIndex + i, (T)e.NewItems[i]);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (int i = 0; i < e.OldItems.Count; i++)
                        targetCollection.RemoveAt(e.OldStartingIndex + i);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    for (int i = 0; i < e.NewItems.Count; i++)
                        targetCollection[e.NewStartingIndex + i] = (T)e.NewItems[i];
                    break;
                case NotifyCollectionChangedAction.Reset:
                    targetCollection.Clear();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static void ApplyChanges<T>(NotifyCollectionChangedEventArgs e, IList<T> targetCollection, Predicate<T> filter)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                        if (filter((T)e.NewItems[i]))
                            targetCollection.Add((T)e.NewItems[i]);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (int i = 0; i < e.OldItems.Count; i++)
                        if (filter((T)e.OldItems[i]))
                            targetCollection.Remove((T)e.OldItems[i]);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    targetCollection.Clear();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static void ApplyChangesByObjects<T>(NotifyCollectionChangedEventArgs e, IList<T> targetCollection)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                        targetCollection.Add((T)e.NewItems[i]);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (int i = 0; i < e.OldItems.Count; i++)
                        targetCollection.Remove((T)e.OldItems[i]);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    targetCollection.Clear();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// отобразить изменения в коллекцию другого типа с идентичными номерами элементов
        /// </summary>
        /// <typeparam name="TSource">тип элементов изменённой коллекции</typeparam>
        /// <typeparam name="TTarget">тип элементов коллекции, в которую будут отображены изменения</typeparam>
        /// <param name="e">информация об изменении</param>
        /// <param name="targetCollection">коллекция, в которую будут отображены изменения</param>
        /// <param name="convertionDelegate">метод для преобразования элемента исходной коллекции в элемент коллекции, в которую будут отображены изменения</param>
        public static void ApplyChangesByNumbers<TSource, TTarget>(NotifyCollectionChangedEventArgs e, ObservableCollection<TTarget> targetCollection, Func<TSource, TTarget> convertionDelegate)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                        targetCollection.Insert(e.NewStartingIndex + i, convertionDelegate((TSource)e.NewItems[i]));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (int i = 0; i < e.OldItems.Count; i++)
                        targetCollection.RemoveAt(e.OldStartingIndex + i);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    for (int i = 0; i < e.NewItems.Count; i++)
                        targetCollection[e.NewStartingIndex + i] = convertionDelegate((TSource)e.NewItems[i]);
                    break;
                case NotifyCollectionChangedAction.Move:
                    targetCollection.Move(e.OldStartingIndex, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    targetCollection.Clear();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// отобразить изменения в коллекцию другого типа с идентичными номерами элементов
        /// </summary>
        /// <typeparam name="TSource">тип элементов изменённой коллекции</typeparam>
        /// <typeparam name="TTarget">тип элементов коллекции, в которую будут отображены изменения</typeparam>
        /// <param name="e">информация об изменении</param>
        /// <param name="targetCollection">коллекция, в которую будут отображены изменения</param>
        /// <param name="convertionDelegate">метод для преобразования элемента исходной коллекции в элемент коллекции, в которую будут отображены изменения</param>
        public static void ApplyChangesByNumbers<TSource, TTarget>(NotifyCollectionChangedEventArgs e, IList<TTarget> targetCollection, Func<TSource, TTarget> convertionDelegate)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                        targetCollection.Insert(e.NewStartingIndex + i, convertionDelegate((TSource)e.NewItems[i]));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (int i = 0; i < e.OldItems.Count; i++)
                        targetCollection.RemoveAt(e.OldStartingIndex + i);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    for (int i = 0; i < e.NewItems.Count; i++)
                        targetCollection[e.NewStartingIndex + i] = convertionDelegate((TSource)e.NewItems[i]);
                    break;
                    break;
                case NotifyCollectionChangedAction.Reset:
                    targetCollection.Clear();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// отобразить изменения в коллекцию другого типа с различающимся порядком элементов
        /// </summary>
        /// <typeparam name="TSource">тип элементов изменённой коллекции</typeparam>
        /// <typeparam name="TTarget">тип элементов коллекции, в которую будут отображены изменения</typeparam>
        /// <param name="e">информация об изменении</param>
        /// <param name="targetCollection">коллекция, в которую будут отображены изменения</param>
        /// <param name="convertionDelegate">метод для преобразования элемента исходной коллекции в элемент коллекции, в которую будут отображены изменения</param>
        /// <param name="findDelegate">метод для поиска элемента коллекции, в которую будут отображены изменения, по элементу исходной коллекции</param>
        public static void ApplyChangesByObjects<TSource, TTarget>(NotifyCollectionChangedEventArgs e, IList<TTarget> targetCollection, Func<TSource, TTarget> convertionDelegate, Func<TSource, TTarget> findDelegate)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                        targetCollection.Insert(e.NewStartingIndex + i, convertionDelegate((TSource)e.NewItems[i]));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (int i = 0; i < e.OldItems.Count; i++)
                        targetCollection.Remove(findDelegate((TSource)e.OldItems[i]));
                    break;
                case NotifyCollectionChangedAction.Replace:
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        int index = targetCollection.IndexOf(findDelegate((TSource)e.OldItems[i]));
                        targetCollection[index] = findDelegate((TSource)e.NewItems[i]);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    targetCollection.Clear();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static void ApplyChangesByObjects<TSource, TUsingSource, TTarget>(NotifyCollectionChangedEventArgs e, ObservableCollection<TTarget> targetCollection, Func<TUsingSource, TTarget> convertionDelegate, Func<TUsingSource, TTarget> findDelegate)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (TUsingSource item in e.NewItems.OfType<TUsingSource>().ToArray())
                        targetCollection.Add(convertionDelegate(item));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (TUsingSource item in e.OldItems.OfType<TUsingSource>().ToArray())
                        targetCollection.Remove(findDelegate(item));
                    break;
                case NotifyCollectionChangedAction.Reset:
                    targetCollection.Clear();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
