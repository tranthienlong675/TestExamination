using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestExamination
{
    public class SingleChoiceTest : Test
    {
        private List<string> Options;
        private string CorrectAnswer;
        public SingleChoiceTest(string question, List<string> options, string correctAnswer) : base(question)
        {
            Options = options;
            CorrectAnswer = correctAnswer;
        }

        public override void Display() 
        {
            for (int i = 0; i < Options.Count; i++)
            {
                Console.WriteLine($"{(char)('A' + i)}. {Options[i]}");
            }
        }

        protected override bool CheckAnswer(string answer)
        {
            return answer.ToUpper() == CorrectAnswer;
        }
    }
}