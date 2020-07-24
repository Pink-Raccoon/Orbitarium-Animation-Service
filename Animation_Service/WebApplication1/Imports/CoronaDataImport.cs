using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.Imports
{
    public class CoronaDataImport : Import
    {
        public string[] ImportCoronaData()
        {
            var srcPath = Path.Combine(BASE_DIR, SRC_FOLDER, "corona_data", "full_grouped.csv");

            OrderedDictionary coronaInformation = new OrderedDictionary();
            DateTime dateBefore = DateTime.MinValue;
            Dictionary<string, DateCountryInfection> dayInformation = new Dictionary<string, DateCountryInfection>();
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
                    var dateCountryInformation = new DateCountryInfection
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
                        dayInformation = new Dictionary<string, DateCountryInfection>();
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