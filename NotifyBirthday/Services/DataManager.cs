using Microsoft.Win32;
using System.IO;
using System.Xml.Serialization;

namespace NotifyBirthday
{
    public class DataManager
    {
        public static T Load<T>(string filename) where T : class, new()
        {
            T returnedList = new T();
            if (File.Exists(filename))
            {
                FileInfo file = new FileInfo(filename);
                if (file.Length != 0)
                {
                    using (StreamReader streamReader = new StreamReader(filename))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                        returnedList = (T)xmlSerializer.Deserialize(streamReader);
                    }
                }
            }
            return returnedList;
        }
        public static void Save<T>(T collection, string filename) where T : class
        {
            if (!File.Exists(filename))
                File.Create(filename).Close();
            if (collection != null)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                var streamWriter = new StreamWriter(filename, false);
                xmlSerializer.Serialize(streamWriter, collection);
                streamWriter.Close();
            }
        }
        public static void Export<T>(T collection) where T : class
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "XML файлы|*.xml"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                if (!File.Exists(saveFileDialog.FileName))
                    File.Create(saveFileDialog.FileName).Close();
                if (collection != null)
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    var streamWriter = new StreamWriter(saveFileDialog.FileName, false);
                    xmlSerializer.Serialize(streamWriter, collection);
                    streamWriter.Close();
                }
            }
        }

        public static T Import<T>() where T : class, new()
        {
            T returnedList = new T();
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "XML файлы|*.xml"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                if (File.Exists(openFileDialog.FileName))
                {
                    FileInfo file = new FileInfo(openFileDialog.FileName);
                    if (file.Length != 0)
                    {
                        using (StreamReader streamReader = new StreamReader(openFileDialog.FileName))
                        {
                            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                            returnedList = (T)xmlSerializer.Deserialize(streamReader);
                        }
                    }
                }
                return returnedList;
            }
            return returnedList;
        }

    }
}
