using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using Project4.Dialogs;
using Project4.Provider;

namespace Project4.Activities
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : BaseActivity
    {
        private EditText _usernameEditText;
        private EditText _passwordEditText;
        private Button _loginButton;
        private Button _registerButton;
        private CheckBox _saveLoginCheckBox;
        
        private ISharedPreferences _preferences;
        private ISharedPreferencesEditor _prefsEditor;

        private MyApplication _application;

        private bool _overlayEnabled;
        private bool _saveLogin;
        private string _username;
        private string _password;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Calls the OnCreate and SetContentView inside the BaseActivity
            OnActivityCreate(savedInstanceState, Resource.Layout.user_login);

            //Set the title on the toolbar
            SetActivityTitle(Resource.String.header_login);

            Toolbar.SetLogo(Resource.Drawable.Icon);

            //Find the Overlay and ActivityLayout inside the BaseActivity
            UseOverlay();

            SetupViews();
            SetupPreferences();

            //Safe cast ApplicationContext as MyApplication, _application is needed to set the CurrentUser
            _application = ApplicationContext as MyApplication;
        }

        /// <summary>
        /// Find all needed views inside the Activity and add the needed event handling
        /// </summary>
        private void SetupViews()
        {
            _usernameEditText = FindViewById<EditText>(Resource.Id.ulo_username_input);
            _passwordEditText = FindViewById<EditText>(Resource.Id.ulo_password_input);

            _loginButton = FindViewById<Button>(Resource.Id.ulo_login_button);
            _registerButton = FindViewById<Button>(Resource.Id.ulo_register_button);

            _usernameEditText.EditorAction += HandleEditorAction;
            _passwordEditText.EditorAction += HandleEditorAction;
            _loginButton.Click += LoginButtonClick;
            _registerButton.Click += RegisterButtonClick;

            _saveLoginCheckBox = FindViewById<CheckBox>(Resource.Id.ulo_save_login_checkbox);
        }

        /// <summary>
        /// Setup the preferences using the PreferenceManager build inside Android
        /// </summary>
        private void SetupPreferences()
        {
            _preferences = PreferenceManager.GetDefaultSharedPreferences(this);
            _prefsEditor = _preferences.Edit();

            _saveLogin = _preferences.GetBoolean("saveLogin", false);

            if (_saveLogin)
            {
                _usernameEditText.Text = _preferences.GetString("username", "");
                _passwordEditText.Text = _preferences.GetString("password", "");
                _saveLoginCheckBox.Checked = true;
            }
        }

        /// <summary>
        /// Gets called when the login button is clicked.
        /// </summary>
        private void LoginButtonClick(object sender, EventArgs e)
        {
            HideKeyboard();
            ProcessLogin();
        }

        /// <summary>
        /// Gets called when the register button is clicked.
        /// </summary>
        private void RegisterButtonClick(object sender, EventArgs e)
        {
            HideKeyboard();
            StartActivity(new Intent(Application.Context, typeof(UserRegistrationActivity)));
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

        /// <summary>
        /// Process the given username and password.
        /// </summary>
        public void ProcessLogin()
        {
            var result = 0;
            Task.Factory.StartNew(() =>
            {
                ToggleLayout();
                
                _username = _usernameEditText.Text;
                _password = _passwordEditText.Text;

                result = CredentialsProvider.Login(_application, _username, _password);

                ToggleLayout();
            }).ContinueWith(task => 
            {
                ProcessResult(result);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Process the result. Starts the next activity or shows an error dialog.
        /// </summary>
        private void ProcessResult(int result)
        {
            switch (result)
            {
                case CredentialsProvider.NoError:
                    if (_saveLoginCheckBox.Checked)
                    {
                        _prefsEditor.PutBoolean("saveLogin", true);
                        _prefsEditor.PutString("username", _username);
                        _prefsEditor.PutString("password", _password);
                        _prefsEditor.Apply();
                    }
                    else
                    {
                        _prefsEditor.Clear();
                        _prefsEditor.Apply();
                    }
                    

                    StartActivity(new Intent(ApplicationContext, typeof(ChooseTeamActivity)));
                    Finish();
                    break;
                case CredentialsProvider.NoCredentials:
                {
                    var dialog = new ErrorDialog(this, Resource.String.dlg_no_credentials_title, Resource.String.dlg_no_credentials);
                    dialog.Display();
                    break;
                }
                case CredentialsProvider.NoUsername:
                {
                    var dialog = new ErrorDialog(this, Resource.String.dlg_no_username_title, Resource.String.dlg_no_username);
                    dialog.Display();
                    break;
                }
                case CredentialsProvider.NoPassword:
                {
                    var dialog = new ErrorDialog(this, Resource.String.dlg_no_password_title, Resource.String.dlg_no_password);
                    dialog.Display();
                    break;
                }
                case CredentialsProvider.WrongCredentials:
                {
                    var dialog = new ErrorDialog(this, Resource.String.dlg_bad_credentials_title, Resource.String.dlg_bad_credentials);
                    dialog.Display();
                    break;
                }
                case CredentialsProvider.DatabaseConnectionError:
                {
                    var dialog = new ErrorDialog(this, Resource.String.dlg_database_error_title, Resource.String.dlg_database_error);
                    dialog.Display();
                    break;
                }
            }
        }
        
    }
}