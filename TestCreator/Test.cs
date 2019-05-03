using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCreator
{
    class Test
    {
        #region Properties

        public string Name { get; set; }
        public string Description { get; set; }
        public List<Question> Questions { get; set; }

        #endregion


        #region Constructors

        // default constructor for json deserializer
        public Test() { }

        public Test(string name, string description)
        {
            Name = name;
            Description = description;
            Questions = new List<Question>();
        }

        public Test(string name, string description, List<Question> questions)
        {
            Name = name;
            Description = description;
            Questions = questions;
        }

        #endregion


        #region Functions

        public bool QuestionExists(string question)
        {
            int occurrence = 0;
            foreach (var item in Questions)
            {
                if (item.QuestionContent.Equals(question))
                    occurrence++;
            }

            if (occurrence > 0) return true;
            else return false;
        }


        public Question FindQestion(string question)
        {
            foreach (var item in Questions)
            {
                if (item.QuestionContent.Equals(question))
                    return item;
            }
            return null;
        }


        public void EditQuestion(int index, Question newQuestion)
        {
            Questions.RemoveAt(index);
            Questions.Insert(index, newQuestion);
        }

        #endregion
    }
}
