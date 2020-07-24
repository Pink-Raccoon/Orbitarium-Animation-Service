using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class CountryPopulation
    {
        public string CountryName { get; set; }
        public int Population { get; set; }
        public double YearlyChange { get; set; }
        public int NetChange { get; set; }
        public int Density { get; set; }
        public int LandArea { get; set; }
        public int Migrants { get; set; }
        public double FertilityRate { get; set; }
        public int MedianAge { get; set; }
        public double UrbanPopulation { get; set; }
        public double WorldShare { get; set; }
    }
}