using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TestExamination.model
{
    public class Option
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }
        [JsonPropertyName("isCorrect")]
        public bool IsCorrect { get; set; }
        
        public Option() { }
        
        public Option(string content, bool isCorrect)
        {
            Content = content;
            IsCorrect = isCorrect;
        }
    }
}
