using System;
using System.Collections.Generic;
using System.Text;

namespace TestExamination.model
{
    public class Test
    {
        public string Title { get; set; }

        public int TimeLimitMinutes { get; set; } = 15;

        public bool ShuffleQuestions { get; set; } = true;

        public bool ShuffleOptions { get; set; } = true;

        public List<Question> Questions { get; set; }
    }
}
