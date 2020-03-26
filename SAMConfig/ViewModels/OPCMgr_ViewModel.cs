using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
   public class OPCMgr_ViewModel
   {
      public List<string> GroupHeaders { get; set; }
      public List<string> GroupDescriptions { get; set; }
      public List<string> ItemHeaders { get; set; }
      public List<string> ItemDescriptions { get; set; }
      public string[] TypeValues { get; set; }
      public string[] AccessValues { get; set; }
      public ObservableCollectionWithPropertyChanged<OPCGroup> Groups { get; set; }
      public ObservableCollection<string> GroupNames { get; set; }
      public ObservableCollectionWithPropertyChanged<OPCItem> Items { get; set; }
      public ObservableCollection<string> ItemIDs { get; set; }   // Shifted 1 item from Items (always contains "NONE" item)
      private OPCConnection OPCConnection;
      public OPCMgr_ViewModel()
      {
         InitializeBaseData();
      }

      public void InitializeBaseData()
      {
         // Initialize

         #region Headers and Combo Values
         GroupHeaders = new List<string>() { "Name", "Write Together", "Update Rate", "Trace Mask", "Active" };
         GroupDescriptions = new List<string>()
         {
            "The name of the group. Must be unique per OPC server.",

            "\"yes\" indicates that when any item in the group changes," + Environment.NewLine +
            "all items in the group are to be written." + Environment.NewLine +
            "(optional; default is \"no\", i.e., each item is independent)",

            "The rate at which the items are polled, in ms." + Environment.NewLine +
            "(optional; default is \"0\", i.e., the fastest reasonable rate)",

            "log (halwr) reads, writes, and changes. (Ex: trace_mask=\"RWC\")",

            "The active state of the group. Active items are updated" + Environment.NewLine +
            "by the OPC server automatically, and we are notified of their" + Environment.NewLine +
            "changes. (optional; default is \"1\")"
         };

         ItemHeaders = new List<string>() { "Group", "Access Path", "ID", "Name", "Type", "Access", "Code", "Active" };
         ItemDescriptions = new List<string>()
         {
            "The group this item belongs to (see above)",

            "The path that the OPC server uses to find the physical device.",

            "The address on that device that this item refers to.",

            "Only used for logging; has no meaning to OPC.",

            "Used to determine if byte swapping is necessary; has no meaning to OPC." + Environment.NewLine +
            "(optional; must be \"default\" or \"string\".",

            "Used to restrict writing OPC items if they are inputs. Some OPC items" + Environment.NewLine +
            "(e.g., Opto inputs) can be written to without error, but then the GLOBAL" + Environment.NewLine +
            "representation no longer matches the device inputs. Value means read only" + Environment.NewLine +
            "(read), write only (write) or read and write (modify) permissions. (optional;" + Environment.NewLine +
            "must be \"read\", \"write\", or \"modify\". Default is \"modify\".",

            "Used to generate a mapping between OPC items and claimed/released mux in" + Environment.NewLine +
            "the system databases; has no meaning to OPC. Should be a decimal or hex" + Environment.NewLine +
            "value (if hex, prefix with \"0x\"). Optional.",

            "The active state of the item. This is only considered" + Environment.NewLine +
            "when the active state of the group is \"1\", and may be used to" + Environment.NewLine +
            "designate inactive items in an active group. (optional;" + Environment.NewLine +
            "default is \"1\")"
         };

         TypeValues = new string[] { "default", "string" };
         AccessValues = new string[] { "read", "write", "modify" };

         #endregion

         GroupNames = new ObservableCollection<string>();
         ItemIDs = new ObservableCollection<string>();
         ItemIDs.Add("NONE");

         OPCConnection = MainDataModel.ReadOPCMGR();

         Groups = new ObservableCollectionWithPropertyChanged<OPCGroup>(OPCConnection.Groups);
         foreach (OPCGroup g in Groups)
         {
            GroupNames.Add(g.name);
         }

         Groups.CollectionChanged += (s, e) =>
         {
            if(e.Action == NotifyCollectionChangedAction.Remove)
            {
               GroupNames.RemoveAt(e.OldStartingIndex);
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
               GroupNames.Insert(e.NewStartingIndex, Groups[e.NewStartingIndex].name);
            }
            else // Property Changed
            {
               int index = ((NotifyCollectionChangedEventArgs_WithPropertyChanged)e).Index;
               GroupNames[index] = Groups[index].name;
            }
         };

         Items = new ObservableCollectionWithPropertyChanged<OPCItem>();
         foreach(OPCGroup g in Groups)
         {
            foreach(OPCItem i in g.Items)
            {
               i.group = g.name;
               Items.Add(i);
               ItemIDs.Add(i.id);
            }
         }

         Items.CollectionChanged += (s, e) =>
         {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
               ItemIDs.RemoveAt(e.OldStartingIndex + 1);
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
               ItemIDs.Insert(e.NewStartingIndex + 1, Items[e.NewStartingIndex + 1].id);
            }
            else // Property Changed
            {
               int index = ((NotifyCollectionChangedEventArgs_WithPropertyChanged)e).Index;
               ItemIDs[index + 1] = Items[index].id;
            }
         };
      }

      public void SaveFile()
      {
         MainDataModel.SaveOPCMGR(OPCConnection);
      }

      public void DuplicateItem(int index)
      {
         if (index >= 0 && index == Items.Count)
         {
            Items.Add(new OPCItem());
         }
         else
         {
            Items.Insert(index + 1, new OPCItem()
            {
               access_path = Items[index].access_path,
               active = Items[index].active,
               id = Items[index].id,
               name = Items[index].name,
               type = Items[index].type,
               access = Items[index].access,
               code = Items[index].code,
               group = Items[index].group,
            });
         }
      }
      public void AddItem(int index)
      {
         if (index >= 0 && index < Items.Count)
         {
            Items.Insert(index + 1, new OPCItem());
         }
      }
      public void RemoveItem(int index)
      {
         if (index >= 0 && index < Items.Count)
         {
            Items.RemoveAt(index);
         }
      }
      public void DuplicateGroup(int index)
      {
         if (index >= 0 && index == Groups.Count)
         {
            Groups.Add(new OPCGroup());
         }
         else
         {
            Groups.Insert(index + 1, new OPCGroup()
            {
               name = Groups[index].name,
               active = Groups[index].active,
               write_together = Groups[index].write_together,
               update_rate = Groups[index].update_rate,
               trace_mask = Groups[index].trace_mask,
         });
         }
      }
      public void AddGroup(int index)
      {
         if (index >= 0 && index < Groups.Count)
         {
            Groups.Insert(index + 1, new OPCGroup());
         }
      }
      public void RemoveGroup(int index)
      {
         if (index >= 0 && index < Groups.Count)
         {
            Groups.RemoveAt(index);
         }
      }
   }
}
