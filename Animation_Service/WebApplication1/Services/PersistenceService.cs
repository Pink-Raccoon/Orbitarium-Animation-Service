using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace WebApplication1.Services
{
    public class PersistenceService
    {
        private static string FILE_PATH = "C:\\Users\\Remo Preuss\\Documents\\GitHub\\Orbitarium-Animation-Service\\Animation_Service\\WebApplication1\\Data\\";
        private static string FILE_NAME = "gadm36_CHE_0";

        public void SaveToFile (string content)
        {
            //Save content to file
        }

        public void ReadFromFile()
        {

        }

        public string ParseFiles() {
            var doc = XDocument.Load("" + FILE_PATH + "countries\\" +  FILE_NAME + ".kml");
            XNamespace ns = "http://www.opengis.net/kml/2.2";

            // get all outer boundaries. we don't care about the inner ones.
            var outerBoundaries = doc.Descendants(ns + "outerBoundaryIs").ToList();
            string countryName = doc.Descendants(ns + "SimpleData").ToList()[0].Value;

            List<string> coordList = new List<string>();

            // list with string for each collection of boarder points
            foreach (XElement elem in outerBoundaries) {
                coordList.Add(elem.Element(ns + "LinearRing").Element(ns + "coordinates").Value);
            }

            char[] spaceSeparator = { ' ' };
            char[] commaSeparator = { ',' };
            List<List<Dictionary<string, double>>> countryCoordinates = new List<List<Dictionary<string, double>>>();
            foreach (string coords in coordList) {
                String[] latLongPairList = coords.Split(spaceSeparator, StringSplitOptions.RemoveEmptyEntries);
                List<Dictionary<string, double>> coordPoints = new List<Dictionary<string, double>>();
                foreach (string latLongPair in latLongPairList) {
                    String[] latLongValues = latLongPair.Split(commaSeparator, StringSplitOptions.RemoveEmptyEntries);
                    Dictionary<string, double> latLong = new Dictionary<string, double>();
                    latLong.Add("lng", Convert.ToDouble(latLongValues[0]));
                    latLong.Add("lat", Convert.ToDouble(latLongValues[1]));
                    coordPoints.Add(latLong);
                }
                countryCoordinates.Add(coordPoints);
            }

            Dictionary<string, Object> country = new Dictionary<string, Object>();
            country.Add("name", countryName);
            country.Add("coordinates", countryCoordinates);


            using (StreamWriter file = File.CreateText(@"" + FILE_PATH + "jsonData\\" + FILE_NAME + ".txt"))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, country);
            }

            return "";
        }
    }
}