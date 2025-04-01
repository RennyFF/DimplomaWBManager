using MYWFE.Core;
using MYWFE.MVVM.Model.Database.Requests;
using MYWFE.Utils.Types;

namespace MYWFE.Utils.Services
{
    interface IUserService
    {
        User? User { get; set; }
        void RemoveUser();
        void AddUser(User UnsettedUser);
        void UpdateUser(User UnsettedUser);
    }

    internal class UserService : ObservableObject, IUserService
    {
        #region PassedValues
        private readonly IUserRequests UserContext;
        #endregion
        #region Values
        private User? _user;

        public User? User
        {
            get { return _user; }
            set { _user = value; onPropertyChanged(); }
        }
        #endregion
        #region Methods
        private async void SetUser()
        {
            var UserSet = await UserContext.GetUser();
            User = UserSet == null ? null : 
                User = new User()
                {
                    Id = UserSet.Id,
                    UserName = UserSet.UserName,
                    TradeMark = UserSet.TradeMark,
                    StatiscticsToken = UserSet.StatiscticsToken,
                    FeedbackToken = UserSet.FeedbackToken
                };
        }

        public async void RemoveUser()
        {
            await UserContext.RemoveUser((int)User.Id);
            SetUser();
        }

        public async void AddUser(User UnsettedUser)
        {
            await UserContext.AddUser(UnsettedUser);
            SetUser();
        }
        public async void UpdateUser(User UnsettedUser)
        {
            await UserContext.UpdateUser(UnsettedUser);
            SetUser();
        }
        #endregion
        public UserService(IUserRequests userRequests)
        {
            UserContext = userRequests;
            SetUser();
        }
    }
}
