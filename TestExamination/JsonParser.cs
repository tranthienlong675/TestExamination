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
            test.TimeLimitMinutes = testDto.TimeLimitMinutes > 0 ? testDto.TimeLimitMinutes : 15;
            test.ShuffleQuestions = testDto.ShuffleQuestions;
            test.ShuffleOptions = testDto.ShuffleOptions;

            try
            {
                test.Questions = testDto.Questions
                .Select(q => QuestionFactory.Create(q))
                .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test is not valid: " + ex.Message);
                Console.WriteLine("If you are student. Please inform your teacher.");

                return null;
            }

            return test;
        }
    }
}
