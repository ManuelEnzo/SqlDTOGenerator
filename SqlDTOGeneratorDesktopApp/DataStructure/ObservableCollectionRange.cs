using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDTOGeneratorDesktopApp.DataStructure
{
    public class ObservableCollectionRange<T> : ObservableCollection<T>
    {
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            foreach (var item in collection)
            {
                Items.Add(item);
            }

            // Notify changed
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
