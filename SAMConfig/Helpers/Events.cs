using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SAMConfig
{
   public static class Events
   {

      public static void Duplicate_Click(object sender, RoutedEventArgs e)
      {
         DataGridRow row = ((DependencyObject)sender).FindParent<DataGridRow>();
         DataGrid grid = row.FindParent<DataGrid>();

         //int rowIndex = row.GetIndex();

         //if (rowIndex < 0 || rowIndex >= grid.Items.Count - 1) return;

         object d = grid.DataContext;
         if (d is BaseViewModel)
         {
            ((BaseViewModel)d).DuplicateRow(row.GetIndex());

            // Force validation on new row
            //DataGridRow newRow = VisualTreeHelper.GetChild(VisualTreeHelper.GetParent(row), row.GetIndex() + 1) as DataGridRow;
            //newRow.BindingGroup.CommitEdit();
         }
         else // OPCMgr
         {
            if (string.Equals(grid.Name, "opcmgrGroupGrid"))
            {
               ((OPCMgr_ViewModel)d).DuplicateGroup(row.GetIndex());
            }
            else // Items Grid
            {
               ((OPCMgr_ViewModel)d).DuplicateItem(row.GetIndex());
            }
         }
      }
      public static void Add_Click(object sender, RoutedEventArgs e)
      {
         DataGridRow row = ((DependencyObject)sender).FindParent<DataGridRow>();
         DataGrid grid = row.FindParent<DataGrid>();

         //int rowIndex = row.GetIndex();

         //if (rowIndex < 0 || rowIndex >= grid.Items.Count - 1) return;

         object d = grid.DataContext;
         if (d is BaseViewModel)
         {
            ((BaseViewModel)d).AddRow(row.GetIndex());

            // Force validation on new row
            //DataGridRow newRow = VisualTreeHelper.GetChild(VisualTreeHelper.GetParent(row), row.GetIndex() + 1) as DataGridRow;
            //newRow.BindingGroup.CommitEdit();
         }
         else // OPCMgr
         {
            if (string.Equals(grid.Name, "opcmgrGroupGrid"))
            {
               ((OPCMgr_ViewModel)d).AddGroup(row.GetIndex());
            }
            else // Items Grid
            {
               ((OPCMgr_ViewModel)d).AddItem(row.GetIndex());
            }
         }
      }
      public static void Remove_Click(object sender, RoutedEventArgs e)
      {
         DataGridRow row = ((DependencyObject)sender).FindParent<DataGridRow>();
         DataGrid grid = row.FindParent<DataGrid>();

         if (grid.Items.Count == 1) return;

         object d = grid.DataContext;
         if (d is BaseViewModel)
         {
            ((BaseViewModel)d).RemoveRow(row.GetIndex());
         }
         else // OPCMgr
         {
            if (string.Equals(grid.Name, "opcmgrGroupGrid"))
            {
               ((OPCMgr_ViewModel)d).RemoveGroup(row.GetIndex());
            }
            else // Items Grid
            {
               ((OPCMgr_ViewModel)d).RemoveItem(row.GetIndex());
            }
         }
      }
   }
}
