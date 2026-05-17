using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using TestExamination.model;

namespace TestExamination.DTO
{
    class QuestionDTO
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("content")]
        public string Content { get; set; }
        [JsonPropertyName("options")]
        public List<Option> Options { get; set; }
        [JsonPropertyName("correctAnswer")]
        public string CorrectAnswer { get; set; }
    }
}
