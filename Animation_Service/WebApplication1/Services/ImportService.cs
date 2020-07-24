using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class ImportService
    {
        private PersistenceService persistence = new PersistenceService();
        private static string BASE_DIR = ConfigurationManager.AppSettings["BASE_DIR"];
        private static string SRC_FOLDER = ConfigurationManager.AppSettings["SRC_FOLDER"];
        private static string DATASTORE_FOLDER = ConfigurationManager.AppSettings["DATASTORE_FOLDER"];

        public string[] ImportData(string type)
        {
            if (type.ToLower().Equals("countries"))
            {
                return ImportCountries();
            }
            else if (type.ToLower().Equals("coronadata"))
            {
                return ImportCoronaData();
            }
            else
            {
                return new string[] { "error", "data " + type + " has no corresponding import function" };
            }
        }

        private string[] ImportCountries()
        {
            var srcPath = Path.Combine(BASE_DIR, SRC_FOLDER, "countries");
            var files = Directory.GetFiles(srcPath);
            //IList<Country> allCountries = new List<Country>();
            foreach (var file in files)
            {
                //allCountries.Add(ParseCountryModel(file));
                ParseCountryModel(file);
            }
            //PersistCountryModel(allCountries);
            return new string[] { "success", files.Count() + " country borders have been successfully imported into the datastore" };
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
                int coordLenght = coords.Length - 1;
                foreach (var coord in coords)
                {
                    if (count % 10 == 0 || count == coordLenght - 1)
                    {
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
            var countryString = JsonConvert.SerializeObject(country, Formatting.None);
            persistence.SaveToFile(Path.Combine(BASE_DIR, DATASTORE_FOLDER, "countries", country.Name + ".json"), countryString);
        }

        private string[] ImportCoronaData()
        {
            var srcPath = Path.Combine(BASE_DIR, SRC_FOLDER, "corona_data", "full_grouped.csv");

            OrderedDictionary coronaInformation = new OrderedDictionary();
            DateTime dateBefore = DateTime.MinValue;
            Dictionary<string, DateCountryInfectionInformation> dayInformation = new Dictionary<string, DateCountryInfectionInformation>();
            using (StreamReader sr = File.OpenText(srcPath))
            {
                string s;
                //skip csv header
                s = sr.ReadLine();
                while ((s = sr.ReadLine()) != null)
                {
                    var splitted = s.Split(',');
                    var dateSplitted = splitted[0].Split('-');
                    var date = new DateTime(
                            Convert.ToInt16(dateSplitted[0]),
                            Convert.ToInt16(dateSplitted[1]),
                            Convert.ToInt16(dateSplitted[2])
                        );
                    if (dateBefore.Equals(DateTime.MinValue))
                    {
                        dateBefore = date;
                    }
                    var country = splitted[1];
                    var dateCountryInformation = new DateCountryInfectionInformation
                    {
                        Date = date,
                        Country = country,
                        ConfirmedInfections = Convert.ToInt32(splitted[2]),
                        Deaths = Convert.ToInt32(splitted[3]),
                        Recovered = Convert.ToInt32(splitted[4]),
                        Active = Convert.ToInt32(splitted[5]),
                        NewCases = Convert.ToInt32(splitted[6]),
                        NewDeaths = Convert.ToInt32(splitted[7]),
                        NewRecovered = Convert.ToInt32(splitted[8])
                    };
                    if (date.Equals(dateBefore))
                    {
                        dayInformation.Add(country, dateCountryInformation);
                    }
                    else
                    {
                        coronaInformation.Add(dateBefore, dayInformation);
                        dayInformation = new Dictionary<string, DateCountryInfectionInformation>();
                        dayInformation.Add(country, dateCountryInformation);
                        dateBefore = date;
                    }
                }
            }
            PersistCoronaInformation(coronaInformation);
            return new string[] { "success", "corona data for countries has been successfully imported into the datastore" };
        }

        private void PersistCoronaInformation(OrderedDictionary coronaInformation)
        {
            var destPath = Path.Combine(BASE_DIR, DATASTORE_FOLDER, "corona_spread", "corona_spread.json");
            var coronaInformationString = JsonConvert.SerializeObject(coronaInformation, Formatting.None);
            persistence.SaveToFile(destPath, coronaInformationString);
        }
    }
}