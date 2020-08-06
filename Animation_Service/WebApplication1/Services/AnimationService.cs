using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApplication1.Services
{
    public class AnimationService
    {
        private static string BASE_DIR = ConfigurationManager.AppSettings["BASE_DIR"];
        private static string ANIMATIONS_STORAGE_FOLDER = ConfigurationManager.AppSettings["ANIMATIONS_STORAGE_FOLDER"];

        public string GetAnimationsSummary()
        {
            var animationsSummaryPath = Path.Combine(BASE_DIR, "animations_information.json");
            if (File.Exists(animationsSummaryPath))
            {
                return File.ReadAllText(animationsSummaryPath);
            }
            else
            {
                return "noanimations";
            }
        }
    }
}