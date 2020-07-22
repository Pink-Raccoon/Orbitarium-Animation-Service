using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace WebApplication1.Services
{
    public class PersistenceService
    {

        //@Preussmeister: comment out this line for your directory.
        //private static string DATA_DIR = @"C:\Users\Remo Preuss\Documents\GitHub\Orbitarium-Animation-Service\Animation_Service\WebApplication1\Data";
        private static string DATA_DIR = ConfigurationManager.AppSettings["MyCustomSetting"];
        private static string COUNTRIES_SRC_FOLDER = "countries";
        private static string ANIMATIONS_STORAGE_FOLDER = "json";
        private static string FILE_NAME = "gadm36_CHE_0.kml";

        public void SaveToFile (string path, string content)
        {
            FileInfo file = new FileInfo(path);
            file.Directory.Create();
            File.WriteAllText(path, content);
            Console.WriteLine(path);
        }

        public void ReadFromFile()
        {

        }
    }
}