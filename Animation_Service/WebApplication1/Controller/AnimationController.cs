using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication1.Services;

namespace WebApplication1.Controller
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AnimationController : ApiController
    {
        // GET api/<controller>
        public string Get()
        {
            AnimationService animationService = new AnimationService();
            return animationService.GetAnimation();
        }

        // GET api/<controller>/zimbabwe
        public string Get(string name)
        {
            AnimationService a = new AnimationService();
            return a.getDataFor(name);
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