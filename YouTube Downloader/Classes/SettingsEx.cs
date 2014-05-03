using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace YouTube_Downloader
{
    public class SettingsEx
    {
        public static bool AutoConvert = false;
        public static bool AutoDelete = false;
        public static string DefaultDirectory = Directory.GetCurrentDirectory();
        public static List<string> SaveToDirectories = new List<string>();
        public static int SelectedDirectory = 0;
        public static Dictionary<string, WindowState> WindowStates = new Dictionary<string, WindowState>();

        public static void Load()
        {
            
            Debug.WriteLine("This is the Settings Loading");
            string file = Application.StartupPath + "\\YouTube Downloader.xml";

            SettingsEx.WindowStates = new Dictionary<string, WindowState>();

            if (!File.Exists(file))
                Save();                

            XmlDocument document = new XmlDocument();

            document.LoadXml(File.ReadAllText(file));

            if (!document.HasChildNodes)
                return;

            var properties = document.GetElementsByTagName("properties")[0];

            if (properties != null)
            {
                if (properties.Attributes["auto_convert"] != null)
                {
                    AutoConvert = bool.Parse(properties.Attributes["auto_convert"].Value);
                }
                if (properties.Attributes["auto_delete"] != null)
                {
                    AutoDelete = bool.Parse(properties.Attributes["auto_delete"].Value);
                }
                if (properties.Attributes["default_directory"] != null)
                {
                    DefaultDirectory = properties.Attributes["default_directory"].Value;
                }
                else
                    DefaultDirectory = Directory.GetCurrentDirectory();
            }

            foreach (XmlNode node in document.GetElementsByTagName("form"))
            {
                WindowState windowState = new WindowState(node);

                SettingsEx.WindowStates.Add(windowState.FormName, windowState);
            }

            XmlNode directories = document.GetElementsByTagName("save_to_directories")[0];

            SelectedDirectory = int.Parse(directories.Attributes["selected_directory"].Value);
           
            foreach (XmlNode node in directories.ChildNodes)
            {
                if (node.LocalName != "path")
                    continue;

                SaveToDirectories.Add(node.InnerText);             
            }
        }

        public static void Save()
        {
            Debug.WriteLine("We enter the save method");
          
            XmlWriterSettings settings = new XmlWriterSettings();

            settings.Indent = true;

            string file = Application.StartupPath + "\\YouTube Downloader.xml";

            if (!SaveToDirectories.Contains(DefaultDirectory))             
                SaveToDirectories.Add(DefaultDirectory);

            using (XmlWriter w = XmlWriter.Create(file, settings))
            {
                w.WriteStartDocument();
                w.WriteStartElement("properties");
                w.WriteAttributeString("auto_convert", AutoConvert.ToString());
                w.WriteAttributeString("auto_delete", AutoDelete.ToString());
                w.WriteAttributeString("default_directory", DefaultDirectory.ToString());              

                foreach (WindowState windowState in SettingsEx.WindowStates.Values)
                {
                    windowState.SerializeXml(w);
                }

                w.WriteStartElement("save_to_directories");
                w.WriteAttributeString("selected_directory", SelectedDirectory.ToString());

                foreach (string directory in SaveToDirectories)
                {
                    w.WriteElementString("path", directory);
                }

                w.WriteEndElement();

                w.WriteEndElement();
                w.WriteEndDocument();

                w.Flush();
                w.Close();
            }
        }
    }
}