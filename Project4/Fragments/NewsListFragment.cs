using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Project4.Activities;
using Project4.Database.Core;
using Fragment = Android.Support.V4.App.Fragment;

namespace Project4.Fragments
{
    public class NewsListFragment : Fragment
    {
        private MainActivity _activity;
        private MyApplication _myApplication;

        private List<string> newsList = new List<string>();
        private List<string> newsListBody = new List<string>();
        private ListView _newsListView;
        private Button _newMessageButton;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.news_list, container, false);
            _activity = (MainActivity) Activity;
            _myApplication = _activity.ApplicationContext as MyApplication;

            SetupViews(view);
            Load();

            return view;
        }

        public void SetupViews(View view)
        {
            _newsListView = view.FindViewById<ListView>(Resource.Id.newsl_ListView);

            _newMessageButton = view.FindViewById<Button>(Resource.Id.newsl_new_message_button);
            _newMessageButton.Click += delegate
            {
                var fragment = new NewsMessageCreateFragment();
                fragment.OnAttach(_activity.ApplicationContext);

                _activity.SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.content_frame, fragment).AddToBackStack("tag")
                    .Commit();
            };

            _newsListView.ItemClick += NewsListView_ItemClicked;
        }

        public void Load()
        {
            Task.Factory.StartNew(() =>
            {
                newsList.Clear();
                var newsmessages = DaoManager.Get().NewsDao.FindAll();

                foreach (var news in newsmessages)
                {
                    newsList.Add(news.Title);
                    newsListBody.Add(news.Body);
                };

            }).ContinueWith(task => {

                ArrayAdapter<string> arrayadapter = new ArrayAdapter<string>(_activity, Android.Resource.Layout.SimpleListItem1, newsList);
                _newsListView.Adapter = arrayadapter;
                arrayadapter.NotifyDataSetChanged();

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void NewsListView_ItemClicked(object sender, AdapterView.ItemClickEventArgs e)
        {
            var selectTitle = newsList[e.Position];
            var selectBody = newsListBody[e.Position];
            ShowNewsMessage(selectTitle, selectBody);
        }

        public void ShowNewsMessage(string newsTitle, string newsBody)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(_activity);
            builder.SetTitle(newsTitle);
            builder.SetMessage(newsBody);

            builder.SetPositiveButton("Close", (System.EventHandler<Android.Content.DialogClickEventArgs>)null);

            AlertDialog dialog = builder.Create();
            dialog.Show();
        }
    }
}