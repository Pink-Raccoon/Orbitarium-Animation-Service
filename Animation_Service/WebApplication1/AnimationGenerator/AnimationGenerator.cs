using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using WebApplication1.Services;
using WebApplication1.Models;
using Newtonsoft.Json;

namespace WebApplication1.AnimationGenerator
{
    public class AnimationGenerator
    {
        protected PersistenceService persistence = new PersistenceService();
        protected static string BASE_DIR = ConfigurationManager.AppSettings["BASE_DIR"];
        protected static string DATASTORE_FOLDER = ConfigurationManager.AppSettings["DATASTORE_FOLDER"];
        protected static string ANIMATIONS_FOLDER = ConfigurationManager.AppSettings["ANIMATIONS_FOLDER"];

        protected AnimationInformation animationInformation { get; set; }


        protected void GenerateAnimationInformation()
        {
            var animationsSummaryPath = Path.Combine(BASE_DIR, "animations_information.json");
            Dictionary<string, AnimationInformation> animationsSummary;
            if (File.Exists(animationsSummaryPath))
            {
                animationsSummary = JsonConvert.DeserializeObject<Dictionary<string, AnimationInformation>>(File.ReadAllText(animationsSummaryPath));
            }
            else
            {
                animationsSummary = new Dictionary<string, AnimationInformation>();
            }
            if (animationsSummary.ContainsKey(animationInformation.AnimationKey))
            {
                animationsSummary[animationInformation.AnimationKey] = animationInformation;
               
            } else
            {
                animationsSummary.Add(animationInformation.AnimationKey, animationInformation);
            }

            PersistAnimationsSummary(animationsSummary);
        }

        private void PersistAnimationsSummary(Dictionary<string, AnimationInformation> animationsSummary)
        {
            var animationsSummaryPath = Path.Combine(BASE_DIR, "animations_information.json");
            var serialized = JsonConvert.SerializeObject(animationsSummary);
            File.WriteAllText(animationsSummaryPath, serialized);
        }
    }

    
}