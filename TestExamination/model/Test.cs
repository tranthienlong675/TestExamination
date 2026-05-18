using System;
using System.Collections.Generic;
using System.Text;

namespace TestExamination.model
{
    public class Test
    {
        public string Title { get; set; }

        public List<Question> Questions { get; set; }
    }
}
