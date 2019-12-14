namespace System.Collections.ObjectModel
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    public class ObservableCollectionExt<T> : ObservableCollection<T>
    {
        public ObservableCollectionExt()
        {
        }

        public ObservableCollectionExt(List<T> data) : base(data)
        {
        }

        public ObservableCollectionExt(IEnumerable<T> data) : base(data)
        {           
        }

        public void Replace(T oldItem, T newItem)
        {
            this.CheckReentrancy();

            int foundIndex = this.Items.IndexOf(oldItem);
            if (foundIndex >= 0)
            {
                this.Items[foundIndex] = newItem;

                // Raise relevant notifications after all items are added
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem));
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Items)));
            }
        }

        /// <summary>
        /// Adds multiple items without nofiy after each single item.
        /// (better performance than loops with single Add calls).
        /// </summary>
        /// <param name="itemsToAdd">The items to add.</param>
        public void AddRange(IEnumerable<T> itemsToAdd)
        {
            this.CheckReentrancy();
            
            if (itemsToAdd == null || !itemsToAdd.Any())
            {
                return;
            }

            int countBeforeAdding = this.Count;

            foreach (var item in itemsToAdd)
            {
                this.Items.Add(item);
            }

            // Raise relevant notifications after all items are added
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(itemsToAdd), countBeforeAdding));
            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Count)));
            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Items)));
        }

        /// <summary>
        /// Remove multiple items without notify after each single item.
        /// (better performance than loops with single Remove calls).
        /// </summary>
        /// <param name="itemsToRemove">The items to remove.</param>
        public void RemoveItems(IEnumerable<T> itemsToRemove)
        {
            this.CheckReentrancy();

            if (itemsToRemove == null || !itemsToRemove.Any())
            {
                return;
            }

            int countBeforeRemoving = this.Count;

            foreach (var item in itemsToRemove)
            {
                this.Items.Remove(item);
            }

            // Raise relevant notifications after all items are removed
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, new List<T>(), itemsToRemove));
            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Count)));
            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Items)));
        }        
    }
}