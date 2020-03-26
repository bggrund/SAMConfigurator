using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace SAMConfig
{
   public class ObservableCollectionWithPropertyChanged<T> : ObservableCollection<T> where T : INotifyPropertyChanged
   {
      public ObservableCollectionWithPropertyChanged()
      {
         this.CollectionChanged += items_CollectionChanged;
      }


      public ObservableCollectionWithPropertyChanged(IEnumerable<T> collection) : base(collection)
      {
         this.CollectionChanged += items_CollectionChanged;
         foreach (INotifyPropertyChanged item in collection)
            item.PropertyChanged += item_PropertyChanged;

      }

      private void items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         if (e != null)
         {
            if(e is NotifyCollectionChangedEventArgs_WithPropertyChanged)
            {
               return;
            }
            if (e.OldItems != null)
               foreach (INotifyPropertyChanged item in e.OldItems)
                  item.PropertyChanged -= item_PropertyChanged;

            if (e.NewItems != null)
               foreach (INotifyPropertyChanged item in e.NewItems)
                  item.PropertyChanged += item_PropertyChanged;
         }
      }

      private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         NotifyCollectionChangedEventArgs_WithPropertyChanged args = new NotifyCollectionChangedEventArgs_WithPropertyChanged(Items.IndexOf((T)sender), sender);
         this.OnCollectionChanged(args);
      }
   }

   public class NotifyCollectionChangedEventArgs_WithPropertyChanged : NotifyCollectionChangedEventArgs
   {
      public int Index { get; set; }
      public NotifyCollectionChangedEventArgs_WithPropertyChanged(int index, object sender) : base(NotifyCollectionChangedAction.Move, sender, index, index)
      {
         Index = index;
      }
   }
}
