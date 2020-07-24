using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class ImportService
    {
        private PersistenceService persistence = new PersistenceService();
        private static string BASE_DIR = ConfigurationManager.AppSettings["BASE_DIR"];
        private static string SRC_FOLDER = ConfigurationManager.AppSettings["SRC_FOLDER"];
        private static string DATASTORE_FOLDER = ConfigurationManager.AppSettings["DATASTORE_FOLDER"];

        //public string[] ImportData(string type)
        //{
        //    if (type.ToLower().Equals("countries"))
        //    {
        //        return ImportCountries();
        //    }
        //    else if (type.ToLower().Equals("coronadata"))
        //    {
        //        return ImportCoronaData();
        //    }
        //    else
        //    {
        //        return new string[] { "error", "data " + type + " has no corresponding import function" };
        //    }
        //}

        

        


    }
}