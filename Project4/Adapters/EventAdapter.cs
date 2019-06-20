using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Project4.Models;

namespace Project4.Adapters
{
    public class EventAdapter : BaseAdapter<EventItem>
    {
        public EventAdapter(Context context, int itemLayoutResource, EventItem[] items) : base(context, itemLayoutResource, items)
        {

        }

        public override View CreateView(int position, View convertView, ViewGroup parent)
        {
            var selectedEvent = this[position];

            convertView.FindViewById<TextView>(Resource.Id.Title).Text = selectedEvent.Name;
            var dateString = string.Format("{0:dddd, MMMM d, yyyy}", selectedEvent.StartDate);
            var endDateString = string.Format("{0:dddd, MMMM d, yyyy}", selectedEvent.EndDate);
            convertView.FindViewById<TextView>(Resource.Id.Date).Text = dateString + " - " + endDateString;
            
            return convertView;
        }
    }
}