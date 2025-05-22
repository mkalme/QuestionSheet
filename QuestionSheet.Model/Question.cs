using System.IO;
using System.Windows.Media.Imaging;

namespace QuestionSheet.Model;

public class Question
{
    public string Name { get; set; }
    public string Answer { get; set; }
    public BitmapSource? Image { get; set; }
    public Formula? Formula { get; set; }

    public static Question Empty { get; } = new(string.Empty, string.Empty);

    public Question(string name, string answer, BitmapSource? iamge = null) 
    {
        Name = name;
        Answer = answer;
        Image = iamge;
    }

    public void Write(BinaryWriter writer) 
    {
        writer.Write(Name);
        writer.Write(Answer);
        writer.Write(Image is not null);

        if (Image is not null) 
        {
            using (MemoryStream bmp = new()) 
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(Image));
                enc.Save(bmp);

                bmp.Flush();

                byte[] imageByteArray = bmp.ToArray();
                writer.Write(imageByteArray.Length);
                writer.Write(imageByteArray);
            }
        }

        writer.Write(Formula is not null);
        if (Formula is not null) {
            Formula.Write(writer);
        }
    }
    public static Question FromBinaryReader(BinaryReader reader) 
    {
        string name = reader.ReadString();
        string answer = reader.ReadString();
        BitmapSource? image = null;
        Formula? formula = null;

        if (reader.ReadBoolean()) 
        {
            int length = reader.ReadInt32();
            byte[] bytes = reader.ReadBytes(length);

            using (MemoryStream stream = new(bytes)) 
            {
                image = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
        }

        if (reader.ReadBoolean()) {
            formula = Formula.FromBinaryReader(reader);
        }

        return new Question(name, answer, image) { 
            Formula = formula
        };
    }
}
