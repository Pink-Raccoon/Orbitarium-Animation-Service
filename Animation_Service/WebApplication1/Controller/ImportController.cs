using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Imports;
using WebApplication1.Services;

namespace WebApplication1.Controller
{
    public class ImportController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public string[] Post([FromBody]string type)
        {
            if (type.ToLower().Equals("countries"))
            {
                var import = new CountryBordersImport();
                return import.ImportCountries();
                
            }
            else if (type.ToLower().Equals("coronadata"))
            {
                var import = new CoronaDataImport();
                return import.ImportCoronaData();
            }
            else
            {
                return new string[] { "error", "data " + type + " has no corresponding import function" };
            }



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