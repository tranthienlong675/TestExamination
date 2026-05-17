using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestExamination.model
{
    public class MultipleChoiceQuestion : Question
    {
        public override string Description
            => "Multiple Choice Question. Answer the question by typing A, B, C... correspond to the correct answer. \nYou can choice more than one answer, separate them by comma, for example: A,C; or leave it blank if no answer is correct.";
        public List<Option> Options { get; set; }

        public MultipleChoiceQuestion(
            string content,
            List<Option> options
        ) : base(content)
        {
            if (options.Count < 2)
            {
                throw new Exception(
                    "There are a multiple choice question that contain less than 2 answers. Invalid question."
                );
            }
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
            string[] choices = answer
                .ToUpper()
                .Replace(" ", "")
                .Split(',');

            foreach (var choice in choices)
            {
                if (choice.Length > 1) throw new Exception("More than 1 character");
            }

            var userIndexes = choices.Select(x => ParseAnswer(x)).ToHashSet();

            if (userIndexes.Any(i => i < 0 || i >= Options.Count))
            {
                throw new ArgumentException(
                    "Answer contains invalid option."
                );
            }

            var correctIndexes = Options
                .Select((option, index) => new { option, index })
                .Where(x => x.option.IsCorrect)
                .Select(x => x.index)
                .ToHashSet();

            return userIndexes.SetEquals(correctIndexes);
        }
    }
}