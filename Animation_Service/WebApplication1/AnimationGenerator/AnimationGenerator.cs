using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using WebApplication1.Services;

namespace WebApplication1.AnimationGenerator
{
    public class AnimationGenerator
    {
        protected PersistenceService persistence = new PersistenceService();
        protected static string BASE_DIR = ConfigurationManager.AppSettings["BASE_DIR"];
        protected static string DATASTORE_FOLDER = ConfigurationManager.AppSettings["DATASTORE_FOLDER"];
        protected static string ANIMATIONS_FOLDER = ConfigurationManager.AppSettings["ANIMATIONS_FOLDER"];

        public string Name { get; set; }
    }
}