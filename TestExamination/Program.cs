using TestExamination.model;

namespace TestExamination
{
    class App
    {
        public static void Validate()
        {
            Test test = JsonParser.Parse("tests/test.json");
            if (test != null)
            {
                Init(test);
            }
            else
            {
                Console.WriteLine("Test closed");
            }
            
        }
        public static void Init(Test test)
        {
            Console.WriteLine("Welcome to the Test Examination system.");
            Console.WriteLine("We will answer the following questions to get scores.");
            Console.WriteLine($"\nTest Title: {test.Title}");

            HashSet<string> questionSet = new HashSet<string>();

            foreach (var s in test.Questions)
            {
                questionSet.Add(s.Description);
            }

            Console.WriteLine("In this test, we have the following types of questions:\n");
            foreach (var s in questionSet) Console.WriteLine($"- {s}");

            Console.WriteLine("\nLet's start the test by click Enter key.\n");
            var key = Console.ReadKey();
            while (key.Key != ConsoleKey.Enter)
            {
                key = Console.ReadKey();
            }
            Run(test);
        }
        public static void Run(Test test)
        {
            Console.WriteLine("Starting the test...\n");
            double score = 10 / (1.0 * test.Questions.Count);
            double max = 10;
            foreach (var item in test.Questions)
            {
                item.Display();
                item.GetUserAnswer();
                if (!item.isCorrect)
                {
                    max -= score;
                }
            }

            Console.WriteLine($"\nYour score is: {max}");
        }
        public static void Main(string[] args)
        {
            Validate();
        } 
    }
}