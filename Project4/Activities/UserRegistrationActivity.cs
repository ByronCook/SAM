using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using DevOne.Security.Cryptography.BCrypt;
using Project4.Database.Core;
using Project4.Database.Daos;
using Project4.Database.Entities;
using Object = Java.Lang.Object;

namespace Project4.Activities
{
    [Activity(Label = "UserRegistrationActivity")]
    public class UserRegistrationActivity : AppCompatActivity
    {
        private DB _database;
        private DaoManager _daoManager;

        private TextView registerNewAccountTextView;
        private EditText usernameEditText;
        private EditText firstnameEditText;
        private EditText lastnameEditText;
        private EditText passwordEditText;
        private EditText emailEditText;
        private Button createAccountButton;

        private RelativeLayout overlay;

        private RegisterTask task;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.user_registration_screen);

            registerNewAccountTextView = FindViewById<TextView>(Resource.Id.urs_register_new_account_text);
            usernameEditText = FindViewById<EditText>(Resource.Id.urs_username_input);
            firstnameEditText = FindViewById<EditText>(Resource.Id.urs_firstname_input);
            lastnameEditText = FindViewById<EditText>(Resource.Id.urs_lastname_input);
            passwordEditText = FindViewById<EditText>(Resource.Id.urs_password_input);
            emailEditText = FindViewById<EditText>(Resource.Id.urs_email_input);
            overlay = FindViewById<RelativeLayout>(Resource.Id.urs_overlay);

            createAccountButton = FindViewById<Button>(Resource.Id.urs_create_account_button);
            createAccountButton.Click += delegate
            {
                CreateAccountButtonClicked();
            };

            _database = DB.Get();
            _daoManager = DaoManager.Get();
        }

        private void CreateAccountButtonClicked()
        {
            task = new RegisterTask();
            task.Execute(this);
        }

        public bool CheckForValidNaming(string username, string firstname, string lastname, string password, string email)
        {
            if (username == string.Empty)
            {
                System.Diagnostics.Debug.WriteLine("username or password wasn't entered.");
                return false;
            }

            if (firstname == string.Empty)
            {
                return false;
            }

            if (lastname == string.Empty)
            {
                return false;
            }

            if (_daoManager.UserDao.DoesUserExist(username))
            {
                registerNewAccountTextView.Text = registerNewAccountTextView.Text + " ERROR: Username already used";
                System.Diagnostics.Debug.WriteLine("user already exists.");
                return false;
            }

            if (password == string.Empty)
            {
                System.Diagnostics.Debug.WriteLine("username or password wasn't entered.");
                return false;
            }

            return true;
        }

        private void ToggleLayout()
        {
            RunOnUiThread(() =>
            {
                overlay.Visibility = overlay.Visibility == ViewStates.Invisible ? ViewStates.Visible : ViewStates.Invisible;
            });
        }

        private class RegisterTask : AsyncTask
        {
            protected override void OnPreExecute()
            {
                base.OnPreExecute();
            }

            protected override Object DoInBackground(params Object[] @params)
            {
                var activity = @params[0] as UserRegistrationActivity;
                activity.ToggleLayout();

                string usernameValue = activity.usernameEditText.Text;
                string firstnameValue = activity.firstnameEditText.Text;
                string lastnameValue = activity.lastnameEditText.Text;
                string passwordValue = activity.passwordEditText.Text;
                string emailValue = activity.emailEditText.Text;

                if (activity.CheckForValidNaming(usernameValue, firstnameValue, lastnameValue, passwordValue, emailValue))
                {
                    //account creation success condition
                    UserDao userDao = activity._daoManager.UserDao;

                    string userSalt = BCryptHelper.GenerateSalt();
                    string userHashedPassword = BCryptHelper.HashPassword(passwordValue, userSalt);

                    var user = new User
                    {
                        Username = usernameValue,
                        Firstname = firstnameValue,
                        Lastname = lastnameValue,
                        PasswordSalt = userSalt,
                        PasswordHash = userHashedPassword,
                        Email = emailValue
                    };

                    userDao.Save(user);

                    System.Diagnostics.Debug.WriteLine("account creation succesful.");
                    activity.Finish();
                }
                else
                {
                    // account creation failed condition
                    System.Diagnostics.Debug.WriteLine("account creation failed.");
                    activity.ToggleLayout();
                }

                return true;
            }
        }
    }
}