using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SAMConfig
{
   // Current Issues:
   //    need to handle empty files
   
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      WindowViewModel viewModel;
      public MainWindow()
      {
         InitializeComponent();
         viewModel = new WindowViewModel();
         DataContext = viewModel;
      }
      private void Window_Loaded(object sender, RoutedEventArgs e)
      {
         LoadViewModel(viewModel.PNDDEF_ViewModel);
         LoadViewModel(viewModel.ATRDEF_ViewModel);
      }

      private void LoadViewModel(PNDATR_ViewModel pndatrViewModel)
      {
         DataGrid grid = pndatrViewModel.fileType == FileType.PND ? pnddefGrid : atrdefGrid;

         for (int i = 0; i < pndatrViewModel.Headers.Count; i++)
         {
            if (string.Equals(pndatrViewModel.Types[i], "bool", StringComparison.OrdinalIgnoreCase))
            {
               Binding colBinding = new Binding($"[{i}]");
               colBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

               DataGridCheckBoxColumn col = new DataGridCheckBoxColumn();
               col.Binding = colBinding;

               col.CellStyle = (Style)FindResource("DataGridCellStyle");

               grid.Columns.Add(col);
            }
            else if (string.Equals(pndatrViewModel.Types[i], "combo", StringComparison.OrdinalIgnoreCase))
            {
               Binding colBinding = new Binding($"[{i}]");
               colBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

               DataGridComboBoxColumn col = new DataGridComboBoxColumn();
               col.SelectedItemBinding = colBinding;
               col.ItemsSource = pndatrViewModel.ComboValues[i];

               col.CellStyle = (Style)FindResource("DataGridCellStyle");

               grid.Columns.Add(col);
            }
            else if (string.Equals(pndatrViewModel.Types[i], "OPC Item", StringComparison.OrdinalIgnoreCase))
            {
               Binding colBinding = new Binding($"[{i}]");
               colBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

               DataGridComboBoxColumn col = new DataGridComboBoxColumn();
               col.SelectedItemBinding = colBinding;

               Binding itemsSourceBinding = new Binding("DataContext.OPCMgr_ViewModel.ItemIDs");
               itemsSourceBinding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Window), 1);

               Setter itemsSourceSetter = new Setter(ComboBox.ItemsSourceProperty, itemsSourceBinding);
               
               Style editingElementStyle = new Style(typeof(ComboBox));
               Style elementStyle = new Style(typeof(ComboBox));
               editingElementStyle.Setters.Add(itemsSourceSetter);
               elementStyle.Setters.Add(itemsSourceSetter);

               col.EditingElementStyle = editingElementStyle;
               col.ElementStyle = elementStyle;

               col.CellStyle = (Style)FindResource("DataGridCellStyle");

               grid.Columns.Add(col);
            }
            else // Text
            {
               Binding colBinding = new Binding($"[{i}]");
               colBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

               DataGridTextColumn col = new DataGridTextColumn();
               col.Binding = colBinding;

               grid.Columns.Add(col);
            }

            Binding b = new Binding($"DataContext.Descriptions[{i}]");
            b.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DataGrid), 1);

            Style headerStyle = new Style(typeof(DataGridColumnHeader));
            Setter headerSetter = new Setter(ToolTipProperty, b);
            headerStyle.Setters.Add(headerSetter);

            // Code to add tooltip to each cell, along with the mouseover trigger
            //Style cellStyle = new Style(typeof(DataGridCell));
            //Setter cellToolTipSetter = new Setter(ToolTipProperty, b);
            //Setter cellIsEditingSetter = new Setter(DataGridCell.IsEditingProperty, true);
            //Trigger cellMouseOverTrigger = new Trigger();
            //cellMouseOverTrigger.Property = IsMouseOverProperty;
            //cellMouseOverTrigger.Value = true;
            //cellMouseOverTrigger.Setters.Add(cellIsEditingSetter);
            //cellStyle.Setters.Add(cellToolTipSetter);
            //cellStyle.Triggers.Add(cellMouseOverTrigger);

            grid.Columns[i].HeaderStyle = headerStyle;
            //grid.Columns[i].CellStyle = cellStyle;
            grid.Columns[i].Header = pndatrViewModel.Headers[i];
         }
      }

      private void PNDDEFConfig_Click(object sender, RoutedEventArgs e)
      {
         PNDDEF_Config config = new PNDDEF_Config(this);
         config.Show();

         config.Closed += Config_Closed;
      }

      private void ATRDEFConfig_Click(object sender, RoutedEventArgs e)
      {
         ATRDEF_Config config = new ATRDEF_Config(this);
         config.Show();

         config.Closed += Config_Closed;
      }

      private void Config_Closed(object sender, EventArgs e)
      {
         MessageBox.Show("Please restart the applciation for configuration changes to take effect.", "SAM Configurator", MessageBoxButton.OK, MessageBoxImage.Information);
      }

      private void Save_Click(object sender, RoutedEventArgs e)
      {
         if(string.Equals(((TabItem)tabControl.SelectedItem).Header, "PNDDEF.DAT"))
         {
            viewModel.PNDDEF_ViewModel.SaveFile();
         }
         else if(string.Equals(((TabItem)tabControl.SelectedItem).Header, "ATRDEF.DAT"))
         {
            viewModel.ATRDEF_ViewModel.SaveFile();
         }
         else if (string.Equals(((TabItem)tabControl.SelectedItem).Header, "PCKLST.DAT"))
         {
            viewModel.PCKLST_ViewModel.SaveFile();
         }
         else if (string.Equals(((TabItem)tabControl.SelectedItem).Header, "PRKLST.DAT"))
         {
            viewModel.PRKLST_ViewModel.SaveFile();
         }
         else if (string.Equals(((TabItem)tabControl.SelectedItem).Header, "BMPAGV.DAT"))
         {
            viewModel.BMPAGV_ViewModel.SaveFile();
         }
         else if (string.Equals(((TabItem)tabControl.SelectedItem).Header, "CLSAGV.DAT"))
         {
            viewModel.CLSAGV_ViewModel.SaveFile();
         }
         else if (string.Equals(((TabItem)tabControl.SelectedItem).Header, "OPCMgr.xml"))
         {
            viewModel.OPCMgr_ViewModel.SaveFile();
         }
      }

      private void SaveAll_Click(object sender, RoutedEventArgs e)
      {
         viewModel.PNDDEF_ViewModel.SaveFile();
         viewModel.ATRDEF_ViewModel.SaveFile();
         viewModel.PCKLST_ViewModel.SaveFile();
         viewModel.PRKLST_ViewModel.SaveFile();
         viewModel.BMPAGV_ViewModel.SaveFile();
         viewModel.CLSAGV_ViewModel.SaveFile();
         viewModel.OPCMgr_ViewModel.SaveFile();
      }

      private void DataGridCell_Selected(object sender, RoutedEventArgs e)
      {
         if (e.OriginalSource is DataGridCell)
         {
            ((DataGrid)sender).BeginEdit();
            //bool b3 = (e.OriginalSource as DataGridCell).IsSelected;
            //(sender as DataGrid).CommitEdit();
            //(e.OriginalSource as DataGridCell).Focus();
            //bool b = ((DataGrid)sender).BeginEdit(e);
            //bool editing = (e.OriginalSource as DataGridCell).IsEditing;
            //if (!editing) (e.OriginalSource as DataGridCell).IsEditing = true;
         }
      }

      private void AddingNewItem(object sender, AddingNewItemEventArgs e)
      {
         if(sender is DataGrid)
         {
            DataGrid grid = (DataGrid)sender;

            if(grid.DataContext is BaseViewModel)
            {
               e.NewItem = new List<object>(new object[((BaseViewModel)((DataGrid)sender).DataContext).Headers.Count]);
            }
            else // OPCMgr
            {
               e.NewItem = new OPCGroup();
            }
         }
      }

      private void Duplicate_Click(object sender, RoutedEventArgs e)
      {
         Events.Duplicate_Click(sender, e);
      }
      private void Add_Click(object sender, RoutedEventArgs e)
      {
         Events.Add_Click(sender, e);
      }
      private void Remove_Click(object sender, RoutedEventArgs e)
      {
         Events.Remove_Click(sender, e);
      }

      private void CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
      {
         e.Row.BindingGroup?.CommitEdit();
      }
      private void PNDCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
      {
         if(e.Column.DisplayIndex == 0)
         {
            ((PNDATR_ViewModel)((DataGrid)sender).DataContext).UpdateLocationIDs();
         }
      }

      // Used for initial row validation on atrdefGrid
      private static bool selectionChangedEventFired = false;
      private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         if (selectionChangedEventFired) return;

         if (e.AddedItems.Count == 0) return;

         if (!(e.AddedItems[0] is TabItem) || !string.Equals((e.AddedItems[0] as TabItem).Header, "ATRDEF.DAT")) return;

         selectionChangedEventFired = true;
         
         DependencyObject parent = SearchChildrenForParentOfType<DataGridRow>((DependencyObject)sender);

         int children = VisualTreeHelper.GetChildrenCount(parent);
         for(int i = 0; i < children; i++)
         {
            (VisualTreeHelper.GetChild(parent, i) as DataGridRow).BindingGroup?.CommitEdit();
         }
      }

      private DependencyObject SearchChildrenForParentOfType<T>(DependencyObject reference)
      {
         if(reference is T)
         {
            return VisualTreeHelper.GetParent(reference);
         }

         DependencyObject child = null;
         int children = VisualTreeHelper.GetChildrenCount(reference);
         for(int i = 0; i < children; i++)
         {
            child = SearchChildrenForParentOfType<T>(VisualTreeHelper.GetChild(reference, i));
            if(child != null)
            {
               return child;
            }
         }

         return null;
      }

      private void DataGrid_Loaded(object sender, RoutedEventArgs e)
      {
         DependencyObject parent = SearchChildrenForParentOfType<DataGridRow>((DependencyObject)sender);

         if (parent == null) return;

         int children = VisualTreeHelper.GetChildrenCount(parent);
         for (int i = 0; i < children; i++)
         {
            (VisualTreeHelper.GetChild(parent, i) as DataGridRow).BindingGroup?.CommitEdit();
         }
      }

      private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
      {
         if (e.AddedCells.Count == 0) return;

         BaseViewModel b = (sender as DataGrid).DataContext as BaseViewModel;
         if (b != null) b.SelectedIndex = b.Data.IndexOf(e.AddedCells[0].Item as List<object>);
      }
   }
}
