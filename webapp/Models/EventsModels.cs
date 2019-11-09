using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class EventsModels
    {
        public string id { get; set; }
        public string title { get; set; }
        public Nullable<System.DateTime> start { get; set; }
        public Nullable<int> startHour { get; set; }
        public Nullable<int> startMinute { get; set; }
        //public Nullable<int> startSecond { get; set; }
        public Nullable<System.DateTime> end { get; set; }
        public Nullable<int> endHour { get; set; }
        public Nullable<int> endMinute { get; set; }
        //public Nullable<int> endSecond { get; set; }
        public string className { get; set; }
        public string icon { get; set; }
        public string description { get; set; }
        public string category { get; set; }
        public bool allDay { get; set; }
        public string kod { get; set; }
        public Nullable<System.DateTime> date { get; set; }
        public string place { get; set; }
        public string gred { get; set; }
    }
   
}