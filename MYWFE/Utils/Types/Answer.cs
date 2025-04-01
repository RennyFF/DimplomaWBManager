namespace MYWFE.Utils.Types
{
    interface IAnswer
    {
        public string Title { get; set; }
        public sbyte Priority { get; set; }
        public bool IsUsing { get; set; }
        public string TargetRating { get; set; }
        public string Text { get; set; }
    }
    public class Answer : IAnswer
    {
        public int Id { get; set; }
        public string Title { get; set; } = "Новый шаблон";
        public sbyte Priority { get; set; } = 9;
        public bool IsUsing { get; set; } = false;
        public string TargetRating { get; set; } = "1-5";
        public string Text { get; set; } = string.Empty;
        public Answer(string title, sbyte priority, bool isUsing, string targetRating, string text)
        {
            Title = title;
            Priority = priority;
            IsUsing = isUsing;
            TargetRating = targetRating;
            Text = text;
        }
        public Answer()
        {
        }
    }
}
