using Microsoft.EntityFrameworkCore;
using MYWFE.Utils.Types;
using static MYWFE.MVVM.Model.Database.AppDbContext;

namespace MYWFE.MVVM.Model.Database.Requests
{
    public interface IUserRequests
    {
        Task<UserSet> GetUser();
        Task AddUser(User user);
        Task RemoveUser(int Id);
        Task UpdateUser(User user);
    }

    public class UserRequests : IUserRequests
    {
        #region Values
        private readonly AppDbContext _context;
        #endregion
        #region Methods
        public async Task AddUser(User user)
        {
            var NewUser = new UserSet
            {
                UserName = user.UserName,
                TradeMark = user.TradeMark,
                FeedbackToken = user.FeedbackToken,
                StatiscticsToken = user.StatiscticsToken
            };
            await _context.User.AddAsync(NewUser);
            await _context.SaveChangesAsync();
        }

        public async Task<UserSet> GetUser()
        {
            return await _context.User.FirstOrDefaultAsync();
        }

        public async Task RemoveUser(int Id)
        {
            var RemoveUser = await _context.User.FirstAsync(i => i.Id == Id);
            if (RemoveUser != null)
            {
                await Task.Run(() => _context.User.Remove(RemoveUser));
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateUser(User user)
        {
            var OldUser = await _context.User.FirstAsync(i => i.Id == user.Id);

            if (OldUser != null)
            {
                OldUser.FeedbackToken = user.FeedbackToken;
                OldUser.StatiscticsToken = user.StatiscticsToken;

                _context.User.Update(OldUser);
                await _context.SaveChangesAsync();
            }
        }
        #endregion
        public UserRequests(AppDbContext context)
        {
            _context = context;
        }
    }
}
