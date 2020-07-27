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
        public void SaveToFile (string path, string content)
        {
            FileInfo file = new FileInfo(path);
            file.Directory.Create();
            File.WriteAllText(path, content);
            Console.WriteLine(path);
        }

        public string ReadFromFile(string path)
        {
            FileInfo file = new FileInfo(path);
            string content = "nothing";
            if (file.Exists)
            {
                content = File.ReadAllText(path);
            }
            return content;

        }
    }
}