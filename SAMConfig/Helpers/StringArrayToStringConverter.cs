using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SAMConfig
{
   class StringArrayToStringConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if(value == null)
         {
            return "";
         }

         string res = "";
         foreach(string s in (List<string>)value)
         {
            res += s + ',';
         }

         res = res.TrimEnd(',');

         return res;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if (string.IsNullOrWhiteSpace((string)value))
         {
            return new List<string>();
         }
         string val = ((string)value).Trim();
         val = val.TrimEnd(',');

         List<string> res = new List<string>(val.Split(','));
         for (int i = 0; i < res.Count; i++)
         {
            res[i] = res[i].Trim();
         }

         return res;
      }
   }
}
