using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestExamination.model
{
    public abstract class Question
    {
        public string Content { get; set; }

        public Question(string content)
        {
            Content = content;
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

        public int ParseAnswer(string answer)
        {
            char c = answer.Trim().ToUpper()[0];

            return c - 'A';
        }
        public abstract bool CheckAnswer(string answer);
    }
}