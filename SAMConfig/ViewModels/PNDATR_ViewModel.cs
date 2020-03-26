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
   public class PNDATR_ViewModel : BaseViewModel
   {
      public List<string> Types { get; set; }
      public List<List<string>> ComboValues { get; set; }
      public static List<int> LocationIDs { get; set; }

      public PNDATR_ViewModel(FileType fileType) : base(fileType)
      {
         InitializeBaseData();
      }

      public void InitializeBaseData()
      {
         // Initialize

         Descriptions = new List<string>();
         Headers = new List<string>();
         Data = new ObservableCollection<List<object>>();
         Types = new List<string>();
         ComboValues = new List<List<string>>();

         // Read Data
         List<Field> fields = new List<Field>(fileType == FileType.PND ? MainDataModel.ReadPNDDEFConfig() : MainDataModel.ReadATRDEFConfig());

         foreach (Field f in fields)
         {
            Headers.Add(f.FieldName);
            Descriptions.Add(f.Description);
            Types.Add(f.Type);
            ComboValues.Add(f.ComboValues);
         }

         List<List<string>> records = fileType == FileType.PND ? MainDataModel.ReadPNDDEF() : MainDataModel.ReadATRDEF();

         foreach (List<string> strData in records)
         {
            if (strData.Count > Headers.Count)
            {
               for (int i = Headers.Count; i < strData.Count; i++)
               {
                  Headers.Add($"Unknown{i}");
                  Descriptions.Add($"Unknown{i}");
                  Types.Add("Text");
               }
            }

            List<object> dataObjs = new List<object>(strData);

            for (int i = 0; i < strData.Count; i++)
            {
               if (string.Equals(Types[i], "bool", StringComparison.OrdinalIgnoreCase))
               {
                  dataObjs[i] = string.Equals(strData[i], "1", StringComparison.OrdinalIgnoreCase);
               }
            }
            Data.Add(dataObjs);
         }

         if (Data.Count == 0)
         {
            List<object> dummyRecord = new List<object>();

            for (int i = 0; i < Headers.Count; i++)
            {
               dummyRecord.Add(null);
            }

            Data.Add(dummyRecord);
         }

         Data.CollectionChanged += (s, e) => UpdateLocationIDs();

         if (fileType == FileType.PND)
         {
            LocationIDs = new List<int>();
            UpdateLocationIDs();
         }
      }

      public void UpdateLocationIDs()
      {
         LocationIDs.Clear();
         foreach (List<object> strList in Data)
         {
            if (string.IsNullOrWhiteSpace((string)strList[0])) continue;
            int locID = int.Parse((string)strList[0]);
            if (!LocationIDs.Contains(locID))
            {
               LocationIDs.Add(locID);
            }
         }
      }

      public void SaveFile()
      {
         int[] maxLengths = new int[Headers.Count];

         for(int i = 0; i < Headers.Count; i++)
         {
            maxLengths[i] = Headers[i].Length;
         }

         // Get data into unformatted lines
         List<List<string>> unformattedLines = new List<List<string>>();

         foreach(List<object> record in Data)
         {
            List<string> strList = new List<string>();

            foreach(object obj in record)
            {
               if(obj is bool)
               {
                  strList.Add((bool)obj ? "1" : "0");
               }
               else if(obj is string)
               {
                  strList.Add((string)obj);
               }
               else
               {
                  strList.Add("0");
               }
            }

            for (int i = strList.Count; i < Headers.Count; i++)
            {
               strList.Add("0");
            }

            unformattedLines.Add(strList);
         }

         // Find max lengths
         foreach(List<string> record in unformattedLines)
         {
            for (int i = 0; i < record.Count; i++)
            {
               if(record[i].Length > maxLengths[i])
               {
                  maxLengths[i] = record[i].Length;
               }
            }
         }

         maxLengths[0]++;  // First item should have 1 additional space to line up with header '#' character

         // Format data

         List<string> formattedLines = new List<string>();
         
         for(int i = 0; i < unformattedLines.Count; i++)
         {
            formattedLines.Add("");
            for(int j = 0; j < maxLengths.Length; j++)
            {
               formattedLines[i] += unformattedLines[i][j].PadLeft(maxLengths[j]) + ',';
            }

            formattedLines[i] = formattedLines[i].TrimEnd(',') + '/';
         }

         string headerLine = "#";
         for(int i =  0; i < Headers.Count; i++)
         {
            headerLine += Headers[i].PadLeft(maxLengths[i] - 1) + ',';
         }

         headerLine = headerLine.TrimEnd(',') + '/';

         formattedLines.Insert(0, headerLine);

         if (fileType == FileType.PND) MainDataModel.SavePNDDEF(formattedLines);
         else MainDataModel.SaveATRDEF(formattedLines);
      }
   }
}
