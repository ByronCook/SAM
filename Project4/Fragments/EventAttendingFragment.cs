using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Project4.Activities;
using Project4.Adapters;
using Project4.Database.Core;
using Project4.Database.Entities;
using Project4.Models;
using Fragment = Android.Support.V4.App.Fragment;

namespace Project4.Fragments
{
    public class EventAttendingFragment : Fragment
    {
        private MainActivity _activity;
        private MyApplication _myApplication;

        private ScrollView _scrollView;
        private RadioButton _yesRadioButton;
        private RadioButton _maybeRadioButton;
        private RadioButton _noRadioButton;
        private TextView _eventNameTextView;
        private TextView _eventDescriptionTextView;
        private TextView _eventStartDateTextView;
        private TextView _eventEndDateTextView;
        private EditText _reasonEditText;
        private Button _confirmButton;

        private ListView _attendantsListView;
        private AttendantListItemAdapter _attendantListItemAdapter;

        private RelativeLayout _overlay;
        private DrawerLayout _activityLayout;

        private bool _overlayEnabled;

        private int _eventId;
        private Event _event;
        private List<Attendant> _attendants;
        private List<AttendantListItem> _attendantsListItems;
        private Attendant _myAttendant;
        private int _myAttendantState;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var bundle = Arguments;
            if (bundle != null)
            {
                _eventId = bundle.GetInt("event_id", 0);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.event_attendant, container, false);
            _activity = (MainActivity) Activity;
            _myApplication = _activity.ApplicationContext as MyApplication;

            SetupViews(view);
            Load();

            return view;
        }

        private void Load()
        {
            Task.Factory.StartNew(() =>
            {
                ToggleLayout();
                LoadEvent();
                LoadAttendants();
                ToggleLayout();
            }).ContinueWith(task =>
            {
                _attendantListItemAdapter.SetItems(_attendantsListItems.ToArray());
                LoadMyAttendant();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void LoadEvent()
        {
            var events = DaoManager.Get().EventDao.Find(_eventId);

            if (events.Count != 0)
            {
                _event = events[0];
                SetEventInfoOnTextViews(_event);
            }
        }

        private void SetEventInfoOnTextViews(Event currentEvent)
        {
            _activity.RunOnUiThread(() =>
            {
                _eventNameTextView.Text = currentEvent.Name;
                _eventDescriptionTextView.Text = currentEvent.Description;
                _eventStartDateTextView.Text = "From: " + $"{currentEvent.StartDate:dddd, MMMM d, yyyy HH:mm:ss}";
                _eventEndDateTextView.Text = "Until: " + $"{currentEvent.EndDate:dddd, MMMM d, yyyy HH:mm:ss}";
            });
        }

        private void LoadAttendants()
        {
            _attendants = DaoManager.Get().AttendantDao.Find("Event_Id", _eventId.ToString());

            //Setup new attendants if 0 are found
            if (_attendants.Count == 0)
            {
                var users = DaoManager.Get().TeamUserDao.FindByTeamId(_myApplication.CurrentTeam.Id);
                foreach (var teamUser in users)
                {
                    var newAttendant = new Attendant
                    {
                        Attending = AttendingState.Maybe,
                        EventId = _eventId,
                        Reason = "",
                        UserId = teamUser.UserId
                    };

                    var datatabasAttendant = DaoManager.Get().AttendantDao.Save(newAttendant);
                    _attendants.Add(datatabasAttendant);
                }
            }

            foreach (var attendant in _attendants)
            {
                if (attendant.UserId == _myApplication.CurrentUser.Id)
                {
                    _myAttendant = attendant;
                    break;
                }
            }

            _attendantsListItems = new List<AttendantListItem>();

            foreach (var attendant in _attendants)
            {
                var user = DaoManager.Get().UserDao.Find(attendant.UserId);

                var newItem = new AttendantListItem
                {
                    User = user[0],
                    AttendState = attendant.Attending,
                    Reason = attendant.Reason
                };
                _attendantsListItems.Add(newItem);
            }
        }

        private void LoadMyAttendant()
        {
            if (_myAttendant != null)
            {
                switch (_myAttendant.Attending)
                {
                    case AttendingState.Yes:
                        _yesRadioButton.Checked = true;
                        break;
                    case AttendingState.Maybe:
                        _maybeRadioButton.Checked = true;
                        _reasonEditText.Visibility = ViewStates.Visible;
                        _reasonEditText.Text = _myAttendant.Reason;
                        break;
                    case AttendingState.No:
                        _noRadioButton.Checked = true;
                        _reasonEditText.Visibility = ViewStates.Visible;
                        _reasonEditText.Text = _myAttendant.Reason;
                        break;
                }

                _confirmButton.Visibility = ViewStates.Visible;
            }
        }

        private void SetupViews(View view)
        {
            _scrollView = view.FindViewById<ScrollView>(Resource.Id.eat_scroll_view);

            _yesRadioButton = view.FindViewById<RadioButton>(Resource.Id.eat_attend_yes);
            _maybeRadioButton = view.FindViewById<RadioButton>(Resource.Id.eat_attend_maybe);
            _noRadioButton = view.FindViewById<RadioButton>(Resource.Id.eat_attend_no);

            _yesRadioButton.Click += RadioButtonClick;
            _maybeRadioButton.Click += RadioButtonClick;
            _noRadioButton.Click += RadioButtonClick;

            _eventNameTextView = view.FindViewById<TextView>(Resource.Id.eat_event_name);
            _eventDescriptionTextView = view.FindViewById<TextView>(Resource.Id.eat_event_description);
            _eventStartDateTextView = view.FindViewById<TextView>(Resource.Id.eat_event_startdate);
            _eventEndDateTextView = view.FindViewById<TextView>(Resource.Id.eat_event_enddate);

            _reasonEditText = view.FindViewById<EditText>(Resource.Id.eat_reason);
            _confirmButton = view.FindViewById<Button>(Resource.Id.eat_submit_button);

            _confirmButton.Click += ConfirmButtonClick;

            _overlay = _activity.FindViewById<RelativeLayout>(Resource.Id.overlay);
            _activityLayout = _activity.FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            _attendantsListView = view.FindViewById<ListView>(Resource.Id.eat_lv_attendants);
            _attendantsListView.Focusable = false;
            _attendantListItemAdapter = new AttendantListItemAdapter(_activity, Resource.Layout.item_attendant);
            _attendantsListView.Adapter = _attendantListItemAdapter;
        }

        private void ConfirmButtonClick(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                ToggleLayout();

                if (_myAttendant == null)
                {
                    _myAttendant = new Attendant
                    {
                        EventId = _eventId,
                        UserId = _myApplication.CurrentUser.Id
                    };
                }

                _myAttendant.Attending = _myAttendantState;
                _myAttendant.Reason = _reasonEditText.Text;
                
                DaoManager.Get().AttendantDao.Save(_myAttendant);

                ToggleLayout();
            }).ContinueWith(task =>
            {
                Load();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void RadioButtonClick(object sender, EventArgs e)
        {
            var radioButton = (RadioButton) sender;

            switch (radioButton.Id)
            {
                case Resource.Id.eat_attend_yes:
                    _myAttendantState = AttendingState.Yes;
                    _reasonEditText.Visibility = ViewStates.Invisible;
                    _reasonEditText.Text = "";
                    break;
                case Resource.Id.eat_attend_maybe:
                    _myAttendantState = AttendingState.Maybe;
                    _reasonEditText.Visibility = ViewStates.Visible;
                    break;
                case Resource.Id.eat_attend_no:
                    _myAttendantState = AttendingState.No;
                    _reasonEditText.Visibility = ViewStates.Visible;
                    break;
            }
            _confirmButton.Visibility = ViewStates.Visible;
        }

        private void ToggleLayout()
        {
            if (_overlayEnabled)
            {
                SetActivityLayoutChildElementsEnabled(true);
                ShowOverlay(false);
                _overlayEnabled = false;
            }
            else
            {
                SetActivityLayoutChildElementsEnabled(false);
                ShowOverlay(true);
                _overlayEnabled = true;
            }
        }

        private void ShowOverlay(bool show)
        {
            _activity.RunOnUiThread(() =>
            {
                _overlay.Visibility = show ? ViewStates.Visible : ViewStates.Invisible;
            });
        }

        private void SetActivityLayoutChildElementsEnabled(bool enabled)
        {
            _activity.RunOnUiThread(() =>
            {
                SetChildsEnabled(_activityLayout, enabled);
            });
        }

        private static void SetChildsEnabled(ViewGroup vg, bool enabled)
        {
            for (var i = 0; i < vg.ChildCount; i++)
            {
                var child = vg.GetChildAt(i);
                child.Enabled = enabled;

                if (child is ViewGroup @group)
                {
                    SetChildsEnabled(@group, enabled);
                }
            }
        }

        public static class AttendingState
        {
            public const int No = 0;
            public const int Maybe = 1;
            public const int Yes = 2;
        }

    }
}