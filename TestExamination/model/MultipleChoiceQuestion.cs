using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestExamination.model
{
    public class MultipleChoiceQuestion : Question
    {
        public List<Option> Options { get; set; }

        public MultipleChoiceQuestion(
            string content,
            List<Option> options
        ) : base(content)
        {
            Options = options;
        }

        public override void Display()
        {
            Console.WriteLine(Content);

            for (int i = 0; i < Options.Count; i++)
            {
                char label = (char)('A' + i);

                Console.WriteLine(
                    $"{label}. {Options[i].Content}"
                );
            }
        }

        public override bool CheckAnswer(string answer)
        {
            int index = ParseAnswer(answer);

            return Options[index].IsCorrect;
        }
    }
}