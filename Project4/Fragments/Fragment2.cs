using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Util;
using Project4.Database.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Support.V4.App;
using Acr.UserDialogs;

namespace Project4.Fragments
{
	public class Fragment2 : Fragment
    {
        private DaoManager              _daoManager;
        private MyApplication           _myApplication;
		private Database.Entities.User  _currentUser;
		private View                    _currentView;
		Dictionary<string, string>      _configData;

		private TextView                _userNameTextView,
            							_userNameSmallTextView,
            		                    _emailTextView;

        private int _userId;


        public override void OnCreate(Bundle savedInstanceState)
        {
			base.OnCreate(savedInstanceState);
            _daoManager = DaoManager.Get();
            
            var bundle = Arguments;
            if (bundle != null)
            {
                _userId = bundle.GetInt("user_id", 0);
            }

            var layout = Activity.FindViewById<RelativeLayout>(Resource.Id.content_frame_wrapper);
            layout.SetPadding(0, 0, 0, 0);

            getCurrentUser();
        }

		public Fragment2(Android.App.Activity activity)
		{
		    _myApplication = activity.ApplicationContext as MyApplication;
		    UserDialogs.Init(activity);
        }
        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            _currentView = inflater.Inflate(Resource.Layout.ProfileLayout, null);
			return _currentView;
        }

		public void getCurrentUser()
        {
			List<Database.Entities.User>  result = null;
            Task.Factory.StartNew(() =>
			{
				result =  DaoManager.Get().UserDao.Find(_userId);
				Log.Debug("user: ", result.ToString());
				return result;
			}).ContinueWith(task =>
            {
                ProcessResult(result);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ProcessResult( List<Database.Entities.User> result)
        {
			if( result.Count == 1 )
			{
				_currentUser = result[0];
				_configData = new Dictionary<string, string>()
				{
					{
					    "username",
					    _currentUser.Username
					},
					{
						"email",
						_currentUser.Email
					}
				};
				DisplayUserData(_currentUser);
			}
        }

		private void DisplayUserData(Database.Entities.User currentUser)
		{
			_userNameTextView = _currentView.FindViewById<TextView>(Resource.Id.textView_user_profile_name);
			_userNameTextView.SetText(currentUser.Username, null);

			_userNameSmallTextView = _currentView.FindViewById<TextView>(Resource.Id.textview_user_profile_email);
			_userNameSmallTextView.SetText("Email: " + currentUser.Email, null);

			_emailTextView = _currentView.FindViewById<TextView>(Resource.Id.textview_user_profile_name_small);
			_emailTextView.SetText("Name: " + currentUser.Username, null);

		    if (currentUser.Id == _myApplication.CurrentUser.Id)
		    {
		        _userNameSmallTextView.Click += OnClickEmail;
                _emailTextView.Click += OnClickUsername;
            }
		}

		private void OnClickUsername(object obj, object e){         
			UserDialogs.Instance.Prompt(GeneratePromptConfig("username"));
		}
		private void OnClickEmail(object obj, object e){
			UserDialogs.Instance.Prompt(GeneratePromptConfig("email"));
		}


		private PromptConfig GeneratePromptConfig(string key)
        {
            return new PromptConfig()
            {
                Text = _configData[key],
                OnAction = args =>
                {
                    if (args.Ok)
                    {
                        HandleEditRequest(key, args.Value);
                    }
                }
            };
        }

        private void HandleEditRequest(string key, string data)
        {            
			switch(key){
				case "username":
					_currentUser.Username = data;
					break;
				case "email":
					_currentUser.Email = data;
					break;
			};
			_daoManager.UserDao.Save(_currentUser);

        }

        
    }
}