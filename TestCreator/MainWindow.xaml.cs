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
        private string TestName { get { return tboxName.Text; } }
        private string TestDescr { get { return tboxDescr.Text; } }
        private Test MyTest;
        private int LastSelected;

        public MainWindow()
        {
            InitializeComponent();
            MyTest = new Test("", "");
        }


        private void LoadTest()
        {
            string fileContent = File.ReadAllText(@"C:\Users\Hubert\Desktop\Test1.json");
            Test test = JsonConvert.DeserializeObject<Test>(fileContent);

            Console.WriteLine($"Nazwa testu: {test.Name}\nOpis: {test.Description}");
        }


        private void FillInputs(object sender, SelectionChangedEventArgs e)
        {
            LastSelected = questionsList.SelectedIndex;

            if (LastSelected > -1)
            {
                Question quest = MyTest.Questions[LastSelected];

                //fill inputs
                tboxQuestion.Text = quest.QuestionContent;
                tboxAnswerA.Text = quest.Answers[0].ToString();
                tboxAnswerB.Text = quest.Answers[1].ToString();
                tboxAnswerC.Text = quest.Answers[2].ToString();
                tboxAnswerD.Text = quest.Answers[3].ToString();
            }
        }

        private void btnAddQuestion_Click(object sender, RoutedEventArgs e)
        {
            List<string> answers = new List<string>();

            answers.Add(tboxAnswerA.Text.Trim());
            answers.Add(tboxAnswerB.Text.Trim());
            answers.Add(tboxAnswerC.Text.Trim());
            answers.Add(tboxAnswerD.Text.Trim());

            if (answers[0].Equals("") && answers[1].Equals("") && answers[2].Equals("") && answers[3].Equals(""))
            {
                MessageBox.Show("Nie podano żadnej odpowiedzi", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MyTest.QuestionExists(tboxQuestion.Text))
            {
                MessageBox.Show("Takie pytanie już istnieje", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MyTest.Questions.Add(new Question(tboxQuestion.Text, tboxAnswerA.Text, tboxAnswerB.Text, tboxAnswerC.Text, tboxAnswerD.Text, 0));
                questionsList.Items.Add(tboxQuestion.Text);
            }
        }


        private void btnEditQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (LastSelected > -1)
            {
                //change item - backend
                MyTest.EditQuestion(LastSelected, new Question(tboxQuestion.Text, tboxAnswerA.Text, tboxAnswerB.Text, tboxAnswerC.Text, tboxAnswerD.Text, 0));

                //change item - frontend
                questionsList.Items.Insert(LastSelected, MyTest.Questions[LastSelected].QuestionContent);
                questionsList.Items.RemoveAt(LastSelected);
                questionsList.Items.Refresh();
            }
        }

        private void btnDeleteQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (LastSelected > -1)
            {
                //remove item - backend
                MyTest.Questions.RemoveAt(LastSelected);

                //remove item - frontend
                questionsList.Items.RemoveAt(LastSelected);
                questionsList.Items.Refresh();

                LastSelected--;
            }
        }


        private void btnSaveTest_Click(object sender, RoutedEventArgs e)
        {
            LoadTest();
            if (TestName.Trim().Equals(""))
            {
                MessageBox.Show("Nie podano nazwy testu", "Nie można zapisać testu", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MyTest.Name = TestName;
                MyTest.Description = TestDescr;

                string jsonContent = JsonConvert.SerializeObject(MyTest);

                if (!Directory.Exists("tests"))
                {
                    Directory.CreateDirectory("tests");
                } 
                File.WriteAllText($@"tests\{TestName}.json", jsonContent);
            }
        }

        private void CboxExistingTests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadTest();
        }
    }
}
