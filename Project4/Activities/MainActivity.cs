using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Project4.Fragments;
using Acr.UserDialogs;
using Android.Widget;
using Project4.Provider;


namespace Project4.Activities
{
    [Activity(Label = "@string/app_name", LaunchMode = LaunchMode.SingleTop, Icon = "@drawable/Icon")]
    public class MainActivity : AppCompatActivity
    {
        private DrawerLayout _drawerLayout;
        private NavigationView _navigationView;

        private MyApplication _application;

        private IMenuItem _previousItem;

        private int[] _currentPadding = { 0, 0, 0, 0 };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.main);
            
            RequestedOrientation = ScreenOrientation.Portrait;

            UserDialogs.Init(this);

            _application = ApplicationContext as MyApplication;

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if (toolbar != null)
            {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
            }

            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            //Set hamburger items menu
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);

            //setup navigation view
            _navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            var view = _navigationView.GetHeaderView(0);

            var usernameTextView = view.FindViewById<TextView>(Resource.Id.nav_header_username);
            usernameTextView.Text = _application.CurrentUser.Username;

            var nameTextView = view.FindViewById<TextView>(Resource.Id.nav_header_name);
            nameTextView.Text = _application.CurrentUser.Firstname + " " + _application.CurrentUser.Lastname;

            var emailTextView = view.FindViewById<TextView>(Resource.Id.nav_header_email);
            emailTextView.Text = _application.CurrentUser.Email;

            var changeTeamTextView = _navigationView.FindViewById<TextView>(Resource.Id.nav_footer_item_change_team);
            changeTeamTextView.Click += ChangeTeam;

            var logoutTextView = _navigationView.FindViewById<TextView>(Resource.Id.nav_footer_item_logout);
            logoutTextView.Click += Logout;

            var layout = FindViewById<RelativeLayout>(Resource.Id.content_frame_wrapper);
            _currentPadding[0] = layout.PaddingLeft;
            _currentPadding[1] = layout.PaddingTop;
            _currentPadding[2] = layout.PaddingRight;
            _currentPadding[3] = layout.PaddingBottom;

            //handle navigation
            _navigationView.NavigationItemSelected += (sender, e) =>
            {
                _previousItem?.SetChecked(false);

                _navigationView.SetCheckedItem(e.MenuItem.ItemId);

                _previousItem = e.MenuItem;

                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_calendar:
                        ListItemClicked(0);
                        break;
                    case Resource.Id.nav_news:
                        ListItemClicked(1);
                        break;
                    case Resource.Id.nav_member_list:
                        ListItemClicked(2);
                        break;
                }
                
                _drawerLayout.CloseDrawers();
            };


            //if first time you will want to go ahead and click first item.
            if (savedInstanceState == null)
            {
                _navigationView.SetCheckedItem(Resource.Id.nav_calendar);
                var fragment = CalendarFragment.NewInstance();
                fragment.OnAttach(ApplicationContext);

                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.content_frame, fragment)
                    .Commit();
            }
        }

        private int _oldPosition = -1;

        private void ListItemClicked(int position)
        {
            //this way we don't load twice, but you might want to modify this a bit.
            if (position == _oldPosition)
                return;

            _oldPosition = position;

            Android.Support.V4.App.Fragment fragment = null;
            switch (position)
            {
                case 0:
                    fragment = CalendarFragment.NewInstance();
                    fragment.OnAttach(ApplicationContext);
                    break;
                case 1:
                    fragment = new NewsListFragment();
                    fragment.OnAttach(ApplicationContext);
                    break;
                case 2:
                    fragment = new MemberListFragment();
                    fragment.OnAttach(ApplicationContext);
                    break;
            }

            if (fragment != null)
            {
                var layout = FindViewById<RelativeLayout>(Resource.Id.content_frame_wrapper);
                layout.SetPadding(_currentPadding[0], _currentPadding[1], _currentPadding[2], _currentPadding[3]);

                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.content_frame, fragment).AddToBackStack("tag")
                    .Commit();
            }
        }

        private void ChangeTeam(object sender, EventArgs eventArgs)
        {
            _application.CurrentTeam = null;
            _application.CurrentClub = null;
            StartActivity(new Intent(this, typeof(ChooseTeamActivity)));
            Finish();
        }

        private void Logout(object sender, EventArgs eventArgs)
        {
            CredentialsProvider.Logout(_application);
            StartActivity(new Intent(this, typeof(LoginActivity)));
            Finish();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    _drawerLayout.OpenDrawer(GravityCompat.Start);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}

