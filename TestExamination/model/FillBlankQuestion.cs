using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestExamination.model
{
    public class FillBlankQuestion : Question
    {
        public string CorrectAnswer { get; set; }

        public FillBlankQuestion(
            string content,
            string correctAnswer
        ) : base(content)
        {
            CorrectAnswer = correctAnswer;
        }

        public override void Display()
        {
            Console.WriteLine(Content);
        }

        public override bool CheckAnswer(string answer)
        {
            return string.Equals(answer.Trim(), CorrectAnswer, StringComparison.OrdinalIgnoreCase);
        }
    }
}