using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class AnimationInformation
    {
        public string AnimationKey { get; set; }
        public string AnimationName { get; set; }
        public string AnimationDescription { get; set; }
        public string InitUri { get; set; }
        public string RunCommand { get; set; }
    }
}