using System;
using System.Collections.Generic;
using System.Text;

namespace TestExamination.DTO
{
    class QuestionDTO
    {
        public string Type { get; set; }
        public string Content { get; set; }
        public List<string> Options { get; set; }
        public int CorrectIndex { get; set; }
        public List<int> CorrectIndexs { get; set; }
        public string CorrectAnswer { get; set; }
    }
}
