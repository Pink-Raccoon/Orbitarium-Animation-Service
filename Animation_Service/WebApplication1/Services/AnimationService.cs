using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApplication1.Services
{
    public class AnimationService
    {
        private static string BASE_DIR = ConfigurationManager.AppSettings["BASE_DIR"];
        private static string ANIMATIONS_STORAGE_FOLDER = ConfigurationManager.AppSettings["ANIMATIONS_STORAGE_FOLDER"];
        private static string ALL_COUNTRIES_FILE = "allCountries.json";

        public string GetAnimation()
        {
            var countryPath = Path.Combine(BASE_DIR, ANIMATIONS_STORAGE_FOLDER, "countries", ALL_COUNTRIES_FILE);
            var files = Directory.GetFiles(countryPath);
            int length = files.Length;
            //string[] countryData = new string[length];
            //int i = 0;
            string everyCountry = "[";
            foreach (var file in files) {
                everyCountry = everyCountry + File.ReadAllText(file);
            }
            everyCountry = everyCountry + "]";
            //string text = File.ReadAllText(switzerlandPath);
            return everyCountry;
        }

        public string GetDataFor(string name) {
            var charsToRemove = new string[] { "\\", "\""};
            foreach (var c in charsToRemove)
            {
                name = name.Replace(c, string.Empty);
            }
            var countryPath = Path.Combine(BASE_DIR, ANIMATIONS_STORAGE_FOLDER, "countries", name + ".json");
            string text = File.ReadAllText(countryPath);
            return text;
        }
    }
}