using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using TestExamination.DTO;
using TestExamination.model;

namespace TestExamination
{
    class JsonParser
    {
        public static Test Parse(string path)
        { 
            string json = File.ReadAllText(path);

            TestDTO testDto = JsonSerializer.Deserialize<TestDTO>(json);

            Test test = new Test();

            test.Title = testDto.Title;

            test.Questions = testDto.Questions
                .Select(q => QuestionFactory.Create(q))
                .ToList();
            return test;
        }
    }
}
