using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestExamination
{
    public class MultipleChoiceTest : Test
    {
        public MultipleChoiceTest(string content) : base(content)
        {

        }

        public override void Display()
        {

        }

        public override bool CheckAnswer(string answer)
        {
            return false;
        }
    }
}