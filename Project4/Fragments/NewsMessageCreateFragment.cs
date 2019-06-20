using System;
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
    public class NewsMessageCreateFragment : Fragment
    {
        private MainActivity _activity;
        private MyApplication _myApplication;

        private EditText _titleEditText;
        private EditText _bodyEditText;
        private Button _confirmButton;
        private RelativeLayout _overlay;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.news_message_create, container, false);
            _activity = (MainActivity) Activity;
            _myApplication = _activity.ApplicationContext as MyApplication;

            SetupViews(view);

            return view;
        }

        public void SetupViews(View view)
        {
            _titleEditText = view.FindViewById<EditText>(Resource.Id.newsn_title_input);
            _bodyEditText = view.FindViewById<EditText>(Resource.Id.newsn_body_input);
            _confirmButton = view.FindViewById<Button>(Resource.Id.newsn_confirm_button);
            _overlay = view.FindViewById<RelativeLayout>(Resource.Id.urs_overlay);
            _confirmButton.Click += ConfirmButtonClicked;
        }

        private void ConfirmButtonClicked(object sender, EventArgs eventArgs)
        {
            Task.Factory.StartNew(() =>
            {
                ToggleLayout();

                var titleValue = _titleEditText.Text;
                var bodyValue = _bodyEditText.Text;

                if (CheckForValidEntries(titleValue, bodyValue))
                {
                    var newsDao = DaoManager.Get().NewsDao;
                    var news = new News
                    {
                        Title = titleValue,
                        Body = bodyValue,
                        Team_Id = _myApplication.CurrentTeam.Id
                    };

                    newsDao.Save(news);
                }

                ToggleLayout();

            }).ContinueWith(task => {
                _activity.OnBackPressed();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ToggleLayout()
        {
            _activity.RunOnUiThread(() =>
            {
                _overlay.Visibility = _overlay.Visibility == ViewStates.Invisible ? ViewStates.Visible : ViewStates.Invisible;
            });
        }

        protected bool CheckForValidEntries(string title, string body)
        {
            if (title == string.Empty)
            {
                _titleEditText.Hint = "You did not enter any title.";
                return false;
            }

            if (body == string.Empty)
            {
                _bodyEditText.Hint = "You did not enter any message.";
                return false;
            }

            return true;
        }
    }
}