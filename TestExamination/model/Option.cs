using System;
using System.Collections.Generic;
using System.Text;

namespace TestExamination.model
{
    public class Option
    {
        public string Content { get; set; }
        public bool IsCorrect { get; set; }
        public Option(string content, bool isCorrect)
        {
            Content = content;
            IsCorrect = isCorrect;
        }
    }
}
