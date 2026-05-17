using System;
using System.Collections.Generic;
using System.Text;
using TestExamination.DTO;
using TestExamination.model;

namespace TestExamination
{
    class QuestionFactory
    {
        public static Question Create(QuestionDTO dto)
        {
            switch (dto.Type)
            {
                case "single":
                    return new SingleChoiceQuestion(dto.Content, dto.Options);
                case "multiple":
                    return new MultipleChoiceQuestion(dto.Content, dto.Options);
                case "fill":
                    return new FillBlankQuestion(dto.Content, dto.CorrectAnswer);
                default:
                    throw new ArgumentException("Invalid question type");
            }
        }
    }
}
