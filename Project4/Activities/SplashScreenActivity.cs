using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Project4.Database.Core;

namespace Project4.Activities
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashScreenActivity : AppCompatActivity
    {
        static readonly string TAG = "X:" + typeof(SplashScreenActivity).Name;
        private DB _database;
        private DaoManager _daoManager;

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            Log.Debug(TAG, "SplashActivity.OnCreate");
        }

        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => { SimulateStartup(); });
            startupWork.Start();
        }

        // Simulates background work that happens behind the splash screen
        async void SimulateStartup()
        {
            Log.Debug(TAG, "Performing some startup work that takes a bit of time.");
            await Task.Delay(500); // Simulate a bit of startup work.

            SetupDatabase();

            Log.Debug(TAG, "Startup work is finished - starting MainActivity.");
            StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
        }

        private void SetupDatabase()
        {
            _database = DB.Get();
            _daoManager = DaoManager.Get();
            _daoManager.RegisterDaos();
        }
    }
}