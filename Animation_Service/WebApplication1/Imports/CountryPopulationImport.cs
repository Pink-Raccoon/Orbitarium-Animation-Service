using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.Imports
{
    public class CountryPopulationImport : Import
    {
        public string[] ImportCountryPopulationData()
        {
            var srcPath = Path.Combine(BASE_DIR, SRC_FOLDER, "country_population", "country_population_2020.csv");
            var countryPopulationInformations = new Dictionary<string, CountryPopulation>();
            var count = 0;
            using (StreamReader sr = File.OpenText(srcPath))
            {
                string s;
                //skip csv header
                s = sr.ReadLine();
                
                while ((s = sr.ReadLine()) != null)
                {
                    var splitted = s.Split(',');
                    var country = splitted[0];
                    var populationInformation = new CountryPopulation
                    {
                        CountryName = country,
                        Population = Convert.ToInt32(splitted[1]),
                        YearlyChange = Convert.ToDouble(splitted[2].Replace('%', ' ').Trim(' '))/100.0,
                        NetChange = Convert.ToInt32(splitted[3]),
                        Density = Convert.ToInt32(splitted[4]),
                        LandArea = Convert.ToInt32(splitted[5]),
                        Migrants = Convert.ToInt32(splitted[6]),
                        FertilityRate = Convert.ToDouble(splitted[7]),
                        MedianAge = Convert.ToInt32(splitted[8]),
                        UrbanPopulation = Convert.ToDouble(splitted[9].Replace('%', ' ').Trim(' ')) / 100.0,
                        WorldShare = Convert.ToDouble(splitted[10].Replace('%', ' ').Trim(' ')) / 100.0
                    };
                    countryPopulationInformations.Add(country, populationInformation);
                    count++;
                }
            }
            PersistCountryPopulationInformation(countryPopulationInformations);
            return new string[] { "success", "country population for " + count + " countries has been successfully imported into the datastore" };

        }

        private void PersistCountryPopulationInformation(Dictionary<string, CountryPopulation> populationInformation)
        {
            var destPath = Path.Combine(BASE_DIR, DATASTORE_FOLDER, "world_population", "world_population.json");
            var coronaInformationString = JsonConvert.SerializeObject(populationInformation, Formatting.Indented);
            persistence.SaveToFile(destPath, coronaInformationString);
        }
    }
}