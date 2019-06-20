using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Project4.Activities;
using Project4.Adapters;
using Project4.Database.Core;
using Project4.Database.Entities;
using Project4.Dialogs;
using Project4.Models;
using Fragment = Android.Support.V4.App.Fragment;

namespace Project4.Fragments
{
    public class CreateEventFragment : Fragment
    {
        private List<EventItem> Events { get; set; }
        private List<KeyValuePair<string, string>> EventTypes;
        private View _view;
        private int _selectedItem;
        private MyApplication _myApplication;
        private DateTime StartDate { get; set; }
        private DateTime EndDate { get; set; }
        private bool IsStartDate { get; set; }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _myApplication = Activity.ApplicationContext as MyApplication;
            // Create your fragment here
        }

        public static CreateEventFragment NewInstance()
        {
            var calendarFragment = new CreateEventFragment { Arguments = new Bundle() };
            return calendarFragment;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);

            _view = inflater.Inflate(Resource.Layout.create_event_item, container, false);
            var activity = (MainActivity) Activity;
            var dropDown = _view.FindViewById<Spinner>(Resource.Id.DropDownList);
            CreateEventTypeList();
            var events = new List<string>();
            foreach (var eventType in EventTypes)
            {
                events.Add(eventType.Key);
            }

            var button = _view.FindViewById<Button>(Resource.Id.submitEvent);
            button.Click += OnSubmit;

            var setDateButton = _view.FindViewById<Button>(Resource.Id.SetStartDate);
            setDateButton.Click += delegate
            {
                IsStartDate = true;
                SetDate();
            };
            var setEndButton = _view.FindViewById<Button>(Resource.Id.SetEndDate);
            setEndButton.Click += delegate
            {
                IsStartDate = false;
                SetDate();
            };

            var adapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleSpinnerItem, events);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            dropDown.Adapter = adapter;

            _view.FindViewById<Spinner>(Resource.Id.DropDownList).ItemSelected += CreateEventFragment_ItemSelected;
            return _view;
        }

        private void CreateEventFragment_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            _selectedItem = Convert.ToInt32(EventTypes[e.Position].Value);
        }

        private void OnSubmit(object sender, EventArgs e)
        {
            var nameTextView = _view.FindViewById<TextView>(Resource.Id.EventName);
            var descriptionTextView = _view.FindViewById<TextView>(Resource.Id.EventDescription);
            //var startDateView = _view.FindViewById<TextView>(Resource.Id.StartDate);
           // var endDateView = _view.FindViewById<TextView>(Resource.Id.EndDate);

            var result = EventCreationErrorType.NoError;
            Task.Factory.StartNew(() =>
            {
                while (result == EventCreationErrorType.NoError)
                {
                    if (string.IsNullOrEmpty(nameTextView.Text))
                    {
                        result = EventCreationErrorType.NoName;
                        break;
                    }
                    else if (StartDate == DateTime.MinValue)
                    {
                        result = EventCreationErrorType.NoStartDate;
                        break;
                    }
                    else if (EndDate == DateTime.MinValue)
                    {
                        result = EventCreationErrorType.NoEndDate;
                        break;
                    } else if (_selectedItem == 0)
                    {
                        result = EventCreationErrorType.NoType;
                        break;
                    }

                    else if (DateTime.Compare(StartDate, EndDate) > 0)
                    {
                        result = EventCreationErrorType.PrimaryDateSmallerThanSecondary;
                        break;
                    }

                    else if (DateTime.Compare(StartDate, DateTime.Now) < 0)
                    {
                        result = EventCreationErrorType.DateAlreadyPassed;
                        break;
                    }

                    var foundTeams = DaoManager.Get().EventDao.Count("name", _view.FindViewById<TextView>(Resource.Id.EventName).Text);

                    if (foundTeams > 0 && result == EventCreationErrorType.NoError)
                    {
                        result = EventCreationErrorType.EventAlreadyExists;
                        break;
                    }
                    else
                    {
                        var newEvent = new Event
                        {
                            Name = nameTextView.Text,
                            Description = descriptionTextView.Text,
                            EventType = _selectedItem,
                            StartDate = StartDate,
                            EndDate = EndDate,
                            Team = _myApplication.CurrentTeam.Id
                        };

                        DaoManager.Get().EventDao.Save(newEvent);

                        result = EventCreationErrorType.NoResult;
                    }
                }
            }).ContinueWith(task =>
            {
               ProcessResult(result);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void SetDate()
        {
            DateTime date;
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime setDate)
            {
                date = setDate;

                TimePickerFragment frag2 = TimePickerFragment.NewInstance(
                    delegate (TimeSpan time)
                    {
                        //StartDate = date.Add(time);
                        if (IsStartDate)
                        {
                            StartDate = date.Add(time);
                            _view.FindViewById<TextView>(Resource.Id.StartDate).Text = date.ToLongDateString();
                        }
                        else
                        {
                            EndDate = date.Add(time);
                            _view.FindViewById<TextView>(Resource.Id.EndDate).Text = date.ToLongDateString();
                        }
                    });

                frag2.Show(Activity.FragmentManager, TimePickerFragment.TAG);
            });
            frag.Show(Activity.FragmentManager, DatePickerFragment.TAG);
        }
        private void CreateEventTypeList()
        {
            EventTypes = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EventType.Match.ToString(), "1"),
                new KeyValuePair<string, string>(EventType.Training.ToString(), "2"),
                new KeyValuePair<string, string>(EventType.PracticeMatch.ToString(), "3"),
                new KeyValuePair<string, string>(EventType.RunTraining.ToString(), "4"),
                new KeyValuePair<string, string>(EventType.Birthday.ToString(), "5"),
                new KeyValuePair<string, string>(EventType.TeamActivity.ToString(), "6"),
                new KeyValuePair<string, string>(EventType.Tournament.ToString(), "7"),
                new KeyValuePair<string, string>(EventType.Party.ToString(), "8"),

            };
        }
        private void ProcessResult(EventCreationErrorType resultCode)
        {
            switch (resultCode)
            {
                case EventCreationErrorType.NoResult:
                case EventCreationErrorType.NoError:
                    Activity.OnBackPressed();
                    break;
                case EventCreationErrorType.NoName:
                    new ErrorDialog(Activity, Resource.String.dlg_no_event_name_title, Resource.String.dlg_no_event_name).Display();
                    break;
                case EventCreationErrorType.NoStartDate:
                    new ErrorDialog(Activity, Resource.String.dlg_no_event_startdate_title, Resource.String.dlg_no_event_startdate).Display();
                    break;
                case EventCreationErrorType.NoEndDate:
                    new ErrorDialog(Activity, Resource.String.dlg_no_event_enddate_title, Resource.String.dlg_no_event_enddate).Display();
                    break;
                case EventCreationErrorType.EventAlreadyExists:
                    new ErrorDialog(Activity, Resource.String.dlg_event_already_exists_title, Resource.String.dlg_event_already_exists).Display();
                    break;
                case EventCreationErrorType.NoType:
                    new ErrorDialog(Activity, Resource.String.dlg_no_event_type_title, Resource.String.dlg_no_event_type).Display();
                    break;
                case EventCreationErrorType.PrimaryDateSmallerThanSecondary:
                    new ErrorDialog(Activity, Resource.String.dlg_primary_date_later_than_secondary_title, Resource.String.dlg_primary_date_later_than_secondary).Display();
                    break;
                case EventCreationErrorType.DateAlreadyPassed:
                    new ErrorDialog(Activity, Resource.String.dlg_date_has_already_passed_title, Resource.String.dlg_date_has_already_passed).Display();
                    break;
                default:
                    Activity.OnBackPressed();
                    break;
            }
        }
    }
}