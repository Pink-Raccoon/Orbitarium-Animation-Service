using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using WebApplication1.MapsObjects;
using WebApplication1.Models;

namespace WebApplication1.Animations
{
    public class CoronaSpreadWorldwide : Animation
    {
        //get path to the datastore
        protected string datastorePath = Path.Combine(BASE_DIR, DATASTORE_FOLDER);



        public void GenerateAnimation()
        {
            //get all country borders
            var countryBorders = GetCountryBorders();
            //get corona spread data
            var coronaSpread = GetSpreadData();
            //get country population information
            var countryPopulations = GetCountryPopulations();

            var animationInitialization = GenerateAnimationInitialization(countryBorders, coronaSpread, countryPopulations);

            OrderedDictionary animation = new OrderedDictionary();


            




            var a = 1;
        }

        private object GenerateAnimationInitialization(Dictionary<string, CountryBorders> countryBorders, OrderedDictionary coronaSpread, Dictionary<string, CountryPopulation> countryPopulations)
        {
            Dictionary<string, Polygon> mapsObjects = new Dictionary<string, Polygon>();
            foreach (var countryBorder in countryBorders)
            {
                var polygon = new Polygon();
                polygon.Type = "polygon";
                //var initialCountryInformation = coronaSpread[0];
                Dictionary<string, DateCountryInfection> countryInformations = (Dictionary<string, DateCountryInfection>)coronaSpread[0];
                if (countryInformations.ContainsKey(countryBorder.Key))
                {
                    var countryInformation = countryInformations[countryBorder.Key];
                }
                //var countryName = countryInformation[countryBorder.Key];

            }
            return "";
        }

        private Dictionary<string, CountryBorders> GetCountryBorders()
        {
            var countryBorders = new Dictionary<string, CountryBorders>();
            var countryBorderPath = Path.Combine(datastorePath, "country_border");
            var countryBorderFiles = Directory.GetFiles(countryBorderPath);

            foreach(var file in countryBorderFiles)
            {
                var content = File.ReadAllText(file);
                var countryBorder = JsonConvert.DeserializeObject<CountryBorders>(content);
                countryBorders.Add(countryBorder.Name, countryBorder);
            }
            return countryBorders;
        }

        private OrderedDictionary GetSpreadData()
        {
            var spreadDataPath = Path.Combine(datastorePath, "corona_spread", "corona_spread.json");
            var content = File.ReadAllText(spreadDataPath);
            var spreadData = JsonConvert.DeserializeObject<OrderedDictionary>(content);
            var spreadDataParsed = new OrderedDictionary();
            foreach(var key in spreadData.Keys)
            {
                var infectionInformation = spreadData[key].ToString();
                var converted = JsonConvert.DeserializeObject<Dictionary<string, DateCountryInfection>>(infectionInformation);
                spreadDataParsed.Add(key, converted);
            }

            return spreadDataParsed;
        }

        private Dictionary<string, CountryPopulation> GetCountryPopulations()
        {
            var countryPopulationsPath = Path.Combine(datastorePath, "world_population", "world_population.json");
            var content = File.ReadAllText(countryPopulationsPath);
            var countryPopulations = JsonConvert.DeserializeObject<Dictionary<string, CountryPopulation>>(content);
            return countryPopulations;
        }



        public void Test()
        {
            
        }
    }
}