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
   public class PCKPRK_ViewModel : BaseViewModel
   {
      public int SelectedIndex { get; set; }
      public ICommand AddRowKeyCommand { get; set; }
      public PCKPRK_ViewModel(FileType fileType) : base(fileType)
      {
         InitializeBaseData();

         AddRowKeyCommand = new RelayCommand((o) => AddRow(SelectedIndex));
      }

      public void InitializeBaseData()
      {
         // Initialize

         if (fileType == FileType.PCK)
         {
            Headers = new List<string>() { "From", "Location List" };
            Descriptions = new List<string>()
            {
               "Location of a vehicle to assign to a job",

               "List of pick locations that the vehicle can be assigned to, separated by commas and ordered by priority." +
               Environment.NewLine + Environment.NewLine +
               "To specify a range, use a negative sign for the high loc (i.e. 100,-150 for locations 100 through 150)."
            };
         }
         else // PRK
         {
            Headers = new List<string>() { "From", "Location List" };
            Descriptions = new List<string>()
            {
               "Location of a vehicle to be parked",

               "List of locations that the vehicle can park at, separated by commas and ordered by priority." +
               Environment.NewLine + Environment.NewLine +
               "To specify a range, use a negative sign for the high loc (i.e. 100,-150 for locations 100 through 150)."
            };
         }

         // Read Data
         Data = new ObservableCollection<List<object>>();
         List<List<string>> strData = fileType == FileType.PCK ? MainDataModel.ReadPCKLST() : MainDataModel.ReadPRKLST();

         foreach(List<string> strList in strData)
         {
            Data.Add(new List<object>(strList));
         }

         if(Data.Count == 0)
         {
            Data.Add(new List<object>(new object[] { "0", "0" }));
         }
      }

      public void SaveFile()
      {
         // Format data

         string headerLine = "#" + Headers[0] + ',' + Headers[1] +'/';

         List<string> formattedLines = new List<string>() { headerLine };

         foreach(List<object> record in Data)
         {
            formattedLines.Add((string)record[0] + ',' + ((string)record[1]).TrimEnd().TrimEnd(',') + '/');
         }

         if (fileType == FileType.PCK) MainDataModel.SavePCKLST(formattedLines);
         else MainDataModel.SavePRKLST(formattedLines);
      }
   }
}
