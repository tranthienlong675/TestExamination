using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestExamination.model
{
    public class SingleChoiceQuestion : Question
    {
        public override string Description 
            => "Single Choice Question. Answer the question by typing A, B, C... correspond to the correct answer. \nYou must only choice one ";
        public List<Option> Options { get; set; }

        public SingleChoiceQuestion(
            string content,
            List<Option> options
        ) : base(content)
        {
            if (options.Count < 2)
            {
                throw new Exception(
                    "There are a single choice question that contain less than 2 answers. Invalid question."
                );
            }
            int correctCount = options.Count(
                option => option.IsCorrect
            );

            if (correctCount != 1)
            {
                throw new Exception(
                    "There are a single choice question that does not contain exactly one correct answer. Invalid question."
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
            if (answer.Length > 1) throw new Exception("More than 1 character");
            int index = ParseAnswer(answer);

            return Options[index].IsCorrect;
        }
    }
}