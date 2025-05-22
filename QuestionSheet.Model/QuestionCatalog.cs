using System.Collections.Generic;
using System.IO;

namespace QuestionSheet.Model
{
    public class QuestionCatalog
    {
        public IList<Question> Questions { get; set; }

        public QuestionCatalog() 
        {
            Questions = new List<Question>();
        }

        public IList<Question> Search(string input) 
        {
            IList<Question> output = new List<Question>();

            input = input.Trim().ToLower();

            foreach (Question question in Questions) 
            {
                if (question.Name.ToLower().Trim().Contains(input)) 
                {
                    output.Add(question);
                }
            }

            return output;
        }

        public void Save(string file) 
        {
            using (FileStream fileStream = File.OpenWrite(file))
            using (BinaryWriter writer = new(fileStream))
            {
                writer.Write(Questions.Count);

                foreach (Question question in Questions) 
                {
                    question.Write(writer);
                }
            }
        }
        public static QuestionCatalog FromFile(string file) 
        {
            using (FileStream fileStream = File.OpenRead(file))
            using (BinaryReader reader = new(fileStream))
            {
                QuestionCatalog output = new();
                int length = reader.ReadInt32();

                for (int i = 0; i < length; i++) 
                {
                    output.Questions.Add(Question.FromBinaryReader(reader));
                }

                return output;
            }
        }
    }
}
