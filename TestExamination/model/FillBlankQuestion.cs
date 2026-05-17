using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestExamination.model
{
    public class FillBlankQuestion : Question
    {
        public override string Description
            => "Fill in the Blank Question. Answer the question by typing the correct answer. \nThe answer is not case-sensitive, and leading/trailing whitespace will be ignored.";
        public string CorrectAnswer { get; set; }

        public FillBlankQuestion(
            string content, 
            string correctAnswer
        ) : base(content)
        {
            if (string.IsNullOrWhiteSpace(correctAnswer))
            {
                throw new Exception("There is a fill blank question that contain no answer. Invalid question.");
            }
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