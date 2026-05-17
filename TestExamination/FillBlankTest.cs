using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestExamination
{
    public class FillBlankTest : Question
    {
        private string CorrectAnswer;
        public  FillBlankTest(string content, string correctAnswer) : base(content)
        {
            CorrectAnswer = correctAnswer;
        }

        public override void Display()
        {
            Console.WriteLine(Content);
        }

        protected override bool CheckAnswer(string answer)
        {
            return answer == CorrectAnswer;
        }
    }
}