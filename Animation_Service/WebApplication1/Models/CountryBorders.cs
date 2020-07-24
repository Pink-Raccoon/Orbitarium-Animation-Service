using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class CountryBorders
    {
        public string Name { get; set; }
        public IList<Coordinate>[] Path { get; set; }
    }
}