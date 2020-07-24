using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApplication1.Imports
{
    public class CountryPopulationImport : Import
    {
        public string[] ImportCountryPopulationData()
        {
            var srcPath = Path.Combine(BASE_DIR, SRC_FOLDER, "country_population", "country_population_2020.csv");



            return new string[] { "success", "country population for " + "TODO" + " has been successfully imported into the datastore" };
        }
    }
}