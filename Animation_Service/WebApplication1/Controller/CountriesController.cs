using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Services;

namespace WebApplication1.Controller
{
    public class CountriesController : ApiController
    {
        // GET api/<controller>
        public string[] Get()
        {
            
            var appSettings = ConfigurationManager.AppSettings;
            string result = appSettings["MyCustomSetting"] ?? "Not Found";
            //return result;

            CountryImportService s = new CountryImportService();
            return s.ImportCountries();
                
            //PersistenceService p = new PersistenceService();
            //return p.ParseFiles();
            //return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}