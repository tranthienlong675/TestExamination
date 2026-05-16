using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestExamination
{
    public class FillBlankTest : Test
    {
        private string CorrectAnswer;
        public  FillBlankTest(string question, string correctAnswer) : base(question)
        {
            CorrectAnswer = correctAnswer;
        }

        public override void Display()
        {
            Console.WriteLine(Question);
        }

        protected override bool CheckAnswer(string answer)
        {
            return answer == CorrectAnswer;
        }
    }
}