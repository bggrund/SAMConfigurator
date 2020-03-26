using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace SAMConfig
{
   public class PCKPRKValidationRules : ValidationRule
   {
      public override ValidationResult Validate(object value, CultureInfo cultureInfo)
      {
         List<object> record = ((BindingGroup)value).Items[0] as List<object>;

         if (string.IsNullOrWhiteSpace((string)record[0]) || string.IsNullOrWhiteSpace((string)record[1]))
         {
            return new ValidationResult(false, "One or more fields is empty");
         }

         List<string> locIDStrings = GetLocationIDStringsFromRecord(record);

         foreach(string locIDString in locIDStrings)
         {

            if (!int.TryParse(locIDString, out int val))
            {
               return new ValidationResult(false, "One or more locations contains non-numeric characters");
            }

            bool locFound = false;
            foreach (int sourceID in PNDATR_ViewModel.LocationIDs)
            {
               if(Math.Abs(val) == sourceID)
               {
                  locFound = true;
                  break;
               }
            }
            if(!locFound)
            {
               return new ValidationResult(false, "One or more locations does not exist in PNDDEF.DAT");
            }
         }

         return ValidationResult.ValidResult;
      }

      private List<string> GetLocationIDStringsFromRecord(List<object> record)
      {
         List<string> locIDStrings = new List<string>();
         locIDStrings.Add((string)record[0]);

         foreach (string s in ((string)record[1]).Split(','))
         {
            locIDStrings.Add(s);
         }

         return locIDStrings;
      }
   }
}
