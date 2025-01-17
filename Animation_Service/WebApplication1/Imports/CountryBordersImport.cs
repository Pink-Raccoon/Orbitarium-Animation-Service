﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using WebApplication1.Models;

namespace WebApplication1.Imports
{
    public class CountryBordersImport : Import
    {
        public string[] ImportCountries()
        {
            var srcPath = Path.Combine(BASE_DIR, SRC_FOLDER, "country_borders");
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

            List<XElement> placemarks = doc.Descendants(ns + "Placemark").ToList();

            foreach (XElement placemark in placemarks) {
                PersistCountryModel(getCountryBorder(placemark, ns));
            }
        }

        private CountryBorders getCountryBorder(XElement placemark, XNamespace ns) { 
            // get all outer boundaries. we don't care about the inner ones.
            
            var outerBoundaries = placemark.Descendants(ns + "outerBoundaryIs").ToArray();
            List<XElement> countryNames = placemark.Descendants(ns + "SimpleData").ToList();

            string countryName = countryNames[0].Value;
            if (countryNames.Count > 1)
            {
                countryName = countryNames[1].Value;
            }

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
                    if (count % 20 == 0 || count == coordLenght - 1)
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
            CountryBorders country = new CountryBorders
            {
                Name = countryName,
                Path = coordinates
            };

            return country;
        }

        private void PersistCountryModel(CountryBorders country)
        {
            var countryString = JsonConvert.SerializeObject(country, Formatting.None);
            string fileName = country.Name;
            if (fileName.Equals("Taiwan*")) {
                fileName = "Taiwan";
            }
            persistence.SaveToFile(Path.Combine(BASE_DIR, DATASTORE_FOLDER, "country_border", fileName + ".json"), countryString);
        }
    }
}