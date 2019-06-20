using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using Project4.Activities;
using Project4.Database.Core;
using Project4.Database.Entities;
using Fragment = Android.Support.V4.App.Fragment;

namespace Project4.Fragments
{
    public class MemberListFragment : Fragment
    {
        private MainActivity _activity;
        private MyApplication _myApplication;

        private readonly Dictionary<string, User> _membersDictionary = new Dictionary<string, User>();
        private readonly List<string> _members = new List<string>();
        
        private ListView _membersListView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.member_list, container, false);
            
            _activity = (MainActivity)Activity;
            _myApplication = _activity.ApplicationContext as MyApplication;

            _membersListView = view.FindViewById<ListView>(Resource.Id.memberListView);
            _membersListView.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs args)
            {
                OnItemClick(args);
            };

            getData();

            return view;
        }

        private void OnItemClick(AdapterView.ItemClickEventArgs e)
        {
            var item = _membersListView.GetItemAtPosition(e.Position);

            var fragment = new Fragment2(_activity);
            var bundle = new Bundle();
            bundle.PutInt("user_id", _membersDictionary[item.ToString()].Id);
            fragment.Arguments = bundle;
            fragment.OnAttach(_activity.ApplicationContext);

            _activity.SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_frame, fragment).AddToBackStack("tag")
                .Commit();
        }

        public void getData()
        {
            Task.Factory.StartNew(() =>
            {
                var userTeams = DaoManager.Get().TeamUserDao.FindByTeamId(_myApplication.CurrentTeam.Id);

                var userIds = userTeams.Select(ut => ut.UserId.ToString()).ToList();
                var users = DaoManager.Get().UserDao.FindByList("User_Id", userIds);

                _members.Clear();
                _membersDictionary.Clear();
                
                foreach (var user in users)
                {
                    _members.Add(user.Username);
                    _membersDictionary.Add(user.Username, user);
                }
            }).ContinueWith(task => {
                var adapter = new ArrayAdapter<string>(_activity, Android.Resource.Layout.SimpleListItem1, _members);
                _membersListView.Adapter = adapter;
                adapter.NotifyDataSetChanged();

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}