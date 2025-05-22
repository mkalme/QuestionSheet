using QuestionSheet.Model;
using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace QuestionSheet.Creator
{
    /// <summary>
    /// Interaction logic for FormulaEditor.xaml
    /// </summary>
    public partial class FormulaEditor : Window
    {
        public Formula Formula { get; set; }

        public FormulaEditor(Formula formula)
        {
            InitializeComponent();
        
            Formula = formula;

            SetInputs();
        }

        private void SetInputs() {
            StringBuilder builder = new StringBuilder();
            foreach (Parameter parameter in Formula.Parameters) {
                builder.AppendLine($"\"{parameter.DisplayName}\" {parameter.VariableName}");
            }

            VariableRichTextBox.Document.Blocks.Clear();
            VariableRichTextBox.Document.Blocks.Add(new Paragraph(new Run(builder.ToString())));
            DisplayNameTextBox.Text = Formula.OutputDisplay;
            FormulaTextBox.Text = Formula.Code;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Parse(Formula);
            Close();
        }

        private void Parse(Formula output) {
            TextRange textRange = new TextRange(
                VariableRichTextBox.Document.ContentStart,
                VariableRichTextBox.Document.ContentEnd
            );
            string[] lines = textRange.Text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            output.Parameters.Clear();

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;

                try
                {
                    output.Parameters.Add(ParseParameter(line));
                }
                catch { }
            }

            output.OutputDisplay = DisplayNameTextBox.Text;
            output.Code = FormulaTextBox.Text;
        }

        private static Parameter ParseParameter(string line)
        { 
            int startIndex = line.IndexOf('\"') + 1;
            int endIndex = line.LastIndexOf('\"') - 1;

            string displayName = line.Substring(startIndex, endIndex - startIndex + 1);
            string variableName = line.Substring(endIndex + 2);

            return new Parameter(displayName, variableName);
        }

        private void ValidateButton_Click(object sender, RoutedEventArgs e)
        {
            Formula f = new Formula();
            Parse(f);

            try
            {
                string[] p = ParameterTextBox.Text.Split(";");
                double[] parameters = new double[p.Length];

                for (int i = 0; i < p.Length; i++) {
                    parameters[i] = double.Parse(p[i].Replace(",", "."), CultureInfo.InvariantCulture);
                }

                double result = f.Execute(parameters);
                MessageBox.Show($"Result: {result}", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(Exception exception) {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
