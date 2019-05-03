using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Test> ExistingTests { get; set; }

        public List<string> ExistingTestsNames { get; set; }

        public string SelectedTestName { get { return cboxExistingTests.SelectedValue.ToString(); } }

        public string TestName { get { return tboxName.Text; } }

        public string TestDescr { get { return tboxDescription.Text; } }

        public Test CurrentTest { get; set; }

        public int LastSelected { get; set; }
        public int SelectedQuestionIndex { get { return questionsList.SelectedIndex; } }


        public MainWindow()
        {
            InitializeComponent();
            CurrentTest = new Test("", "");
            LastSelected = -1;
            LoadExistingTests();
        }


        #region Events functions
        private void btnAddQuestion_Click(object sender, RoutedEventArgs e)
        {
            AddQuestion();
        }

        private void btnEditQuestion_Click(object sender, RoutedEventArgs e)
        {
            EditQuestion();
        }

        private void btnDeleteQuestion_Click(object sender, RoutedEventArgs e)
        {
            DeleteQuestion();
        }

        private void btnSaveTest_Click(object sender, RoutedEventArgs e)
        {
            SaveTest();
        }

        private void CboxExistingTests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadTest();
        }

        private void QuestionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillQuestionInputs();
        }
        private void BtnDeleteTest_Click(object sender, RoutedEventArgs e)
        {
            DeleteTest();
        }
        #endregion



        #region Custom functions
        private void AddQuestion()
        {
            if (tboxQuestion.Text.Trim().Equals(""))
            {
                MessageBox.Show("Pytanie musi mieć treść", "Nie można dodać pytania", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            List<string> answers = new List<string>();

            answers.Add(tboxAnswerA.Text.Trim());
            answers.Add(tboxAnswerB.Text.Trim());
            answers.Add(tboxAnswerC.Text.Trim());
            answers.Add(tboxAnswerD.Text.Trim());

            if (answers[0].Equals("") || answers[1].Equals("") || answers[2].Equals("") || answers[3].Equals(""))
            {
                MessageBox.Show("Wszystkie odpowiedzi muszą być wypełnione", "Nie można dodać pytania", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CurrentTest.QuestionExists(tboxQuestion.Text))
            {
                MessageBox.Show("Takie pytanie już istnieje", "Nie można dodać pytania", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //change item - backend
            int correctAnswer = GetCorrectAnswer();
            CurrentTest.Questions.Add(new Question(tboxQuestion.Text, answers, correctAnswer));

            //change item - frontend
            questionsList.ItemsSource = null;
            questionsList.ItemsSource = CurrentTest.QuestionsNamesList();
        }


        private void EditQuestion()
        {
            if (SelectedQuestionIndex < 0)
            {
                MessageBox.Show("Nie wybrano żadnego pytania", "Nie można wykonać akcji", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            //change item - backend
            int correctAnswer = GetCorrectAnswer();
            CurrentTest.EditQuestion(SelectedQuestionIndex, new Question(tboxQuestion.Text, tboxAnswerA.Text, tboxAnswerB.Text, tboxAnswerC.Text, tboxAnswerD.Text, correctAnswer));

            //change item - frontend
            questionsList.ItemsSource = null;
            questionsList.ItemsSource = CurrentTest.QuestionsNamesList();
        }


        private int GetCorrectAnswer()
        {
            if (correctA.IsChecked == true) return 0;
            else if (correctB.IsChecked == true) return 1;
            else if (correctC.IsChecked == true) return 2;
            else return 3;
        }


        private void DeleteQuestion()
        {
            if (LastSelected > -1)
            {
                if (MessageBox.Show("Czy na pewno chcesz usunąć wybrane pytanie?", "Ostrzeżenie", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No) return;
      
                //remove item - backend
                CurrentTest.Questions.RemoveAt(LastSelected);

                //remove item - frontend
                questionsList.ItemsSource = null;
                questionsList.ItemsSource = CurrentTest.QuestionsNamesList();
                ClearQuestionInputs();

                LastSelected--;
            }
        }


        private void SaveTest()
        {
            MessageBoxResult check = MessageBoxResult.Yes;

            if (ExistingTestsNames.Contains(TestName))
            {
                check = MessageBox.Show($"Test o nazwie '{TestName}' już istnieje. Chcesz go nadpisać?", "Uwaga", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            }

            if (check == MessageBoxResult.Yes)
            {
                if (TestName.Trim().Equals(""))
                {
                    MessageBox.Show("Nie podano nazwy testu", "Nie można zapisać testu", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    CurrentTest.Name = TestName;
                    CurrentTest.Description = TestDescr;

                    string jsonContent = JsonConvert.SerializeObject(CurrentTest);

                    if (!Directory.Exists("tests"))
                    {
                        Directory.CreateDirectory("tests");
                    }
                    File.WriteAllText($@"tests\{TestName}.json", jsonContent);

                    MessageBox.Show("Zapisano pomyślnie", "Sukces", MessageBoxButton.OK, MessageBoxImage.Hand);
                    LoadExistingTests();
                }
            }
        }


        private void FillQuestionInputs()
        {
            if (SelectedQuestionIndex > -1)
            {
                Question quest = CurrentTest.Questions[SelectedQuestionIndex];

                tboxQuestion.Text = quest.QuestionContent;
                tboxAnswerA.Text = quest.Answers[0].ToString();
                tboxAnswerB.Text = quest.Answers[1].ToString();
                tboxAnswerC.Text = quest.Answers[2].ToString();
                tboxAnswerD.Text = quest.Answers[3].ToString();

                int correctAnswer = quest.CorrectAnswer;

                //check correct answer
                if (correctAnswer == 0) correctA.IsChecked = true;
                else correctA.IsChecked = false;

                if (correctAnswer == 1) correctB.IsChecked = true;
                else correctB.IsChecked = false;

                if (correctAnswer == 2) correctC.IsChecked = true;
                else correctC.IsChecked = false;

                if (correctAnswer == 3) correctD.IsChecked = true;
                else correctD.IsChecked = false;
            }
            
            LastSelected = SelectedQuestionIndex;
        }


        private void LoadTest()
        {
            CurrentTest = GetTest(SelectedTestName);

            tboxName.Text = CurrentTest.Name;
            tboxDescription.Text = CurrentTest.Description;
            ClearQuestionInputs();

            questionsList.ItemsSource = null;
            questionsList.ItemsSource = CurrentTest.QuestionsNamesList();
        }


        private Test GetTest(string path)
        {
            string fileContent = File.ReadAllText($"tests\\{path}.json");
            Test test = JsonConvert.DeserializeObject<Test>(fileContent);

            return test;
        }

        
        private void LoadExistingTests()
        {
            ExistingTests = new List<Test>();
            ExistingTestsNames = new List<string>();

            foreach (string path in Directory.GetFiles("tests"))
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                Test test = GetTest(fileName);

                ExistingTests.Add(test);
                ExistingTestsNames.Add(test.Name);
            }

            cboxExistingTests.ItemsSource = null;
            cboxExistingTests.ItemsSource = ExistingTestsNames;
        }


        private void DeleteTest()
        {
            if (CurrentTest.Name.Equals(""))
            {
                MessageBox.Show("Nie wybrano żadnego testu", "Nie można wykonać akcji", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            MessageBoxResult result = MessageBox.Show($"Czy na pewno chcesz usunąć test '{CurrentTest.Name}'?", "Ostrzeżenie", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.No) return;
                
            File.Delete($@"tests\{TestName}.json");
            ClearAllInputs();
            LoadExistingTests();
        }


        private void ClearAllInputs()
        {
            tboxName.Text = "";
            tboxDescription.Text = "";
            ClearQuestionInputs();
        }

        private void ClearQuestionInputs()
        {
            tboxQuestion.Text = "";
            tboxAnswerA.Text = "";
            tboxAnswerB.Text = "";
            tboxAnswerC.Text = "";
            tboxAnswerD.Text = "";
            correctA.IsChecked = true;
        }
        #endregion

        
    }
}
