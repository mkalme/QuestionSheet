using Microsoft.Win32;
using QuestionSheet.Creator;
using QuestionSheet.Model;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace QuestionSheet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private QuestionCatalog Catalog { get; set; }
        private int QuestionIndex { get; set; } = -1;

        private Formula? _formula = null;

        public MainWindow()
        {
            InitializeComponent();

            Catalog = new QuestionCatalog();
        }

        private void Image_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                ImageBox.Source = new BitmapImage(new Uri(files[0]));
            }
        }

        private void FormulaButton_Click(object sender, RoutedEventArgs e)
        {
            if (_formula is null) _formula = new Formula();

            FormulaEditor editor = new FormulaEditor(_formula);
            editor.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CreateNewQuestion();
        }

        private void CreateNewQuestion()
        {
            Question question = new Question("", "");
            Catalog.Questions.Add(question);

            SetQuestion(Catalog.Questions.Count - 1);
            Save();
        }
        private void SetQuestion(int index, bool updateCurrentQuestion = true)
        {
            if(updateCurrentQuestion) UpdateCurrentQuestion();
            if (index < 0 || index >= Catalog.Questions.Count) return;

            QuestionIndex = index;
            QuestionIndexLabel.Text = $"{index + 1} / {Catalog.Questions.Count}";

            Question question = Catalog.Questions[QuestionIndex];

            QuestionTextBox.Text = question.Name;
            AnswerTextBox.Text = question.Answer;
            ImageBox.Source = question.Image;
            _formula = question.Formula;

            if (_formula is null || _formula.Parameters.Count == 0 || string.IsNullOrEmpty(_formula.Code))
            {
                FormulaButton.Content = "Create formula";
            }
            else {
                FormulaButton.Content = "Edit formula";
            }
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(PathTextBox.Text)) return;

            string path = PathTextBox.Text;

            try
            {
                Catalog.Save(path);
            }catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateCurrentQuestion() 
        {
            if (QuestionIndex > -1)
            {
                Question oldQuestion = Catalog.Questions[QuestionIndex];
                oldQuestion.Name = QuestionTextBox.Text;
                oldQuestion.Answer = AnswerTextBox.Text;
                oldQuestion.Image = (BitmapSource)ImageBox.Source;
                oldQuestion.Formula = _formula;
            }
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (QuestionIndex < 0) return;
            SetQuestion(QuestionIndex - 1);
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (QuestionIndex < 0) return;
            SetQuestion(QuestionIndex + 1);
        }

        private void NewQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            CreateNewQuestion();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            bool? opened = openFileDialog.ShowDialog();
            if (opened is null || !opened.Value) return;

            UpdateCurrentQuestion();
            Save();

            PathTextBox.Text = openFileDialog.FileName;

            Catalog = QuestionCatalog.FromFile(openFileDialog.FileName);
            SetQuestion(0, false);
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new() 
            {
                FileName = PathTextBox.Text
            };
            bool? opened = saveFileDialog.ShowDialog();

            if (opened is not null && opened.Value) PathTextBox.Text = saveFileDialog.FileName;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateCurrentQuestion();
            Save();
        }
    }
}
