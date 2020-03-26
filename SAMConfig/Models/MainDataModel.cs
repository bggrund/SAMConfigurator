using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SAMConfig
{
   public static class MainDataModel
   {
      private static string pnddefConfigPath = @"PNDDEF.xml";
      private static string atrdefConfigPath = @"ATRDEF.xml";

      private static string pnddefPath = @"C:\SAM\Base\Server\Run\SysDat\PNDDEF.DAT";
      private static string atrdefPath = @"C:\SAM\Base\Server\Run\SysDat\ATRDEF.DAT";
      private static string pcklstPath = @"C:\SAM\Base\Server\Run\SysDat\PCKLST.DAT";
      private static string prklstPath = @"C:\SAM\Base\Server\Run\SysDat\PRKLST.DAT";
      private static string bmpagvPath = @"C:\SAM\Base\Server\Run\SysDat\BMPAGV.DAT";
      private static string clsagvPath = @"C:\SAM\Base\Server\Run\SysDat\CLSAGV.DAT";
      private static string opcmgrPath = @"C:\SAM\Base\Server\Run\Ini\OPCMgr.xml";


      public static Field[] ReadPNDDEFConfig()
      {
         return ReadConfig(pnddefConfigPath);
      }
      public static Field[] ReadATRDEFConfig()
      {
         return ReadConfig(atrdefConfigPath);
      }
      private static Field[] ReadConfig(string path)
      {
         Field[] fields = new Field[] { };
         if (File.Exists(path))
         {
            using (FileStream f = new FileStream(path, FileMode.Open))
            {
               fields = (Field[])(new XmlSerializer(typeof(Field[]))).Deserialize(f);
            }
         }

         return fields;
      }

      public static void SavePNDDEFConfig(Field[] fields)
      {
         using (FileStream f = new FileStream(pnddefConfigPath, FileMode.Create))
         {
            (new XmlSerializer(typeof(Field[]))).Serialize(f, fields);
         }
      }
      public static void SaveATRDEFConfig(Field[] fields)
      {
         using (FileStream f = new FileStream(atrdefConfigPath, FileMode.Create))
         {
            (new XmlSerializer(typeof(Field[]))).Serialize(f, fields);
         }
      }

      public static List<List<string>> ReadPNDDEF()
      {
         return ReadPNDATR(pnddefPath);
      }
      public static List<List<string>> ReadATRDEF()
      {
         return ReadPNDATR(atrdefPath);
      }
      public static List<List<string>> ReadPNDATR(string path)
      {
         List<List<string>> records = new List<List<string>>();

         if (File.Exists(path))
         {
            string[] lines = File.ReadAllLines(path);
            foreach (string line in lines)
            {
               if (line.IndexOf('/') < 0 || (line.IndexOf('#') >= 0 && line.IndexOf('#') < line.IndexOf('/'))) continue;
               List<string> data = new List<string>(line.Split(','));
               for (int i = 0; i < data.Count; i++)
               {
                  int slashIndex = data[i].IndexOf('/');
                  if(slashIndex >= 0)
                  {
                     data[i] = data[i].Remove(slashIndex);
                  }
                  data[i] = data[i].Trim();
               }

               records.Add(data);
            }
         }
         else
         {
            records = new List<List<string>>();
         }

         return records;
      }
      public static List<List<string>> ReadPCKLST()
      {
         return ReadPCKPRK(pcklstPath);
      }
      public static List<List<string>> ReadPRKLST()
      {
         return ReadPCKPRK(prklstPath);
      }
      public static List<List<string>> ReadPCKPRK(string path)
      {
         List<List<string>> records = new List<List<string>>();

         if (File.Exists(path))
         {
            string[] lines = File.ReadAllLines(path);
            foreach (string line in lines)
            {
               if (line.IndexOf('/') < 0 || (line.IndexOf('#') >= 0 && line.IndexOf('#') < line.IndexOf('/'))) continue;
               List<string> data = new List<string>();
               data.Add(line.Substring(0, line.IndexOf(',')));
               data.Add(line.Substring(line.IndexOf(',') + 1).TrimEnd('/'));

               records.Add(data);
            }
         }
         else
         {
            records = new List<List<string>>();
         }

         return records;
      }
      public static List<List<string>> ReadBMPAGV()
      {
         return ReadBMPCLS(bmpagvPath);
      }
      public static List<List<string>> ReadCLSAGV()
      {
         return ReadBMPCLS(clsagvPath);
      }
      public static List<List<string>> ReadBMPCLS(string path)
      {
         List<List<string>> records = new List<List<string>>();

         if (File.Exists(path))
         {
            string[] lines = File.ReadAllLines(path);
            foreach (string line in lines)
            {
               if (line.IndexOf('/') < 0 || (line.IndexOf('#') >= 0 && line.IndexOf('#') < line.IndexOf('/'))) continue;
               List<string> data = new List<string>(line.Split(','));
               for (int i = 0; i < data.Count; i++)
               {
                  data[i] = data[i].Replace("/", "");
                  data[i] = data[i].Trim();
               }

               records.Add(data);
            }
         }
         else
         {
            records = new List<List<string>>();
         }

         return records;
      }
      public static OPCConnection ReadOPCMGR()
      {
         OPCConnection connection = new OPCConnection();
         XmlReader reader = XmlReader.Create(opcmgrPath);
         
         while(reader.Read())
         {
            switch(reader.NodeType)
            {
               case XmlNodeType.Element:
                  if (string.Equals(reader.Name, "connection", StringComparison.OrdinalIgnoreCase))
                  {
                     connection = new OPCConnection();
                     connection.server = reader.GetAttribute("server");
                     connection.name = reader.GetAttribute("name");
                  }
                  if (string.Equals(reader.Name, "group", StringComparison.OrdinalIgnoreCase))
                  {
                     OPCGroup group = new OPCGroup();
                     group.name = reader.GetAttribute("name");
                     group.active = !string.Equals(reader.GetAttribute("active"), "0");
                     group.write_together = string.Equals(reader.GetAttribute("write_together"), "yes", StringComparison.OrdinalIgnoreCase);
                     group.update_rate = reader.GetAttribute("update_rate") ?? "0";
                     group.trace_mask = reader.GetAttribute("trace_mask");
                     connection.Groups.Add(group);
                  }
                  if (string.Equals(reader.Name, "item", StringComparison.OrdinalIgnoreCase))
                  {
                     OPCItem item = new OPCItem();
                     item.access_path = reader.GetAttribute("access_path");
                     item.id = reader.GetAttribute("id");
                     item.name = reader.GetAttribute("name");
                     item.type = reader.GetAttribute("type") ?? "default";
                     item.access = reader.GetAttribute("access") ?? "modify";
                     item.code = reader.GetAttribute("code") ?? "0";
                     item.active = !string.Equals(reader.GetAttribute("active"), "0");
                     connection.Groups[connection.Groups.Count - 1].Items.Add(item);
                  }
                  break;
            }
         }

         return connection;
      }
      public static void SaveOPCMGR(OPCConnection c)
      {
         XmlWriterSettings settings = new XmlWriterSettings()
         {
            Indent = true,
            IndentChars = "   ",
            //ConformanceLevel = ConformanceLevel.Fragment,
            OmitXmlDeclaration = true
         };
         XmlWriter writer = XmlWriter.Create(opcmgrPath, settings);

         writer.WriteStartElement("OPCMgr");
         writer.WriteStartElement("connection");
         writer.WriteAttributeString("server", c.server);
         writer.WriteAttributeString("name", c.name);
         foreach (OPCGroup g in c.Groups)
         {
            writer.WriteStartElement("group");
            writer.WriteAttributeString("name", g.name);
            writer.WriteAttributeString("write_together", g.write_together ? "yes" : "no");
            writer.WriteAttributeString("update_rate", g.update_rate);
            writer.WriteAttributeString("trace_mask", g.trace_mask);
            writer.WriteAttributeString("active", g.active ? "1" : "0");
            foreach(OPCItem i in g.Items)
            {
               writer.WriteStartElement("item");
               writer.WriteAttributeString("access_path", i.access_path);
               writer.WriteAttributeString("id", i.id);
               writer.WriteAttributeString("name", i.name);
               writer.WriteAttributeString("type", i.type);
               writer.WriteAttributeString("access", i.access);
               writer.WriteAttributeString("code", i.code);
               writer.WriteAttributeString("active", i.active ? "1" : "0");
               writer.WriteEndElement();
            }
            writer.WriteEndElement();
         }
         writer.WriteEndElement();
         writer.WriteEndElement();

         writer.Close();
      }

      public static void SavePNDDEF(List<string> lines)
      {
         File.WriteAllLines(pnddefPath, lines);
      }
      public static void SaveATRDEF(List<string> lines)
      {
         File.WriteAllLines(atrdefPath, lines);
      }
      public static void SavePCKLST(List<string> lines)
      {
         File.WriteAllLines(pcklstPath, lines);
      }
      public static void SavePRKLST(List<string> lines)
      {
         File.WriteAllLines(prklstPath, lines);
      }
      public static void SaveBMPAGV(List<string> lines)
      {
         File.WriteAllLines(bmpagvPath, lines);
      }
      public static void SaveCLSAGV(List<string> lines)
      {
         File.WriteAllLines(clsagvPath, lines);
      }

   }
   public class Field
   {
      public string FieldName { get; set; }
      public string Description { get; set; }
      public string Type { get; set; }
      public List<string> ComboValues { get; set; }
   }

   public class OPCConnection
   {
      public string server { get; set; }
      public string name { get; set; }
      public List<OPCGroup> Groups { get; set; }

      public OPCConnection()
      {
         Groups = new List<OPCGroup>();
      }
   }

   public class OPCItem : INotifyPropertyChanged
   {
      [DoNotNotify]
      public string access_path { get; set; }
      private string _id;
      public string id { get { return _id; } set { _id = value; OnPropertyChanged(nameof(id)); } }
      [DoNotNotify]
      public string name { get; set; }
      [DoNotNotify]
      public string type { get; set; }
      [DoNotNotify]
      public string access { get; set; }
      [DoNotNotify]
      public string code { get; set; }
      [DoNotNotify]
      public bool active { get; set; }
      [DoNotNotify]
      public string group { get; set; }

      public event PropertyChangedEventHandler PropertyChanged;

      private void OnPropertyChanged(string propertyName)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
   }
   public class OPCGroup : INotifyPropertyChanged
   {
      private string _name;
      public string name { get { return _name; } set { _name = value; OnPropertyChanged(nameof(name)); } }
      [DoNotNotify]
      public bool write_together { get; set; }
      [DoNotNotify]
      public string update_rate { get; set; }
      [DoNotNotify]
      public string trace_mask { get; set; }
      [DoNotNotify]
      public bool active { get; set; }
      public List<OPCItem> Items{ get; set; }

      public OPCGroup()
      {
         Items = new List<OPCItem>();
      }

      public event PropertyChangedEventHandler PropertyChanged;

      private void OnPropertyChanged(string propertyName)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
   }

   public enum FileType { PND, ATR, PCK, PRK, BMP, CLS }
}
