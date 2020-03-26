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
   public class BMPCLS_ViewModel : BaseViewModel
   {
      public BMPCLS_ViewModel(FileType fileType) : base(fileType)
      {
         InitializeBaseData();
      }

      private void InitializeBaseData()
      {
         // Initialize

         if (fileType == FileType.BMP)
         {
            Headers = new List<string>() { "From", "Loc Min", "Loc Max", "Dest Min", "Dest Max", "Route Min", "Route Max" };
            Descriptions = new List<string>()
            {
               "Location of a vehicle to be bumped",
               "Location of another vehicle to signal bump (min)",
               "Location of another vehicle to signal bump (max)",
               "Destination of another vehicle to signal bump (min)",
               "Destination of another vehicle to signal bump (max)",
               "Route of another vehicle to signal bump (min)",
               "Route of another vehicle to signal bump (max)",
            };
         }
         else
         {
            Headers = new List<string>() { "From", "Loc Min", "Loc Max", "Dest Min", "Dest Max", };
            Descriptions = new List<string>()
            {
               "Location of a vehicle to be assigned to job",
               "Location of another vehicle that is closer (min)",
               "Location of another vehicle that is closer (max)",
               "Destination of another vehicle that is closer (min)",
               "Destination of another vehicle that is closer (max)",
            };
         }

         // Read Data
         Data = new ObservableCollection<List<object>>();
         List<List<string>> strData = fileType == FileType.BMP ? MainDataModel.ReadBMPAGV() : MainDataModel.ReadCLSAGV();

         foreach(List<string> strList in strData)
         {
            Data.Add(new List<object>(strList));
         }

         if (Data.Count == 0)
         {
            Data.Add(new List<object>() { "0", "0", "0", "0", "0" });
            if (fileType == FileType.BMP) Data[0].AddRange(new object[] { "0", "0" });
         }
      }

      public void SaveFile()
      {
         // Format data

         string headerLine = "#";

         foreach(string s in Headers)
         {
            headerLine += s + ',';
         }

         headerLine = headerLine.TrimEnd(',');
         headerLine += '/';

         List<string> formattedLines = new List<string>() { headerLine };

         foreach(List<object> record in Data)
         {
            string line = "";
            foreach(string s in record)
            {
               line += s + ',';
            }

            line = line.TrimEnd(',');
            line += '/';

            formattedLines.Add(line);
         }

         for(int i = formattedLines.Count - 1; i >= 0; i--)
         {
            if (formattedLines[i][0] == '0')
            {
               formattedLines.RemoveAt(i);
            }
         }

         if (fileType == FileType.BMP) MainDataModel.SaveBMPAGV(formattedLines);
         else MainDataModel.SaveCLSAGV(formattedLines);
      }
   }
}
