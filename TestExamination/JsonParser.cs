using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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

        public static void SaveTestToJson(Test test, string path)
        {
            try
            {
                // Chuyển đổi Test thành TestDTO
                var questions = new List<QuestionDTO>();
                
                foreach (var question in test.Questions)
                {
                    var questionDTO = new QuestionDTO();
                    questionDTO.Content = question.Content;

                    if (question is FillBlankQuestion fillQuestion)
                    {
                        questionDTO.Type = "fill";
                        questionDTO.CorrectAnswer = fillQuestion.CorrectAnswer;
                    }
                    else if (question is SingleChoiceQuestion singleQuestion)
                    {
                        questionDTO.Type = "single";
                        questionDTO.Options = singleQuestion.Options;
                    }
                    else if (question is MultipleChoiceQuestion multiQuestion)
                    {
                        questionDTO.Type = "multiple";
                        questionDTO.Options = multiQuestion.Options;
                    }

                    questions.Add(questionDTO);
                }

                var testDTO = new TestDTO
                {
                    Title = test.Title,
                    TimeLimitMinutes = test.TimeLimitMinutes,
                    ShuffleQuestions = test.ShuffleQuestions,
                    ShuffleOptions = test.ShuffleOptions,
                    Questions = questions
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                string json = JsonSerializer.Serialize(testDTO, options);
                File.WriteAllText(path, json);

                Console.WriteLine($"[SUCCESS] Đã lưu câu hỏi vào file JSON: {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi lưu file JSON: {ex.Message}");
            }
        }
    }
}
