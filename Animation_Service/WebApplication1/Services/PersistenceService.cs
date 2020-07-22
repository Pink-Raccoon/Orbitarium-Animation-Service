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
        private static string FILE_PATH = "C:\\Users\\Remo Preuss\\Documents\\GitHub\\Orbitarium-Animation-Service\\Animation_Service\\WebApplication1\\Data\\countries\\";
        private static string FILE_NAME = "gadm36_CHE_0.kml";

        public void SaveToFile (string content)
        {
            //Save content to file
        }

        public void ReadFromFile()
        {

        }

        public string ParseFiles() {
            var doc = XDocument.Load("" + FILE_PATH + FILE_NAME);
            XNamespace ns = "http://www.opengis.net/kml/2.2";

            // get all outer boundaries. we don't care about the inner ones.
            var outerBoundaries = doc.Descendants(ns + "outerBoundaryIs").ToList();

            List<string> coordList = new List<string>();

            // list with string for each collection of boarder points
            foreach (XElement elem in outerBoundaries) {
                coordList.Add(elem.Element(ns + "LinearRing").Element(ns + "coordinates").Value);
            }

            char[] spaceSeparator = { ' ' };
            char[] commaSeparator = { ',' };
            /*foreach (string coords in coordList) {
                String[] latLongPairList = coords.Split(spaceSeparator, StringSplitOptions.RemoveEmptyEntries);
                foreach (string latLongPair in latLongPairList) {
                    String[] latLongValues = latLongPair.Split(commaSeparator, StringSplitOptions.RemoveEmptyEntries);
                    Dictionary<string, double> latLong = new Dictionary<string, double>();
                    latLong.Add("lng", Convert.ToDouble(latLongValues[0]));
                    latLong.Add("lat", Convert.ToDouble(latLongValues[1]));
                }
            }*/

            List<Dictionary<string, double>> coordPoints = new List<Dictionary<string, double>>();
            String[] latLongPairList = coordList.First().Split(spaceSeparator, StringSplitOptions.RemoveEmptyEntries);
            foreach (string latLongPair in latLongPairList)
            {
                String[] latLongValues = latLongPair.Split(commaSeparator, StringSplitOptions.RemoveEmptyEntries);
                Dictionary<string, double> latLong = new Dictionary<string, double>();
                latLong.Add("lng", Convert.ToDouble(latLongValues[0]));
                latLong.Add("lat", Convert.ToDouble(latLongValues[1]));
                coordPoints.Add(latLong);
            }

            string jsonString;
            jsonString = JsonConvert.SerializeObject(coordPoints);
            Console.WriteLine(jsonString);
            return jsonString;
        }
    }
}