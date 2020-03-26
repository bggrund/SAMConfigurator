using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SAMConfig
{
   public static class Extensions
   {
      public static T FindParent<T>(this DependencyObject child) where T : class, new()
      {
         T parent = new T();
         for (DependencyObject obj = child; obj != null; obj = VisualTreeHelper.GetParent(obj))
         {
            if (obj is T)
            {
               parent = obj as T;
               break;
            }
         }

         return parent;
      }
   }
}
