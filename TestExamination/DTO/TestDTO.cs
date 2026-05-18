using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TestExamination.DTO
{
    class TestDTO
    {
        [JsonPropertyName("title")]
        public required string Title { get; set; }

        [JsonPropertyName("timeLimitMinutes")]
        public int TimeLimitMinutes { get; set; } = 15;

        [JsonPropertyName("shuffleQuestions")]
        public bool ShuffleQuestions { get; set; } = true;

        [JsonPropertyName("shuffleOptions")]
        public bool ShuffleOptions { get; set; } = true;

        [JsonPropertyName("questions")]
        public required List<QuestionDTO> Questions { get; set; }
    }
}
