using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using WebApplication1.Services;

namespace WebApplication1.Animations
{
    public class Animation
    {
        protected PersistenceService persistence = new PersistenceService();
        protected static string BASE_DIR = ConfigurationManager.AppSettings["BASE_DIR"];
        protected static string DATASTORE_FOLDER = ConfigurationManager.AppSettings["DATASTORE_FOLDER"];
        protected static string ANIMATIONS_FOLDER = ConfigurationManager.AppSettings["ANIMATIONS_FOLDER"];


        public string Name { get; set; }
        public string Initialization { get; set; }
        //public IList<MapsObjects> MapsObjects { get; set; }
    }
}