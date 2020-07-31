using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using WebApplication1.MapsObjects;
using WebApplication1.Models;

namespace WebApplication1.AnimationGenerator
{
    public class CoronaSpreadWorldwideGenerator : AnimationGenerator
    {
        private static double highestInfectionRate = 0.0;
        private static double opacityLevelPercent = 0.0;

        protected readonly string INFECTED_COLOR = "#ff0000";
        protected readonly string NO_INFO_COLOR = "#000000";
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

            GenerateAnimationInitialization(countryBorders, coronaSpread, countryPopulations);
            GenerateAnimationUpdates(countryBorders, coronaSpread, countryPopulations);
        }

        public string getDifferences() {
            //get corona spread data
            OrderedDictionary coronaSpread = GetSpreadData();
            //get country population information
            Dictionary<string, CountryPopulation> countryPopulations = GetCountryPopulations();

            Dictionary<string, DateCountryInfection> countryInformations = (Dictionary<string, DateCountryInfection>)coronaSpread[0];

            string diff = "";

            foreach (KeyValuePair<string, DateCountryInfection> entry in countryInformations)
            {
                if (!countryPopulations.ContainsKey(entry.Key)) {
                    diff = diff + entry.Key + ",";
                }
            }

            return diff;
        }

        //todo: move to AnimationDisplay
        public string GetAnimationInitialization()
        {
            var destPath = Path.Combine(BASE_DIR, ANIMATIONS_FOLDER, "corona_spread", "corona_spread_init.json");
            return persistence.ReadFromFile(destPath);
        }

        private string GenerateAnimationInitialization(Dictionary<string, CountryBorders> countryBorders, OrderedDictionary coronaSpread, Dictionary<string, CountryPopulation> countryPopulations)
        {
            Dictionary<string, DateCountryInfection> countryInformations = (Dictionary<string, DateCountryInfection>)coronaSpread[0];
            Dictionary<string, Polygon> mapsObjects = new Dictionary<string, Polygon>();
            foreach (var countryBorder in countryBorders)
            {
                var polygon = new Polygon();
                polygon.Type = "polygon";
                polygon.StrokeOpacity = 0.8;
                polygon.StrokeWeight = 2;
                polygon.Paths = countryBorder.Value.Path;

                if (countryInformations.ContainsKey(countryBorder.Key))
                {
                    var countryInformation = countryInformations[countryBorder.Key];

                    double fillOpacity = calculateFillOpacity(countryInformation.ConfirmedInfections, countryPopulations[countryBorder.Key]);

                    if (countryInformation.ConfirmedInfections != 0)
                    {
                        polygon.StrokeColor = INFECTED_COLOR;
                        polygon.FillColor = INFECTED_COLOR;
                        //Todo: set opacity based on infection rate
                        //polygon.FillOpacity = ToDo!
                        polygon.FillOpacity = fillOpacity;
                        polygon.Paint = true;
                    } else
                    {
                        polygon.Paint = false;
                    }
                } else
                {
                    polygon.StrokeColor = NO_INFO_COLOR;
                    polygon.FillColor = NO_INFO_COLOR;
                    polygon.FillOpacity = 0.2;
                    polygon.Paint = true;
                }
                //Debug line
                //if (countryBorder.Key.Equals("Switzerland")){
                    mapsObjects.Add(countryBorder.Key, polygon);
                //}
            }
            mapsObjects.Remove("Antarctica");
            PersistInitalization(mapsObjects);
            return "animation successfully initialized";
        }

        private string GenerateAnimationUpdates(Dictionary<string, CountryBorders> countryBorders, OrderedDictionary coronaSpread, Dictionary<string, CountryPopulation> countryPopulations)
        {
            var enumerator = coronaSpread.GetEnumerator();
            var polygonUpdateTimeline = new OrderedDictionary();
            while (enumerator.MoveNext())
            {
                var date = enumerator.Key;
                var dayUpdates = new Dictionary<string, MapsObject>();
                Dictionary<string, DateCountryInfection> dayInfections = (Dictionary<string, DateCountryInfection>) enumerator.Value;
                foreach(var dayInfection in dayInfections)
                {
                    var countryName = dayInfection.Key;

                    if (countryBorders.ContainsKey(countryName))
                    {
                        var countryInfectionInformation = dayInfection.Value;

                        double fillOpacity = calculateFillOpacity(countryInfectionInformation.ConfirmedInfections, countryPopulations[countryName]);

                        var polygonUpdate = new PolygonUpdate();
                        if(countryInfectionInformation.ConfirmedInfections != 0)
                        {
                            polygonUpdate.StrokeColor = INFECTED_COLOR;
                            polygonUpdate.FillColor = INFECTED_COLOR;
                            //Todo: set opacity based on infection rate
                            //polygon.FillOpacity = ToDo!
                            polygonUpdate.FillOpacity = fillOpacity;
                            polygonUpdate.Paint = true;
                        }
                        //Debug line                        //if (countryName.Equals("Switzerland"))
                        //{
                        dayUpdates.Add(countryName, polygonUpdate);
                        //}
                    }
                    
                }
                polygonUpdateTimeline.Add(date, dayUpdates);

            }
            PersistAnimationUpdates(polygonUpdateTimeline);

            return "success";
        }

        private void PersistAnimationUpdates(OrderedDictionary polygonUpdateTimeline)
        {
            var destPath = Path.Combine(BASE_DIR, ANIMATIONS_FOLDER, "corona_spread", "corona_spread_updates.json");
            var serialized = JsonConvert.SerializeObject(polygonUpdateTimeline, Formatting.None);
            persistence.SaveToFile(destPath, serialized);
        }

        private void PersistInitalization(Dictionary<string, Polygon> mapsObjects)
        {
            var destPath = Path.Combine(BASE_DIR, ANIMATIONS_FOLDER, "corona_spread", "corona_spread_init.json");
            var serialized = JsonConvert.SerializeObject(mapsObjects, Formatting.None);
            persistence.SaveToFile(destPath, serialized);
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

        private double calculateFillOpacity(int confirmedInfections, CountryPopulation countryPopulation) {
            int population = countryPopulation.Population;
            double rate = confirmedInfections / population;

            updateHighestInfectionRateAndOpacityLevel(population, rate);

            return rate * opacityLevelPercent;
        }

        private void updateHighestInfectionRateAndOpacityLevel(int population, double rate) {

            if (rate > highestInfectionRate) {
                highestInfectionRate = rate;
                // update opacity (in %) for 1% of country infection
                opacityLevelPercent = 100 / highestInfectionRate;
            }
        }

        private int GetHighestInfectionsPerMio(Dictionary<string, CountryPopulation> countryPopulations, Dictionary<string, DateCountryInfection> coronaSpread)
        {
            int highestInfections = 0;
            foreach (var countryPopulation in countryPopulations)
            {
                var countryName = countryPopulation.Key;
                if (coronaSpread.ContainsKey(countryName)){
                    var countryInfo = countryPopulation.Value;
                    var population = countryInfo.Population;
                    
                }


                
                

            }
            return 0;

        }

    }
}