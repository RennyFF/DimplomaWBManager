using Microsoft.EntityFrameworkCore;
using System.IO;

namespace MYWFE.MVVM.Model.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserSet> User { get; set; }
        public DbSet<AnswerSet> Answers { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "userdata.db")}");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public class UserSet
        {
            public int Id { get; set; }
            public string UserName { get; set; }
            public string TradeMark { get; set; }
            public string StatiscticsToken { get; set; }
            public string FeedbackToken { get; set; }
        }
        public class AnswerSet
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public sbyte Priority { get; set; }
            public bool IsUsing { get; set; }
            public string TargetRating { get; set; }
            public string Text { get; set; }
        }
    }
}
