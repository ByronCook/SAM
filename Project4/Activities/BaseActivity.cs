using Android.Content.PM;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Keycode = Android.Views.Keycode;

namespace Project4.Activities
{
    public abstract class BaseActivity : AppCompatActivity
    {
        private DrawerLayout _drawerLayout;
        protected Android.Support.V7.Widget.Toolbar Toolbar;

        protected RelativeLayout Overlay;
        protected RelativeLayout ActivityLayout;

        protected void OnActivityCreate(Bundle savedInstanceState, int layoutResource)
        {
            base.OnCreate(savedInstanceState);
            base.SetContentView(layoutResource);
            //Set screen on portrait mode
            RequestedOrientation = ScreenOrientation.Portrait;
        }

        protected void SetActivityTitle(int titleResource)
        {
            Toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            Toolbar.SetTitle(titleResource);
        }

        protected void EnableActionBarBackButton()
        {
            SetSupportActionBar(Toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
        }

        protected void UseOverlay()
        {
            Overlay = FindViewById<RelativeLayout>(Resource.Id.overlay);
            ActivityLayout = FindViewById<RelativeLayout>(Resource.Id.activity_layout);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        protected void HandleEditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            if (!(sender is EditText editText)) return;

            if (e.ActionId == ImeAction.Search || 
                e.ActionId == ImeAction.Done ||
                e.Event != null && 
                (e.Event.Action == KeyEventActions.Down || 
                 e.Event.KeyCode == Keycode.Enter ||
                 e.Event.KeyCode == Keycode.Unknown))
            {
                if (e.Event != null && !e.Event.IsShiftPressed)
                {
                    HideKeyboard(editText);
                    e.Handled = true;
                    return;
                }
            }
            e.Handled = false;
        }

        protected void HideKeyboard(EditText editText)
        {
            var service = ApplicationContext.GetSystemService(InputMethodService) as InputMethodManager;
            service?.HideSoftInputFromWindow(editText.WindowToken, 0);
        }

        protected void HideKeyboard()
        {
            var service = ApplicationContext.GetSystemService(InputMethodService) as InputMethodManager;
            service?.HideSoftInputFromWindow(CurrentFocus.WindowToken, 0);
        }

        protected void ShowOverlay(bool show)
        {
            RunOnUiThread(() =>
            {
                Overlay.Visibility = show ? ViewStates.Visible : ViewStates.Invisible;
            });
        }

        protected void SetActivityLayoutChildElementsEnabled(bool enabled)
        {
            RunOnUiThread(() =>
            {
                SetChildsEnabled(ActivityLayout, enabled);
            });
        }

        private static void SetChildsEnabled(ViewGroup vg, bool enabled)
        {
            for (var i = 0; i < vg.ChildCount; i++)
            {
                var child = vg.GetChildAt(i);
                child.Enabled = enabled;

                if (child is ViewGroup @group){
                    SetChildsEnabled(@group, enabled);
                }
            }
        }
}
}