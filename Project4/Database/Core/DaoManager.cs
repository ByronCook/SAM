using Project4.Database.Daos;

namespace Project4.Database.Core
{
    public class DaoManager
    {
        private static DaoManager _instance;

        // All daos
        public UserDao UserDao;
        public TeamDao TeamDao;
        public ClubDao ClubDao;
        public NewsDao NewsDao;
        public TeamUserDao TeamUserDao;
        public EventDao EventDao;
        public AttendantDao AttendantDao;

        public static DaoManager Get()
        {
            return _instance ?? (_instance = new DaoManager());
        }

        private DaoManager()
        {

        }

        public void RegisterDaos()
        {
            UserDao = new UserDao();
            TeamDao = new TeamDao();
            ClubDao = new ClubDao();
            NewsDao = new NewsDao();
            TeamUserDao = new TeamUserDao();
            EventDao = new EventDao();
            AttendantDao = new AttendantDao();
        }
    }
}