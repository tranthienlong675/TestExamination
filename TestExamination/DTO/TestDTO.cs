using System;
using System.Collections.Generic;
using System.Text;

namespace TestExamination.DTO
{
    class TestDTO
    {
        public string? Title { get; set; }
        public List<QuestionDTO>? Questions { get; set; }
    }
}
