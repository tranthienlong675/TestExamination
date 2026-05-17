using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestExamination
{
    public abstract class Question
    {
        public string Content {  get; set; }

        public Question(string question)
        {
            Content = question;
        }

        public abstract void Display();

        public void GetUserAnswer()
        {
            Console.Write("Your answer: ");
            string? answer = Console.ReadLine();
            if (answer != null)
            {
                CheckAnswer(answer);
            }
            else
            {
                Console.WriteLine("Invalid input. Please try again.");
                GetUserAnswer();
            }
        }
        protected abstract bool CheckAnswer(string answer);
    }
}