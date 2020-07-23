using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class CountryImportService
    {
        private PersistenceService persistence = new PersistenceService();
        private static string BASE_DIR = ConfigurationManager.AppSettings["BASE_DIR"]; 
        private static string COUNTRIES_SRC_FOLDER = ConfigurationManager.AppSettings["COUNTRIES_SRC_FOLDER"];
        private static string ANIMATIONS_STORAGE_FOLDER = ConfigurationManager.AppSettings["ANIMATIONS_STORAGE_FOLDER"];

        public string getCountryNames()
        {
            var filePath = Path.Combine(BASE_DIR, ANIMATIONS_STORAGE_FOLDER, "countries", "allCountryNames.json");
            string names = File.ReadAllText(filePath);
            return names;
        }

        public string[] ImportCountries()
        {
            var srcPath = Path.Combine(BASE_DIR, COUNTRIES_SRC_FOLDER);
            var files = Directory.GetFiles(srcPath);
            //IList<Country> allCountries = new List<Country>();
            foreach (var file in files)
            {
                //allCountries.Add(ParseCountryModel(file));
                ParseCountryModel(file);
            }
            //PersistCountryModel(allCountries);
            return files;
        }

        private void ParseCountryModel(string filePath)
        {
            var doc = XDocument.Load(filePath);
            XNamespace ns = "http://www.opengis.net/kml/2.2";

            // get all outer boundaries. we don't care about the inner ones.
            var outerBoundaries = doc.Descendants(ns + "outerBoundaryIs").ToArray();
            var countryName = doc.Descendants(ns + "SimpleData").ToList()[0].Value;

            List<string> paths = new List<string>();
            
            // list with string for each collection of boarder points
            foreach (XElement elem in outerBoundaries)
            {
                paths.Add(elem.Element(ns + "LinearRing").Element(ns + "coordinates").Value);
            }


            IList<Coordinate>[] coordinates = new List<Coordinate>[paths.Count];
            var i = 0;
            foreach (var path in paths)
            {
                var coords = path.Split(' ');
                IList<Coordinate> pathCoords = new List<Coordinate>();
                int count = 0;
                int coordLenght = coords.Length -1;
                foreach (var coord in coords)
                {
                    if (count % 10 == 0 || count == coordLenght-1) { 
                        var latLong = coord.Split(',');
                        var coordObj = new Coordinate
                        {
                            lat = Convert.ToDouble(latLong[1]),
                            lng = Convert.ToDouble(latLong[0])
                        };
                        pathCoords.Add(coordObj);
                    }
                    count++;
                }
                coordinates[i] = pathCoords;
                i++;
            }
            Country country = new Country
            {
                Name = countryName,
                Path = coordinates
            };

            PersistCountryModel(country);
        }

        private void PersistCountryModel(Country country)
        {
            string pathToFile = Path.Combine(BASE_DIR, ANIMATIONS_STORAGE_FOLDER, "countries", country.Name + ".json");
            using (TextWriter textWriter = File.CreateText(pathToFile)) {
                var serializer = new JsonSerializer();
                serializer.Serialize(textWriter, country);
            }
        }
    }



}