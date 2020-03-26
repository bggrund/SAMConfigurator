using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SAMConfig
{
   /// <summary>
   /// Interaction logic for PNDDEF_Config.xaml
   /// </summary>
   public partial class PNDDEF_Config : Window
   {
      PNDATR_Config_ViewModel viewModel;
      public PNDDEF_Config(Window owner)
      {
         InitializeComponent();
         Owner = owner;
         viewModel = new PNDATR_Config_ViewModel(FileType.PND);
         DataContext = viewModel;
      }

      private void Window_Loaded(object sender, RoutedEventArgs e)
      {
         //Add FieldName column
         Binding nameColBinding = new Binding("FieldName");
         nameColBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

         DataGridTextColumn nameCol = new DataGridTextColumn();
         nameCol.Binding = nameColBinding;
         nameCol.Header = "Field Name";

         pnddefConfigGrid.Columns.Add(nameCol);

         //Add Description column
         Binding descColBinding = new Binding("Description");
         descColBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

         DataGridTextColumn descCol = new DataGridTextColumn();
         descCol.Binding = descColBinding;
         descCol.Header = "Description";

         Style s = new Style(typeof(TextBox));
         s.Setters.Add(new Setter(TextBox.TextWrappingProperty, TextWrapping.Wrap));
         s.Setters.Add(new Setter(TextBox.AcceptsReturnProperty, true));
         descCol.EditingElementStyle = s;

         pnddefConfigGrid.Columns.Add(descCol);

         //Add Type column
         Binding typeColBinding = new Binding("Type");
         typeColBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

         DataGridComboBoxColumn typeCol = new DataGridComboBoxColumn();
         typeCol.SelectedValueBinding = typeColBinding;
         typeCol.ItemsSource = new string[] { "Text", "Bool", "Combo", "OPC Item" };
         typeCol.Header = "Type";

         pnddefConfigGrid.Columns.Add(typeCol);

         //Add ComboValues column
         Binding valsColBinding = new Binding("ComboValues");
         valsColBinding.Converter = new StringArrayToStringConverter();
         valsColBinding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;

         DataGridTextColumn valsCol = new DataGridTextColumn();
         valsCol.Binding = valsColBinding;
         valsCol.Header = "Combo Values";

         pnddefConfigGrid.Columns.Add(valsCol);
      }

      private void DataGridCell_Selected(object sender, RoutedEventArgs e)
      {
         if (e.OriginalSource.GetType() == typeof(DataGridCell))
         {
            ((DataGrid)sender).BeginEdit(e);
         }
      }

      private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
      {
         viewModel.SaveData();
      }
      private void Add_Click(object sender, RoutedEventArgs e)
      {
         Events.Remove_Click(sender, e);
      }
      private void Remove_Click(object sender, RoutedEventArgs e)
      {
         Events.Remove_Click(sender, e);
      }
   }
}
