using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.OS;
using Fragment = Android.Support.V4.App.Fragment;
using Android.Views;
using Android.Widget;
using Project4.Activities;
using Project4.Adapters;
using Project4.Database.Core;
using Project4.Models;
using Project4.Provider;

namespace Project4.Fragments
{
    public class CalendarFragment : Fragment
    {
        private MyApplication _myApplication;
        private readonly List<EventItem> _events = new List<EventItem>();
        private EventAdapter _eventAdapter;

        private ListView _calendarList;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnStart()
        {
            base.OnStart();
            LoadEvents();
        }

        private void LoadEvents()
        {
            _events.Clear();
            Task.Factory.StartNew(() =>
            {
                var databaseEvents =
                    DaoManager.Get().EventDao.Find("Team_Id", _myApplication.CurrentTeam.Id.ToString());
                foreach (var databaseEvent in databaseEvents)
                {
                    _events.Add(new EventItem
                    {
                        Id = databaseEvent.Id,
                        Name = databaseEvent.Name,
                        StartDate = databaseEvent.StartDate,
                        EndDate = databaseEvent.EndDate,
                        EventType = databaseEvent.EventType
                    });
                }
            }).ContinueWith(task =>
            {
                _eventAdapter = new EventAdapter(Activity, Resource.Layout.calendar_list_item, _events.ToArray());
                _calendarList.Adapter = _eventAdapter;
                _eventAdapter.NotifyDataSetChanged();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public static Fragment NewInstance()
        {
            var calendarFragment = new CalendarFragment { Arguments = new Bundle() };
            return calendarFragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.calendar, container, false);
            var activity = (MainActivity)Activity;
            _myApplication = activity.ApplicationContext as MyApplication;

            _calendarList = view.FindViewById<ListView>(Resource.Id.calendarList);

            var sortedEvents = _events.OrderBy(t => t.StartDate).ThenBy(t => t.EndDate);

            _eventAdapter = new EventAdapter(Activity, Resource.Layout.calendar_list_item, sortedEvents.ToArray());

            _calendarList.Adapter = _eventAdapter;

            _calendarList.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs e)
            {
                var item = _events[e.Position];
                var fragment = new EventAttendingFragment();
                var bundle = new Bundle();
                bundle.PutInt("event_id", item.Id);
                fragment.Arguments = bundle;
                fragment.OnAttach(Activity.ApplicationContext);

                Activity.SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.content_frame, fragment).AddToBackStack("tag")
                    .Commit();
            };

            var button = view.FindViewById<Button>(Resource.Id.CreateEvent);
            button.Click += Button_Click;

            return view;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Activity.SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_frame, CreateEventFragment.NewInstance()).AddToBackStack("tag")
                .Commit();
        }
    }
}