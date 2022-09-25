namespace System.Collections.ObjectModel
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// An extended ObservableCollection with additional features<br\>
    /// - Fast AddRange method.<br\>
    /// - Fast RemoveItem.<br\>
    /// - Replace method.<br\>
    /// </summary>   
    /// <remarks>
    ///   <para><b>History</b></para>
    ///   <list type="table">
    ///   <item>
    ///   <term><b>Author:</b></term>
    ///   <description>Marc Armbruster</description>
    ///   </item>
    ///   <item>
    ///   <term><b>Date:</b></term>
    ///   <description>Dec/15/2019</description>
    ///   </item>
    ///   <item>
    ///   <term><b>Remarks:</b></term>
    ///   <description>Provided by WPF Command Aggregator.</description>
    ///   </item>
    ///   </list>
    /// </remarks>
    public class ObservableCollectionExt<T> : ObservableCollection<T>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ObservableCollectionExt()
        {
        }

        /// <summary>
        /// Constructor with list of initial items.
        /// </summary>
        /// <param name="data">Initial items.</param>
        public ObservableCollectionExt(List<T> items) : base(items)
        {
        }

        /// <summary>
        /// Constructor with list of initial items.
        /// </summary>
        /// <param name="data">Initial items.</param>
        public ObservableCollectionExt(IEnumerable<T> items) : base(items)
        {           
        }

        /// <summary>
        /// Replaces a given item with a new one (first occurance).
        /// </summary>
        /// <param name="oldItem">The item to replace.</param>
        /// <param name="newItem">The new item.</param>
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

            foreach (var item in itemsToAdd)
            {
                this.Items.Add(item);
            }

            // Raise relevant notifications after all items are added
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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

            foreach (var item in itemsToRemove)
            {
                this.Items.Remove(item);
            }

            // Raise relevant notifications after all items are removed
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Count)));
            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Items)));
        }   
        
        /// <summary>
        /// Execute the given action for each collection item.
        /// </summary>
        /// <param name="action">Action to execute.</param>
        public void ForEach(Action<T> action)
        {
            if (action == null)
            {
                throw new InvalidOperationException("Action cannot be null");
            }

            foreach (var item in this.Items)
            {
                action.Invoke(item);
            }

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Items)));
        }

        /// <summary>
        /// Execute the given actions for each collection item.
        /// The actions will be executed in order of params array.
        /// </summary>
        /// <param name="actions">Actions to execute.</param>
        public void ForEach(params Action<T>[] actions)
        {
            if (actions == null)
            {
                throw new InvalidOperationException("Actions cannot be null");
            }

            foreach (var item in this.Items)
            {
                foreach (Action<T> action in actions)
                { 
                    action.Invoke(item); 
                }
            }

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Items)));
        }

        /// <summary>
        /// Execute the given action for each collection item that fullfills the given condition.
        /// </summary>
        /// <param name="action">Action to execute.</param>
        /// <param name="condition">The condition.</param>
        public void ForEach(Action<T> action, Predicate<T> condition)
        {
            if (action == null)
            {
                throw new InvalidOperationException("Action cannot be null");
            }

            if (condition == null)
            {
                throw new InvalidOperationException("Condition cannot be null");
            }

            foreach (var item in this.Items.Where(i => condition.Invoke(i) == true))
            {
                action.Invoke(item);
            }

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Items)));
        }

        /// <summary>
        /// Verifies if all items fullfill the given condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        public bool TrueForAll(Predicate<T> condition)
        {
            if (condition == null)
            {
                throw new InvalidOperationException("Condition cannot be null");
            }

            return this.All(i => condition.Invoke(i) == true);
        }

        /// <summary>
        /// Verifies if all items do not fullfill the given condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        public bool FalseForAll(Predicate<T> condition)
        {
            if (condition == null)
            {
                throw new InvalidOperationException("Condition cannot be null");
            }

            return this.All(i => condition.Invoke(i) == false);
        }        
    }
}