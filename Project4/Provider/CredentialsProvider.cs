using DevOne.Security.Cryptography.BCrypt;
using Project4.Database.Core;

namespace Project4.Provider
{
	public static class CredentialsProvider
	{
	    public const int NoError = 0;
	    public const int NoCredentials = 1;
	    public const int NoUsername = 2;
	    public const int NoPassword = 3;
	    public const int WrongCredentials = 4;
	    public const int DatabaseConnectionError = 5;

		public static int Login(MyApplication application, string username, string password)
		{
		    var databaseConnectionFound = DB.Get().TestConnection();
		    if (!databaseConnectionFound)
		    {
		        return DatabaseConnectionError;
		    }

            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
		    {
		        return NoCredentials;
		    }

			if (string.IsNullOrEmpty(password))
			{
			    return NoPassword;
			}
            
			if (string.IsNullOrEmpty(username))
			{
			    return NoUsername;
			}

			var userDao = DaoManager.Get().UserDao;

		    var users = userDao.Find("username", username);

		    if (users.Count > 0)
		    {
		        var user = users[0];

		        var hashedInput = BCryptHelper.HashPassword(password, user.PasswordSalt);

		        if (hashedInput.Equals(user.PasswordHash))
		        {
		            application.CurrentUser = user;
		            return NoError;
		        }
		    }

		    return WrongCredentials;
        }

	    public static void Logout(MyApplication application)
	    {
	        application.CurrentUser = null;
	        application.CurrentTeam = null;
	        application.CurrentClub = null;
	    }
    }
}
