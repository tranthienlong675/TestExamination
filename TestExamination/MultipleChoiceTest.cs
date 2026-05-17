using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestExamination
{
    public class MultipleChoiceTest : Question
    {
        private List<string> Options;
        private HashSet<char> CorrectAnswers;
        public MultipleChoiceTest(string content, List<string> options, HashSet<char> correctAnswers) : base(content)
        {
            Options = options;
            CorrectAnswers = correctAnswers;
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
            HashSet<char> userAnswers = answer
                .ToUpper()
                .Split(',')
                .Select(x => x.Trim()[0])
                .ToHashSet();
            return userAnswers.SetEquals(CorrectAnswers);
        }
    }
}