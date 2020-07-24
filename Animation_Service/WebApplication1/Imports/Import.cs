using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using WebApplication1.Services;

namespace WebApplication1.Imports
{
    public class Import
    {
        protected PersistenceService persistence = new PersistenceService();
        protected string BASE_DIR = ConfigurationManager.AppSettings["BASE_DIR"];
        protected string SRC_FOLDER = ConfigurationManager.AppSettings["SRC_FOLDER"];
        protected string DATASTORE_FOLDER = ConfigurationManager.AppSettings["DATASTORE_FOLDER"];
    }
}