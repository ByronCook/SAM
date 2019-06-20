using System;
using Android.App;
using Android.Runtime;
using Project4.Database.Entities;

namespace Project4
{
    #if DEBUG
        [Application(Debuggable = true)]
    #else
        [Application(Debuggable = false)]
    #endif
    public class MyApplication : Application
    {
        public User CurrentUser;
        public Team CurrentTeam;
        public Club CurrentClub;

        public MyApplication(IntPtr handle, JniHandleOwnership transfer)
            : base(handle, transfer)
        {
            
        }
    }
}