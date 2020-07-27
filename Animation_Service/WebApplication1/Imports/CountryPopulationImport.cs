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
                    var population = ConvertToInt32(splitted[1]);
                    var yearlyChange = ConvertToDouble(splitted[2]);
                    var netChange = ConvertToInt32(splitted[3]);
                    var density = ConvertToInt32(splitted[4]);
                    var landArea = ConvertToInt32(splitted[5]);
                    var migrants = ConvertToDouble(splitted[6]);
                    var fertilityRate = ConvertToDouble(splitted[7]);
                    var medianAge = ConvertToInt32(splitted[8]);
                    var urbanPopulation = ConvertToDouble(splitted[9]);
                    var worldShare = ConvertToDouble(splitted[10]);

                    var populationInformation = new CountryPopulation
                    {
                        CountryName = country,
                        Population = population,
                        YearlyChange = yearlyChange,
                        NetChange = netChange,
                        Density = density,
                        LandArea = landArea,
                        Migrants = migrants,
                        FertilityRate = fertilityRate,
                        MedianAge = medianAge,
                        UrbanPopulation = urbanPopulation,
                        WorldShare = worldShare
                    };
                    countryPopulationInformations.Add(country, populationInformation);
                    count++;
                }
            }
            PersistCountryPopulationInformation(countryPopulationInformations);
            return new string[] { "success", "country population for " + count + " countries has been successfully imported into the datastore" };

        }

        private double ConvertToDouble(string dbl)
        {
            if (dbl.Equals("")){
                return -1.0;
            }
            else if (dbl.ToLower().Equals("n.a."))
            {
                return -1.0;
            }
            else if (dbl.Contains("%"))
            {
                return Convert.ToDouble(dbl.Replace('%', ' ').Trim(' ')) / 100.0;
            } else
            {
                return Convert.ToDouble(dbl);
            }
        }

        private int ConvertToInt32(string intgr)
        {
            if (intgr.Equals(""))
            {
                return -1;
            }
            else if (intgr.ToLower().Equals("n.a."))
            {
                return -1;
            }
            else if (intgr.Contains("%"))
            {
                return Convert.ToInt32(intgr.Replace('%', ' ').Trim(' ')) / 100;
            }
            else
            {
                return Convert.ToInt32(intgr);
            }
        }

        private double ConvertUrbanPopulation(string pop)
        {
            if (pop.ToLower().Equals("n.a."))
            {
                return -1.0;
            } else
            {
                return Convert.ToDouble(pop.Replace('%', ' ').Trim(' ')) / 100.0;
            }
        }

        private void PersistCountryPopulationInformation(Dictionary<string, CountryPopulation> populationInformation)
        {
            var destPath = Path.Combine(BASE_DIR, DATASTORE_FOLDER, "world_population", "world_population.json");
            var coronaInformationString = JsonConvert.SerializeObject(populationInformation, Formatting.Indented);
            persistence.SaveToFile(destPath, coronaInformationString);
        }
    }
}