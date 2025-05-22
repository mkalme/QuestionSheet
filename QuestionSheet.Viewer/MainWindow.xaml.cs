using CodeExecutor;
using Microsoft.Win32;
using QuestionSheet.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace QuestionSheet.Viewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private QuestionCatalog? Catalog { get; set; }
        private Formula? _formula = null;

        private TextBox[] _parameterTextBoxes;

        public MainWindow()
        {
            InitializeComponent();

            Style rowStyle = new Style(typeof(DataGridRow));
            rowStyle.Setters.Add(new EventSetter(MouseDoubleClickEvent, new MouseButtonEventHandler(Row_DoubleClick)));
            DataGridView.RowStyle = rowStyle;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            bool? opened = dialog.ShowDialog();

            if (opened is null || !opened.Value || !File.Exists(dialog.FileName)) Environment.Exit(0);

            Catalog = QuestionCatalog.FromFile(dialog.FileName);
            SetRows(Catalog.Questions);
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Catalog is null) return;

            if (string.IsNullOrEmpty(NameTextBox.Text)) 
            {
                SetRows(Catalog.Questions);
                return;
            }

            IList<Question> search = Catalog.Search(NameTextBox.Text);

            SetRows(search);
            if (search.Count == 1) SetQuestion(search[0]);
            else SetQuestion(Question.Empty);
        }

        private void SetRows(IEnumerable<Question> questions) 
        {
            DataGridView.Items.Clear();

            foreach (Question question in questions) 
            {
                Item item = new(question);
                DataGridView.Items.Add(item);
            }
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow? row = sender as DataGridRow;
            if (row is null) return;

            SetQuestion(((Item)row.Item).Question);
        }

        private void SetQuestion(Question question) 
        {
            QuestionLabel.Text = question.Name;
            AnswerLabel.Text = question.Answer;
            ImageBox.Source = question.Image;
            _formula = question.Formula;

            if (_formula is null)
            {
                FormulaGrid.Visibility = Visibility.Collapsed;
            }
            else {
                FormulaGrid.Visibility = Visibility.Visible;
                DisplayNameLabel.Text = _formula.OutputDisplay;

                ParameterGrid.Children.Clear();
                ParameterGrid.ColumnDefinitions.Clear();

                _parameterTextBoxes = new TextBox[_formula.Parameters.Count];
                for (int i = 0; i < _formula.Parameters.Count; i++) {
                    _parameterTextBoxes[i] = new TextBox() {
                        Width = 100,
                        Margin = new Thickness(i == 0 ? 0 : 10, 0, 0, 0),
                        Foreground = Brushes.Yellow,
                        Background = new SolidColorBrush(Color.FromRgb(53, 53, 53)),
                        HorizontalAlignment = HorizontalAlignment.Left,
                    };

                    ParameterGrid.ColumnDefinitions.Add(new ColumnDefinition() {
                        Width = new GridLength(0, GridUnitType.Auto)
                    });

                    Grid.SetRow(_parameterTextBoxes[i], 1);
                    Grid.SetColumn(_parameterTextBoxes[i], i);
                    ParameterGrid.Children.Add(_parameterTextBoxes[i]);

                    Label label = new Label() { 
                        Content = _formula.Parameters[i].DisplayName,
                        Foreground = Brushes.Yellow,
                        Margin = new Thickness(i == 0 ? 0 : 10, 0, 0, 0),
                        Padding = new Thickness(0)
                    };

                    Grid.SetRow(label, 0);
                    Grid.SetColumn(label, i);
                    ParameterGrid.Children.Add(label);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ImageBox.Source is null) return;

            try
            {
                Clipboard.SetImage((BitmapSource) ImageBox.Source);
            }
            catch { }
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double[] parameters = new double[_formula.Parameters.Count];
                for (int i = 0; i < parameters.Length; i++) {
                    parameters[i] = double.Parse(_parameterTextBoxes[i].Text.Replace(",", "."), CultureInfo.InvariantCulture);
                }

                ResultTextBox.Text = _formula.Execute(parameters).ToString();

                GC.Collect();
            }
            catch (Exception exception) {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
