using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.MapsObjects
{
    public class Polygon : MapsObject
    {
        public IList<Coordinate>[] Paths { get; set; }
        public string StrokeColor { get; set; }
        public double StrokeOpacity { get; set; }
        public int StrokeWeight { get; set; }
        public string FillColor { get; set; }
        public double FillOpacity { get; set; }
    }
}