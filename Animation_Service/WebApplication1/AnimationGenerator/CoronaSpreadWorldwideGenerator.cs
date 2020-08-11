using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using WebApplication1.MapsObjects;
using WebApplication1.Models;
using System.Drawing;

namespace WebApplication1.AnimationGenerator
{
    public class CoronaSpreadWorldwideGenerator : AnimationGenerator
    {
        private double maxRelativeNewCases = 0.0;
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

            EvaluateHighestInfectionCount(coronaSpread, countryPopulations);
            SetAnimationInformation();
            GenerateAnimationInitialization(countryBorders, coronaSpread, countryPopulations);
            GenerateAnimationUpdates(countryBorders, coronaSpread, countryPopulations);
            GenerateAnimationInformation();
        }

        private void SetAnimationInformation()
        {
            animationInformation = new AnimationInformation();
            //animationInformation.AnimationKey = "corona_spread_worldwide";
            //animationInformation.AnimationName = "Corona Spread Worldwide";
            animationInformation.AnimationKey = "corona_spread_active_cases";
            animationInformation.AnimationName = "Corona Spread Worldwide Active Cases";
            animationInformation.InitUri = "http://localhost:12346/corona_spread/";
            animationInformation.RunCommand = "runAnimation();";
            //animationInformation.AnimationDescription = "This animation displays the spread of the corona virus. In the beginning, it display the corona situation on January 22nd, 2020. Normal countries are not marked, infected are red. The opacity indicates the count of infected humans relative to other countries.";
            animationInformation.AnimationDescription = "This animation displays all active cases of Covid-19 infections for any given day in every tracked country starting on January 22nd, 2020. EXCLAIMER: The active infection count for each US state is estimated! We do not have the exact active infection count per state!";
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
            var destPath = Path.Combine(BASE_DIR, ANIMATIONS_FOLDER, "corona_spread_active_cases", "corona_spread_init.json");
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

                    double fillOpacity = getFillOpacity(countryInformation.Active, countryPopulations[countryBorder.Key]);
                    //string color = getFillColor(countryInformation.NewCases, countryPopulations[countryBorder.Key]);

                    if (countryInformation.ConfirmedInfections != 0)
                    {
                        polygon.StrokeColor = INFECTED_COLOR;
                        polygon.FillColor = INFECTED_COLOR;
                        polygon.FillOpacity = fillOpacity;
                        //polygon.FillOpacity = 0.2;
                        polygon.Paint = true;
                    } else
                    {
                        polygon.Paint = false;
                    }
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

                        double fillOpacity = getFillOpacity(countryInfectionInformation.Active, countryPopulations[countryName]);
                        //string color = getFillColor(countryInfectionInformation.NewCases, countryPopulations[countryName]);

                        var polygonUpdate = new PolygonUpdate();
                        if(countryInfectionInformation.ConfirmedInfections != 0)
                        {
                            polygonUpdate.StrokeColor = INFECTED_COLOR;
                            polygonUpdate.FillColor = INFECTED_COLOR;
                            polygonUpdate.FillOpacity = fillOpacity;
                            //polygonUpdate.FillOpacity = 0.2;
                            polygonUpdate.Paint = true;
                        }
                        //Debug line                        
                        //if (countryName.Equals("Switzerland"))
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
            var destPath = Path.Combine(BASE_DIR, ANIMATIONS_FOLDER, animationInformation.AnimationKey, "corona_spread_updates.json");
            var serialized = JsonConvert.SerializeObject(polygonUpdateTimeline, Formatting.None);
            persistence.SaveToFile(destPath, serialized);
        }

        private void PersistInitalization(Dictionary<string, Polygon> mapsObjects)
        {
            var destPath = Path.Combine(BASE_DIR, ANIMATIONS_FOLDER, animationInformation.AnimationKey, "corona_spread_init.json");
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

        private void EvaluateHighestInfectionCount(OrderedDictionary coronaSpread, Dictionary<string, CountryPopulation> countryPopulations)
        {
            var enumerator = coronaSpread.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var date = enumerator.Key;
                Dictionary<string, DateCountryInfection> dayInfections = (Dictionary<string, DateCountryInfection>)enumerator.Value;
                foreach (var dayInfection in dayInfections)
                {
                    var countryName = dayInfection.Key;
                    var countryInfectionInformation = dayInfection.Value;

                    double newCases = Convert.ToDouble(countryInfectionInformation.Active);
                    CountryPopulation info = countryPopulations[countryName];
                    double population = Convert.ToDouble(info.Population);
                    double relNewCases = newCases / population;
                    if (relNewCases > maxRelativeNewCases)
                    {
                        // grösste gemessene neu-ansteckungszahl relativ zur bevölkerung dieses landes
                        maxRelativeNewCases = relNewCases;
                    }
                }
            }
        }

        private string getFillColor(int infections, CountryPopulation countryPopulation) {
            double percent = getFillOpacity(infections, countryPopulation);
            Color myColor;
            if (percent <= 0.25)
            {
                myColor = Color.FromArgb(230, 200, 50);
                return "#" + myColor.R.ToString("X2") + myColor.G.ToString("X2") + myColor.B.ToString("X2");
            }
            else if (percent <= 0.5)
            {
                myColor = Color.FromArgb(230, 170, 50);
                return "#" + myColor.R.ToString("X2") + myColor.G.ToString("X2") + myColor.B.ToString("X2");
            }
            else if (percent <= 0.8)
            {
                myColor = Color.FromArgb(230, 130, 50);
                return "#" + myColor.R.ToString("X2") + myColor.G.ToString("X2") + myColor.B.ToString("X2");
            }
            else if (percent <= 0.995)
            {
                myColor = Color.FromArgb(230, 50, 50);
                return "#" + myColor.R.ToString("X2") + myColor.G.ToString("X2") + myColor.B.ToString("X2");
            }
            else {
                myColor = Color.FromArgb(180, 50, 230);
                return "#" + myColor.R.ToString("X2") + myColor.G.ToString("X2") + myColor.B.ToString("X2");
            }
        }

        private double getFillOpacity(int newC, CountryPopulation countryPopulation) {
            double newCases = Convert.ToDouble(newC);
            double population = Convert.ToDouble(countryPopulation.Population);
            double relNewCases = newCases / population;
            return calculateFillOpacity(relNewCases);
        }

        private double calculateFillOpacity(double relNewCases) {
            // calculate logarithmic values to linear percentage
            // source: https://stackoverflow.com/a/19796139
            //double b = 10.0;
            double b = 11.0;
            double s = 100.0 / (b - 1);
            double t = -100.0 / (b - 1);
            double f = s * Math.Pow(b, relNewCases / maxRelativeNewCases) + t;
            // bring it in [0..1] range
            return f;
        }
    }
}