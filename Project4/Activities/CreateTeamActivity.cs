using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Project4.Database.Core;
using Project4.Database.Entities;
using Project4.Dialogs;

namespace Project4.Activities
{
    [Activity(Label = "CreateTeamActivity")]
    public class CreateTeamActivity : BaseActivity
    {
        private const int NoResult = -1;
        private const int NoError = 0;
        private const int NoTeamName = 1;
        private const int NoClub = 2;
        private const int TeamNameAlreadyTaken = 3;

        private Button _createTeamButton;
        private EditText _teamName;
        private Spinner _clubDropdown;
        private RelativeLayout _overlay;

        private ArrayAdapter<string> _clubAdapter;
        private Dictionary<string, int> _clubDictionary;

        private int _clubId;
        private Club[] _clubs;

        private MyApplication _application;

        private bool _overlayEnabled;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            OnActivityCreate(savedInstanceState, Resource.Layout.team_create);
            SetActivityTitle(Resource.String.header_create_team);
            EnableActionBarBackButton();
            UseOverlay();

            SetupViews();

            _application = ApplicationContext as MyApplication;
        }

        private void SetupViews()
        {
            _createTeamButton = FindViewById<Button>(Resource.Id.ucr_create_button);
            _createTeamButton.Click += OnClickSubmit;

            _teamName = FindViewById<EditText>(Resource.Id.ucr_name_input);
            _teamName.EditorAction += HandleEditorAction;

            _clubDropdown = FindViewById<Spinner>(Resource.Id.ucr_club_dropdown);
            _clubDropdown.ItemSelected += HandleClubSelect;

            _overlay = FindViewById<RelativeLayout>(Resource.Id.overlay);
        }

        protected override void OnStart()
        {
            base.OnStart();

            _clubDictionary = new Dictionary<string, int>();

            Task.Factory.StartNew(() =>
            {
                ShowOverlay(true);
                SetActivityLayoutChildElementsEnabled(false);

                _clubs = DaoManager.Get().ClubDao.FindAll().ToArray();

                _clubDictionary.Add(GetString(Resource.String.spn_club_select), 0);

                foreach (var club in _clubs)
                {
                    _clubDictionary.Add(club.Name, club.Id);
                }

                ShowOverlay(false);
                SetActivityLayoutChildElementsEnabled(true);
            }).ContinueWith(task =>
            {
                _clubAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, new List<string>(_clubDictionary.Keys));
                _clubDropdown.Adapter = _clubAdapter;

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OnClickSubmit(object sender, EventArgs e)
        {
            var result = NoResult;
            Task.Factory.StartNew(() =>
            {
                ToggleLayout();

                while (result == NoResult)
                {
                    if (string.IsNullOrEmpty(_teamName.Text))
                    {
                        result = NoTeamName;
                    }
                    else if (_clubId == 0)
                    {
                        result = NoClub;
                    }

                    var foundTeams = DaoManager.Get().TeamDao.Count("name", _teamName.Text);

                    if (foundTeams > 0)
                    {
                        result = TeamNameAlreadyTaken;
                    }
                    else
                    {
                        var newTeam = new Team
                        {
                            Name = _teamName.Text,
                            ClubId = _clubId
                        };

                        newTeam = DaoManager.Get().TeamDao.Save(newTeam);

                        var newTeamUser = new TeamUser()
                        {
                            TeamId = newTeam.Id,
                            UserId = _application.CurrentUser.Id
                        };

                        DaoManager.Get().TeamUserDao.Save(newTeamUser);
                        result = NoError;
                    }
                }

                ToggleLayout();
            }).ContinueWith(task =>
            {
                ProcessResult(result);
            }, TaskScheduler.FromCurrentSynchronizationContext());

            
        }

        public override void OnBackPressed()
        {
            StartActivity(new Intent(this, typeof(ChooseTeamActivity)));
            Finish();
        }

        private void HandleClubSelect(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var spinner = (Spinner) sender;
            var item = spinner.GetItemAtPosition(e.Position).ToString();
            _clubId = _clubDictionary[item];
        }

        private void ProcessResult(int resultCode)
        {
            switch (resultCode)
            {
                case NoError:
                    OnBackPressed();
                    break;
                case NoTeamName:
                    new ErrorDialog(this, Resource.String.dlg_no_team_name_title, Resource.String.dlg_no_team_name).Display();
                    break;
                case NoClub:
                    new ErrorDialog(this, Resource.String.dlg_no_club_title, Resource.String.dlg_no_club).Display();
                    break;
                case TeamNameAlreadyTaken:
                    new ErrorDialog(this, Resource.String.dlg_team_name_already_taken_title, Resource.String.dlg_team_name_already_taken).Display();
                    break;
                default:
                    OnBackPressed();
                    break;
            }
        }

        /// <summary>
        /// Show/hide the overlay and enable/disable everything behind the overlay.
        /// </summary>
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

    }
}