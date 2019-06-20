using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Project4.Adapters;
using Project4.Database.Core;
using Project4.Models;
using Project4.Provider;

namespace Project4.Activities
{
    [Activity(Label = "ChooseTeamActivity")]
    public class ChooseTeamActivity : BaseActivity
    {
        private ListView _teamListView;
        private Button _createTeamButton;

        private TeamListItemAdapter _adapter;

        private MyApplication _application;

        private List<TeamListItem> _teamListItems;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            OnActivityCreate(savedInstanceState, Resource.Layout.team_choose);
            SetActivityTitle(Resource.String.header_choose_team);
            EnableActionBarBackButton();

            SetupViews();

            _application = ApplicationContext as MyApplication;
        }

        private void SetupViews()
        {
            _teamListView = FindViewById<ListView>(Resource.Id.tch_lv_teams);
            _adapter = new TeamListItemAdapter(this, Resource.Layout.item_team_view);
            _teamListView.Adapter = _adapter;
            _teamListView.ItemClick += TeamListItemClick;

            _createTeamButton = FindViewById<Button>(Resource.Id.tch_btn_create_new_team);
            _createTeamButton.Click += CreateTeamClick;
        }

        protected override void OnStart()
        {
            base.OnStart();

            _teamListItems = new List<TeamListItem>();

            Task.Factory.StartNew(() =>
            {
                var userTeams = DaoManager.Get().TeamUserDao.Find("User_Id", _application.CurrentUser.Id.ToString());

                var teamIds = userTeams.Select(ut => ut.TeamId.ToString()).ToList();
                var teams = DaoManager.Get().TeamDao.FindByList("Team_Id", teamIds);

                var clubIds = teams.Select(t => t.ClubId.ToString()).Distinct().ToList();
                var clubs = DaoManager.Get().ClubDao.FindByList("Club_Id", clubIds);

                foreach (var team in teams)
                {
                    foreach (var club in clubs)
                    {
                        if (team.ClubId == club.Id)
                        {
                            _teamListItems.Add(new TeamListItem
                            {
                                Team = team,
                                Club = club
                            });
                        }
                    }
                }
            }).ContinueWith(task => {
                _adapter.SetItems(_teamListItems.ToArray());
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void TeamListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var clickedItem = _teamListItems[e.Position];
            _application.CurrentTeam = clickedItem.Team;
            _application.CurrentClub = clickedItem.Club;

            StartActivity(new Intent(this, typeof(MainActivity)));
            Finish();
        }

        private void CreateTeamClick(object sender, EventArgs e)
        {
            StartActivity(new Intent(this, typeof(CreateTeamActivity)));
            Finish();
        }

        public override void OnBackPressed()
        {
            CredentialsProvider.Logout(_application);
            StartActivity(new Intent(this, typeof(LoginActivity)));
            Finish();
        }
    }
}