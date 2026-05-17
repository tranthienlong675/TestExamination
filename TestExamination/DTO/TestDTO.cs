using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TestExamination.DTO
{
    class TestDTO
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("questions")]
        public List<QuestionDTO> Questions { get; set; }
    }
}
