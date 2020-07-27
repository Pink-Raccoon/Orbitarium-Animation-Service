using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication1.Animations;
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
            return a.GetDataFor(name);
        }

        // POST api/<controller>
        public string Post([FromBody]string value)
        {
            var splitted = value.Split(',');
            var action = splitted[0];
            var animationName = splitted[1];
            if (action.ToLower().Equals("generate"))
            {
                if (animationName.ToLower().Equals("corona_spread"))
                {
                    var coronaSpread = new CoronaSpreadWorldwide();
                    coronaSpread.GenerateAnimation();

                }
            }
            return "success";
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