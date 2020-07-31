using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.MapsObjects
{
    public class PolygonUpdate : MapsObject
    {
        public string StrokeColor { get; set; }
        public string FillColor { get; set; }
        public string FillOpacity { get; set; }
        public bool Paint { get; set; }
    }
}