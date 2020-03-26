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
   public class ATRValidationRules : ValidationRule
   {
      public override ValidationResult Validate(object value, CultureInfo cultureInfo)
      {
         List<object> record = ((BindingGroup)value).Items[0] as List<object>;

         object idRec = record[0];

         if (string.IsNullOrWhiteSpace((string)idRec))
         {
            return new ValidationResult(false, "One or more fields is empty");
         }

         if (!int.TryParse((string)idRec, out int val))
         {
            return new ValidationResult(false, "One or more fields contains non-numeric characters");
         }

         bool locFound = false;
         foreach (int sourceID in PNDATR_ViewModel.LocationIDs)
         {
            if (Math.Abs(val) == sourceID)
            {
               locFound = true;
               break;
            }
         }
         if (!locFound)
         {
            return new ValidationResult(false, "One or more locations does not exist in PNDDEF.DAT");
         }

         return ValidationResult.ValidResult;
      }
   }
}
