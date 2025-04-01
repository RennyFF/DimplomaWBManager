using Microsoft.EntityFrameworkCore;
using MYWFE.Utils.Types;
using static MYWFE.MVVM.Model.Database.AppDbContext;

namespace MYWFE.MVVM.Model.Database.Requests
{
    public interface IAnswerRequests
    {
        Task<AnswerSet> GetAnswer(int Id);
        Task<List<AnswerSet>> GetListAnswers();
        Task AddAnswer(Answer answer);
        Task RemoveAnswer(int Id);
        Task UpdateAnswer(Answer answer);
    }

    public class AnswerRequests : IAnswerRequests
    {
        #region Values
        private readonly AppDbContext _context;
        #endregion
        #region Methods
        public async Task AddAnswer(Answer answer)
        {
            var NewAnswer = new AnswerSet
            {
               IsUsing = answer.IsUsing,
               Priority = answer.Priority,
               TargetRating = answer.TargetRating,
               Text = answer.Text,
               Title = answer.Title
            };
            await _context.Answers.AddAsync(NewAnswer);
            await _context.SaveChangesAsync();
        }

        public async Task<AnswerSet> GetAnswer(int Id)
        {
            var Result = _context.Answers.FirstOrDefaultAsync(i => i.Id == Id);
            return await (Result ?? null);
        }

        public async Task<List<AnswerSet>> GetListAnswers()
        {
           return await _context.Answers.ToListAsync();
        }

        public async Task RemoveAnswer(int Id)
        {
            var RemoveAnswer = await _context.Answers.FirstOrDefaultAsync(i => i.Id == Id);
            if (RemoveAnswer != null)
            {
                await Task.Run(() => _context.Answers.Remove(RemoveAnswer));
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAnswer(Answer answer)
        {
            var OldAnswer = await _context.Answers.FirstOrDefaultAsync(i => i.Id == answer.Id);

            if (OldAnswer != null)
            {
                OldAnswer.Priority = answer.Priority;
                OldAnswer.TargetRating = answer.TargetRating;
                OldAnswer.Text = answer.Text;
                OldAnswer.Title = answer.Title; 
                OldAnswer.IsUsing = answer.IsUsing;

                _context.Answers.Update(OldAnswer);
                await _context.SaveChangesAsync();
            }
        }
        #endregion
        public AnswerRequests(AppDbContext context)
        {
            _context = context;
        }
    }
}
