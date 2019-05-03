using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCreator
{
    class Question
    {
        #region Properties

        public string QuestionContent { get; set; }
        public List<Answer> Answers { get; set; }
        public int CorrectAnswer { get; set; }

        #endregion


        #region Constructors

        public Question() { }
        public Question(string content, string answerA, string answerB, string answerC, string answerD, int correctAnswer)
        {
            Answers = new List<Answer>();

            QuestionContent = content;

            Answers.Add(new Answer(answerA));
            Answers.Add(new Answer(answerB));
            Answers.Add(new Answer(answerC));
            Answers.Add(new Answer(answerD));

            CorrectAnswer = correctAnswer;
        }

        #endregion


        #region Functions

        #endregion
    }
}
