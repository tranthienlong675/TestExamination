using System;

namespace TestExamination.model
{
    public abstract class Question
    {
        // CHỈ ĐỂ DUY NHẤT 1 DÒNG NÀY, XÓA DÒNG CONTENT CÒN LẠI NẾU CÓ
        public string Content { get; set; } = string.Empty;

        public abstract string Description { get; }
        public bool IsCorrect { get; set; } 

        public Question(string content)
        {
            Content = content; // Hết lỗi đỏ gạch chân!
        }

        public abstract void Display();
        public abstract bool CheckAnswer(string answer);

        public void GetUserAnswer()
        {
            while (true)
            {
                Console.Write("Your answer: ");
                string? answer = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(answer))
                {
                    Console.WriteLine("Blank input. Please try again.");
                    continue;
                }

                try
                {
                    IsCorrect = CheckAnswer(answer);
                    break; 
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid input. Please try again.");
                }
            }
        }

        protected int ParseAnswer(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer)) return -1;
            char c = answer.Trim().ToUpper()[0];
            return c - 'A';
        }
    }
}