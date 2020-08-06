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
        private OrderedDictionary coronaInformation = new OrderedDictionary();
        public string[] ImportCoronaData()
        {
            var srcPath = Path.Combine(BASE_DIR, SRC_FOLDER, "corona_data", "full_grouped.csv");

            DateTime dateBefore = DateTime.MinValue;
            Dictionary<string, DateCountryInfection> dayInformation = new Dictionary<string, DateCountryInfection>();
            List<DateCountryInfection> dateProvinceInfections = new List<DateCountryInfection>();
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
                        dayInformation = getDateStateInformation(date, dayInformation);
                        dayInformation.Add(country, dateCountryInformation);
                    }
                    else
                    {
                        dateProvinceInfections = importChinaCoronaData(dateBefore);
                        dayInformation = addProvincesToList(dateBefore, dateProvinceInfections, dayInformation);
                        coronaInformation.Add(dateBefore, dayInformation);
                        dayInformation = new Dictionary<string, DateCountryInfection>();
                        dayInformation = getDateStateInformation(date, dayInformation);
                        dayInformation.Add(country, dateCountryInformation);
                        dateBefore = date;
                    }
                }
                dateProvinceInfections = importChinaCoronaData(dateBefore);
                dayInformation = addProvincesToList(dateBefore, dateProvinceInfections, dayInformation);
                coronaInformation.Add(dateBefore, dayInformation);
            }

            PersistCoronaInformation(coronaInformation);
            
            return new string[] { "success", "corona data for countries has been successfully imported into the datastore" };
        }

        private Dictionary<string, DateCountryInfection> addProvincesToList(DateTime date, List<DateCountryInfection> dateProvinceInfections, Dictionary<string, DateCountryInfection> dayInformation) {
            DateTime yesterday = date.AddDays(-1);
            int yesterdayInfectionCount = 0;            

            foreach (DateCountryInfection dateProvinceInfection in dateProvinceInfections) {
                if (coronaInformation.Contains(yesterday))
                {
                    Dictionary<string, DateCountryInfection> a = (Dictionary<string, DateCountryInfection>)coronaInformation[yesterday];
                    DateCountryInfection b = a[dateProvinceInfection.Country];
                    yesterdayInfectionCount = b.ConfirmedInfections;
                }
                dateProvinceInfection.NewCases = dateProvinceInfection.ConfirmedInfections - yesterdayInfectionCount;
                dayInformation.Add(dateProvinceInfection.Country, dateProvinceInfection);
            }
            return dayInformation;
        }

        private Dictionary<string, DateCountryInfection> getDateStateInformation(DateTime date, Dictionary<string, DateCountryInfection> dayInformation) {
            if (dayInformation.Count == 0)
            {
                Dictionary<string, int> stateInfections = importStatesForDate(date);
                int yesterdayInfectionCount;

                DateTime yesterday = date.AddDays(-1);
                DateCountryInfection dateStateInformation;

                foreach (KeyValuePair<string, int> stateInfection in stateInfections)
                {
                    yesterdayInfectionCount = 0;
                    if (coronaInformation.Contains(yesterday))
                    {
                        Dictionary<string, DateCountryInfection> a = (Dictionary<string, DateCountryInfection>)coronaInformation[yesterday];
                        DateCountryInfection b = a[stateInfection.Key];
                        yesterdayInfectionCount = b.ConfirmedInfections;
                    }

                    int newInfections = stateInfection.Value - yesterdayInfectionCount;
                    string state = stateInfection.Key;
                    if (state.Equals("Georgia")) {
                        state = "GeorgiaUS";
                    }
                    dateStateInformation = new DateCountryInfection
                    {
                        Date = date,
                        Country = state,
                        ConfirmedInfections = stateInfection.Value,
                        Deaths = 0,
                        Recovered = 0,
                        Active = 0,
                        NewCases = newInfections,
                        NewDeaths = 0,
                        NewRecovered = 0
                    };
                    dayInformation.Add(state, dateStateInformation);
                }
            }
            return dayInformation;
        }

        private void PersistCoronaInformation(OrderedDictionary coronaInformation)
        {
            var destPath = Path.Combine(BASE_DIR, DATASTORE_FOLDER, "corona_spread", "corona_spread.json");
            var coronaInformationString = JsonConvert.SerializeObject(coronaInformation, Formatting.None);
            persistence.SaveToFile(destPath, coronaInformationString);
        }

        private Dictionary<string, int> importStatesForDate(DateTime date) {
            var srcPath = Path.Combine(BASE_DIR, SRC_FOLDER, "corona_data", "time_series_covid19_confirmed_US.csv");
            Dictionary<string, int> stateInfections = new Dictionary<string, int>();
            using (StreamReader sr = File.OpenText(srcPath))
            {
                string[] header;
                string s;
                //skip csv header
                header = sr.ReadLine().Split(',');
                string state = "";
                int infectionCount = 0;

                // UID 84080001 and below does not interest us!
                while (!(s = sr.ReadLine()).StartsWith("84080002"))
                {
                    var splitted = s.Split(',');
                    // [6]: state name, [11]: 1/22/20 ...
                    var currentState = splitted[6];

                    if (state.Equals(""))
                    {
                        state = currentState;
                    }

                    if (state.Equals(currentState))
                    {
                        infectionCount = parseLine(splitted, header, date, infectionCount);
                    }
                    else {
                        stateInfections.Add(state, infectionCount);
                        state = currentState;
                        infectionCount = parseLine(splitted, header, date, 0);
                    }
                }
            }
            return stateInfections;
        }

        private int parseLine(string[] splitted, string[] header, DateTime date, int infectionCount) {
            for (int i = 11; i < splitted.Length; i++)
            {
                string[] dateSplitted = header[i].Split('/'); ;
                DateTime parsedDate = new DateTime(
                    Convert.ToInt16("20" + dateSplitted[2]),
                    Convert.ToInt16(dateSplitted[0]),
                    Convert.ToInt16(dateSplitted[1])
                );

                if (parsedDate.Equals(date))
                {
                    return infectionCount + Convert.ToInt32(splitted[i + 2]);
                }
            }
            return 0;
        }

        private List<DateCountryInfection> importChinaCoronaData(DateTime date) {
            var srcPath = Path.Combine(BASE_DIR, SRC_FOLDER, "corona_data", "covid_19_clean_complete.csv");
            List<DateCountryInfection> dateProvinceInfectionList = new List<DateCountryInfection>();
            using (StreamReader sr = File.OpenText(srcPath))
            {
                string s;
                //skip csv header
                s = sr.ReadLine();
                while ((s = sr.ReadLine()) != null) {
                    // [0]: Province, [1]: Country, [4]: Date, [5]: Confirmed, [6]: Death, [7]: Recovered, [8]: Active
                    var splitted = s.Split(',');

                    var dateSplitted = splitted[4].Split('-');
                    var parsedDate = new DateTime(
                            Convert.ToInt16(dateSplitted[0]),
                            Convert.ToInt16(dateSplitted[1]),
                            Convert.ToInt16(dateSplitted[2])
                        );

                    if (splitted[1].Equals("China") && parsedDate.Equals(date)) {
                        var dateProvinceInformation = new DateCountryInfection
                        {
                            Date = date,
                            Country = splitted[0],
                            ConfirmedInfections = Convert.ToInt32(splitted[5]),
                            Deaths = Convert.ToInt32(splitted[6]),
                            Recovered = Convert.ToInt32(splitted[7]),
                            Active = Convert.ToInt32(splitted[8]),
                            NewCases = 0,
                            NewDeaths = 0,
                            NewRecovered = 0
                        };
                        dateProvinceInfectionList.Add(dateProvinceInformation);
                    }

                    if (parsedDate > date) {
                        // we are too far down the file -> abort reading for performance sake
                        return dateProvinceInfectionList;
                    }
                }
            }
            return dateProvinceInfectionList;
        }
    }
}