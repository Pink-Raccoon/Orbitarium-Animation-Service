using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class DateCountryInfection
    {
        public DateTime Date { get; set; }
        public string Country { get; set; }
        public int ConfirmedInfections { get; set; }
        public int Deaths { get; set; }
        public int Recovered { get; set; }
        public int Active { get; set; }
        public int NewCases { get; set; }
        public int NewDeaths { get; set; }
        public int NewRecovered { get; set; }
    }
}