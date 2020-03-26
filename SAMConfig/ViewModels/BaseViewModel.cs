using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using PropertyChanged;

namespace SAMConfig
{
   [AddINotifyPropertyChangedInterface]
   public abstract class BaseViewModel
   {
      public FileType fileType;
      public ObservableCollection<List<object>> Data { get; set; }
      public List<string> Headers { get; set; }
      public List<string> Descriptions { get; set; }
      public int SelectedIndex { get; set; }
      public BaseViewModel(FileType fileType)
      {
         this.fileType = fileType;
         Data = new ObservableCollection<List<object>>();
         Headers = new List<string>();
         Descriptions = new List<string>();
      }
      
      public void DuplicateRow(int index)
      {
         if(index >= 0 && index == Data.Count)
         {
            Data.Add(new List<object>(new object[Headers.Count]));
         }
         else
         {
            Data.Insert(index + 1, new List<object>(Data[index]));
         }
      }
      public void AddRow(int index)
      {
         if (index >= 0 && index < Data.Count)
         {
            Data.Insert(index + 1, new List<object>(new object[Headers.Count]));
         }
      }
      public void RemoveRow(int index)
      {
         if (index >= 0 && index < Data.Count)
         {
            Data.RemoveAt(index);
         }
      }
   }
}
