using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using WebApplication1.MapsObjects;
using WebApplication1.Services;

namespace WebApplication1.AnimationDisplay
{
    public static class AnimationDisplay
    {
        public static string TimeStamp { get; set; }
        public static Dictionary<string, PolygonUpdate> Updates { get; set; }
        public static OrderedDictionary Timeline { get; set; }
        public static int Step { get; set; }
        public static bool IsCurrentStepSent { get; set; }
        public static bool IsPlaying { get; set; }
        public static bool Stopped { get; set; }

        public static void LoadAnimation(string animationName)
        {
            PersistenceService persistence = new PersistenceService();
            string BASE_DIR = ConfigurationManager.AppSettings["BASE_DIR"];
            string ANIMATIONS_FOLDER = ConfigurationManager.AppSettings["ANIMATIONS_FOLDER"];
            var srcPath = Path.Combine(BASE_DIR, ANIMATIONS_FOLDER, animationName, "corona_spread_updates.json");
            var updateString = File.ReadAllText(srcPath);
            OrderedDictionary polygonUpdateTimeline = JsonConvert.DeserializeObject<OrderedDictionary>(updateString);
            var enumerator = polygonUpdateTimeline.GetEnumerator();
            var polygonUpdateTimelineDeserialized = new OrderedDictionary();
            while (enumerator.MoveNext())
            {
                var date = enumerator.Key;
                var polygonUpdates = JsonConvert.DeserializeObject<Dictionary<string, PolygonUpdate>>(enumerator.Value.ToString());
                polygonUpdateTimelineDeserialized.Add(date, polygonUpdates);
            }
            var firstElement = polygonUpdateTimelineDeserialized.Cast<DictionaryEntry>().ElementAt(0);
            TimeStamp = firstElement.Key.ToString();
            Updates = (Dictionary<string, PolygonUpdate>) firstElement.Value;
            Step = 0;
            Timeline = polygonUpdateTimelineDeserialized;
        }

        private static void MoveToNextTimeStep()
        {
            while (IsPlaying)
            {
                if (Timeline.Count < (Step + 1))
                {
                    Step = 0;
                }
                var element = Timeline.Cast<DictionaryEntry>().ElementAt(Step);
                TimeStamp = element.Key.ToString();
                Updates = (Dictionary<string, PolygonUpdate>)element.Value;
                IsCurrentStepSent = false;
                Step++;
                Thread.Sleep(1000);
            }
        }

        public static void Start()
        {
            IsCurrentStepSent = false;
            IsPlaying = true;
            Stopped = false;
            Thread t = new Thread(new ThreadStart(MoveToNextTimeStep));
            t.Start();
        }

        public static void Stop()
        {
            IsPlaying = false;
            Stopped = true;
        }

        public static string GetAnimationUpdates()
        {
            if (!IsCurrentStepSent)
            {
                IsCurrentStepSent = true;
                return JsonConvert.SerializeObject(new { TimeStamp = TimeStamp, Step = Step, Updates = Updates });
            } else
            {
                return JsonConvert.SerializeObject("noop");
            }
            
        }

    }

    
}