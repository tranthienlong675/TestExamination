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

        [JsonPropertyName("questions")]
        public required List<QuestionDTO> Questions { get; set; }
    }
}