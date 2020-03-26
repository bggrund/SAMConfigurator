using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using PropertyChanged;

namespace SAMConfig
{
   [AddINotifyPropertyChangedInterface]
   public class PNDATR_Config_ViewModel
   {
      public FileType pndatr;
      public ObservableCollection<Field> Fields { get; set; }

      public PNDATR_Config_ViewModel(FileType pndatr)
      {
         this.pndatr = pndatr;
         InitializeData();
      }

      public void InitializeData()
      {
         // Initialize

         Fields = new ObservableCollection<Field>(pndatr == FileType.PND ? MainDataModel.ReadPNDDEFConfig() : MainDataModel.ReadATRDEFConfig());
      }

      public void SaveData()
      {
         if (pndatr == FileType.PND) MainDataModel.SavePNDDEFConfig(Fields.ToArray());
         else MainDataModel.SaveATRDEFConfig(Fields.ToArray());
      }
      public void AddRow(int index)
      {
         if (index >= 0 && index <= Fields.Count)
         {
            Fields.Insert(index, new Field());
         }
      }
      public void RemoveRow(int index)
      {
         if (index >= 0 && index < Fields.Count)
         {
            Fields.RemoveAt(index);
         }
      }
   }
}
