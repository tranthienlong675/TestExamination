using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestExamination
{
    public abstract class Test
    {
        public string Content {  get; set; }

        public Test(string content)
        {
            Content = content;
        }

        public abstract void Display();

        public abstract bool CheckAnswer(string answer);
    }
}