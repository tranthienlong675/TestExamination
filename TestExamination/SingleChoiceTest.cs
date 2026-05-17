using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestExamination
{
    public class SingleChoiceTest : Question
    {
        private List<string> Options;
        private char CorrectAnswer;
        public SingleChoiceTest(string content, List<string> options, char correctAnswer) : base(content)
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
            return answer.Trim().ToUpper()[0] == CorrectAnswer;
        }
    }
}