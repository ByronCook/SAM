using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Project4.Models
{
    public class EventItem
    {
        public int Id;
        public string Name;
        public string Description;
        public int EventType;
        public DateTime StartDate;
        public DateTime EndDate;
    }
}