using System;
using System.Collections.Generic;
using System.IO;

namespace QuizAutomation.Utils.Data
{
    class PropertiesFile
    {

        private Dictionary<string, string> keyValues;

        private string fileName;

        public PropertiesFile(string fileName)
        {
            this.fileName = fileName;
            keyValues = new Dictionary<string, string>();
            InitializeKeyValues();
        }

        private void InitializeKeyValues()
        {
            string[] lines = File.ReadAllLines(fileName);
            string key, value;
            foreach (string line in lines)
            {
                key = line.Substring(0, line.IndexOf("=")).Trim();
                value = line.Substring(line.IndexOf("=") + 1);
                keyValues.Add(key, value);
            }
        }

        public string GetValue(string key) => keyValues[key];

        public string SetValue(string key, string value) => keyValues[key] = value;

        public void Save()
        {
            File.WriteAllText(fileName, "");
            foreach (string key in keyValues.Keys)
            {
                File.AppendAllText(fileName, $"{key}={keyValues[key]}\n");
            }
        }

        public void SetAndSave(string key, string value)
        {
            SetValue(key, value);
            Save();
        }

    }

    class Properties
    {

        private static Dictionary<string, PropertiesFile> properties;
        
        public static PropertiesFile CreateOrOpenPropertiesFile(string fileName)
        {
            if (properties == null)
                properties = new Dictionary<string, PropertiesFile>();

            File.Open(fileName, FileMode.OpenOrCreate).Close();

            PropertiesFile propertiesFile = new PropertiesFile(fileName);
            properties.Add(fileName, propertiesFile);

            return propertiesFile;
        }

        public static PropertiesFile GetPropertiesFileLocally(string fileName) => new PropertiesFile(fileName);

        public static PropertiesFile GetPropertiesFile(string fileName) => properties[fileName];

    }
}
