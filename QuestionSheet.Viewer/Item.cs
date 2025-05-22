using QuestionSheet.Model;

namespace QuestionSheet.Viewer
{
    public class Item
    {
        public Question Question { get; set; }
        public string Name => Question.Name;

        public Item(Question question)
        {
            Question = question;
        }
    }
}
