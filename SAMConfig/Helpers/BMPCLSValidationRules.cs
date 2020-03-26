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
   public class BMPCLSValidationRules : ValidationRule
   {
      public override ValidationResult Validate(object value, CultureInfo cultureInfo)
      {
         List<object> record = ((BindingGroup)value).Items[0] as List<object>;

         if(int.TryParse((string)record[0], out int val) && val == 0)
         {
            return new ValidationResult(false, "From Loc cannot be 0");
         }

         foreach(object o in record)
         {
            if(string.IsNullOrWhiteSpace((string)o))
            {
               return new ValidationResult(false, "One or more fields is empty");
            }
            
            if (!int.TryParse((string)o, out val))
            {
               return new ValidationResult(false, "One or more fields contains non-numeric characters");
            }

            bool locFound = false;
            foreach (int sourceID in PNDATR_ViewModel.LocationIDs)
            {
               if (Math.Abs(val) == sourceID || val == 0)
               {
                  locFound = true;
                  break;
               }
            }
            if (!locFound)
            {
               return new ValidationResult(false, "One or more locations does not exist in PNDDEF.DAT");
            }
         }
         
         return ValidationResult.ValidResult;
      }
   }
}
