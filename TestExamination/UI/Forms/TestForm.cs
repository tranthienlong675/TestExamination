using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TestExamination.model;

namespace TestExamination.UI.Forms
{
    public partial class TestForm : Form
    {
        private readonly Test test;
        private readonly string studentId;
        private readonly string studentName;
        private readonly Dictionary<int, HashSet<int>> selectedOptions = new Dictionary<int, HashSet<int>>();
        private readonly Dictionary<int, string> fillAnswers = new Dictionary<int, string>();
        private readonly List<int> questionOrder = new List<int>();
        private readonly Dictionary<int, List<int>> optionOrderByQuestionIndex = new Dictionary<int, List<int>>();
        private readonly Color pageBack = Color.FromArgb(247, 247, 252);
        private readonly Color ink = Color.FromArgb(33, 33, 44);
        private readonly Color muted = Color.FromArgb(104, 105, 125);
        private readonly Color quizPurple = Color.FromArgb(123, 76, 255);
        private readonly Color quizCoral = Color.FromArgb(255, 96, 112);
        private readonly Color quizTeal = Color.FromArgb(0, 175, 170);
        private readonly Color optionBack = Color.FromArgb(255, 255, 255);
        private readonly Color optionLine = Color.FromArgb(226, 226, 236);

        private int currentIndex;
        private Panel root = null!;
        private Label lblTitle = null!;
        private Label lblProgress = null!;
        private Label lblQuestion = null!;
        private Label lblType = null!;
        private Label lblTimer = null!;
        private ProgressBar progressBar = null!;
        private FlowLayoutPanel optionPanel = null!;
        private Button btnPrevious = null!;
        private Button btnNext = null!;
        private Button btnSubmit = null!;
        private System.Windows.Forms.Timer quizTimer = null!;
        private int remainingSeconds;
        private bool isSubmitted;

        public TestForm(Test test, string studentId, string studentName)
        {
            this.test = test;
            this.studentId = studentId;
            this.studentName = studentName;
            BuildShuffleMaps();
            InitializeComponent();
            BuildLayout();
            ShowQuestion();
            StartTimer();
        }

        public TestForm(Test test) : this(test, "SV001", "Guest Student")
        {
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            quizTimer?.Stop();
            quizTimer?.Dispose();
            base.OnFormClosed(e);
        }

        private void BuildShuffleMaps()
        {
            var random = new Random(Guid.NewGuid().GetHashCode());

            questionOrder.Clear();
            questionOrder.AddRange(Enumerable.Range(0, test.Questions.Count));
            if (test.ShuffleQuestions)
            {
                Shuffle(questionOrder, random);
            }

            for (int questionIndex = 0; questionIndex < test.Questions.Count; questionIndex++)
            {
                int optionCount = GetOptionCount(test.Questions[questionIndex]);
                if (optionCount == 0)
                {
                    continue;
                }

                var optionOrder = Enumerable.Range(0, optionCount).ToList();
                if (test.ShuffleOptions)
                {
                    Shuffle(optionOrder, random);
                }

                optionOrderByQuestionIndex[questionIndex] = optionOrder;
            }
        }

        private void Shuffle<T>(IList<T> items, Random random)
        {
            for (int i = items.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (items[i], items[j]) = (items[j], items[i]);
            }
        }

        private int GetOptionCount(Question question)
        {
            if (question is SingleChoiceQuestion singleChoice)
            {
                return singleChoice.Options.Count;
            }

            if (question is MultipleChoiceQuestion multipleChoice)
            {
                return multipleChoice.Options.Count;
            }

            return 0;
        }

        private int GetOriginalQuestionIndex(int displayIndex)
        {
            return questionOrder[displayIndex];
        }

        private Question GetQuestionAtDisplayIndex(int displayIndex)
        {
            return test.Questions[GetOriginalQuestionIndex(displayIndex)];
        }

        private List<int> GetOptionOrderForDisplayIndex(int displayIndex)
        {
            int originalQuestionIndex = GetOriginalQuestionIndex(displayIndex);
            return optionOrderByQuestionIndex.TryGetValue(originalQuestionIndex, out var order)
                ? order
                : new List<int>();
        }

        private void BuildLayout()
        {
            BackColor = pageBack;
            Controls.Clear();

            root = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = pageBack,
                Padding = new Padding(26)
            };
            Controls.Add(root);

            var top = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = pageBack
            };
            root.Controls.Add(top);

            lblTitle = CreateLabel(test.Title, 24, FontStyle.Bold, ink);
            lblTitle.Location = new Point(4, 6);
            lblTitle.AutoSize = true;
            top.Controls.Add(lblTitle);

            var student = CreateLabel($"{studentName} - {studentId}", 10, FontStyle.Bold, quizTeal);
            student.Location = new Point(8, 50);
            student.AutoSize = true;
            top.Controls.Add(student);

            lblProgress = CreateLabel("", 11, FontStyle.Bold, muted);
            lblProgress.Location = new Point(8, 82);
            lblProgress.AutoSize = true;
            top.Controls.Add(lblProgress);

            lblTimer = CreateLabel("", 18, FontStyle.Bold, quizCoral);
            lblTimer.Location = new Point(890, 36);
            lblTimer.Size = new Size(120, 32);
            lblTimer.TextAlign = ContentAlignment.MiddleRight;
            top.Controls.Add(lblTimer);

            progressBar = new ProgressBar
            {
                Location = new Point(230, 82),
                Size = new Size(650, 18),
                Minimum = 0,
                Maximum = questionOrder.Count,
                Style = ProgressBarStyle.Continuous
            };
            top.Controls.Add(progressBar);

            var questionCard = new Panel
            {
                Dock = DockStyle.Top,
                Height = 178,
                BackColor = Color.White,
                Padding = new Padding(24)
            };
            root.Controls.Add(questionCard);
            questionCard.BringToFront();

            lblType = CreateLabel("", 10, FontStyle.Bold, quizPurple);
            lblType.Location = new Point(24, 22);
            lblType.AutoSize = true;
            questionCard.Controls.Add(lblType);

            lblQuestion = CreateLabel("", 19, FontStyle.Bold, ink);
            lblQuestion.Location = new Point(24, 56);
            lblQuestion.Size = new Size(850, 96);
            questionCard.Controls.Add(lblQuestion);

            optionPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = pageBack,
                Padding = new Padding(0, 22, 0, 22),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true
            };
            root.Controls.Add(optionPanel);
            optionPanel.BringToFront();

            var bottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 82,
                BackColor = pageBack
            };
            root.Controls.Add(bottom);

            btnPrevious = CreateButton("Previous", Color.FromArgb(45, 45, 64), Color.White);
            btnPrevious.ForeColor = Color.White;
            btnPrevious.Location = new Point(0, 18);
            btnPrevious.Size = new Size(130, 42);
            btnPrevious.Click += (_, _) => MovePrevious();
            bottom.Controls.Add(btnPrevious);

            btnSubmit = CreateButton("Submit", quizCoral, Color.White);
            btnSubmit.Location = new Point(678, 18);
            btnSubmit.Size = new Size(130, 42);
            btnSubmit.Click += (_, _) => SubmitQuiz();
            bottom.Controls.Add(btnSubmit);

            btnNext = CreateButton("Next", quizPurple, Color.White);
            btnNext.Location = new Point(824, 18);
            btnNext.Size = new Size(130, 42);
            btnNext.Click += (_, _) => MoveNext();
            bottom.Controls.Add(btnNext);
        }

        private void StartTimer()
        {
            remainingSeconds = Math.Max(1, test.TimeLimitMinutes) * 60;
            UpdateTimerLabel();

            quizTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            quizTimer.Tick += (_, _) =>
            {
                remainingSeconds--;
                UpdateTimerLabel();

                if (remainingSeconds <= 0)
                {
                    quizTimer.Stop();
                    SubmitQuiz(true);
                }
            };
            quizTimer.Start();
        }

        private void UpdateTimerLabel()
        {
            int minutes = Math.Max(0, remainingSeconds) / 60;
            int seconds = Math.Max(0, remainingSeconds) % 60;
            lblTimer.Text = $"{minutes:00}:{seconds:00}";
            lblTimer.ForeColor = remainingSeconds <= 60 ? quizCoral : quizTeal;
        }

        private void ShowQuestion()
        {
            var question = GetQuestionAtDisplayIndex(currentIndex);
            lblQuestion.Text = question.Content;
            lblType.Text = question.Description.Split('\n')[0];
            lblProgress.Text = $"Question {currentIndex + 1} of {questionOrder.Count}";
            progressBar.Value = Math.Min(currentIndex + 1, questionOrder.Count);

            btnPrevious.Enabled = currentIndex > 0;
            btnNext.Text = currentIndex == questionOrder.Count - 1 ? "Review" : "Next";

            optionPanel.Controls.Clear();

            if (question is SingleChoiceQuestion singleChoice)
            {
                AddSingleChoiceOptions(singleChoice);
            }
            else if (question is MultipleChoiceQuestion multipleChoice)
            {
                AddMultipleChoiceOptions(multipleChoice);
            }
            else if (question is FillBlankQuestion)
            {
                AddFillBlankAnswer();
            }
        }

        private void AddSingleChoiceOptions(SingleChoiceQuestion question)
        {
            selectedOptions.TryGetValue(currentIndex, out var saved);

            var optionOrder = GetOptionOrderForDisplayIndex(currentIndex);
            for (int i = 0; i < optionOrder.Count; i++)
            {
                int originalOptionIndex = optionOrder[i];
                var option = new RadioButton
                {
                    Text = $"{(char)('A' + i)}. {question.Options[originalOptionIndex].Content}",
                    Tag = originalOptionIndex,
                    AutoSize = false,
                    Size = new Size(890, 52),
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    ForeColor = ink,
                    BackColor = optionBack,
                    Padding = new Padding(18, 0, 0, 0),
                    Margin = new Padding(0, 0, 0, 12),
                    Checked = saved != null && saved.Contains(originalOptionIndex)
                };
                optionPanel.Controls.Add(option);
            }
        }

        private void AddMultipleChoiceOptions(MultipleChoiceQuestion question)
        {
            selectedOptions.TryGetValue(currentIndex, out var saved);

            var optionOrder = GetOptionOrderForDisplayIndex(currentIndex);
            for (int i = 0; i < optionOrder.Count; i++)
            {
                int originalOptionIndex = optionOrder[i];
                var option = new CheckBox
                {
                    Text = $"{(char)('A' + i)}. {question.Options[originalOptionIndex].Content}",
                    Tag = originalOptionIndex,
                    AutoSize = false,
                    Size = new Size(890, 52),
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    ForeColor = ink,
                    BackColor = optionBack,
                    Padding = new Padding(18, 0, 0, 0),
                    Margin = new Padding(0, 0, 0, 12),
                    Checked = saved != null && saved.Contains(originalOptionIndex)
                };
                optionPanel.Controls.Add(option);
            }
        }

        private void AddFillBlankAnswer()
        {
            var input = new TextBox
            {
                Name = "txtAnswer",
                Text = fillAnswers.TryGetValue(currentIndex, out var answer) ? answer : string.Empty,
                Size = new Size(890, 44),
                Font = new Font("Segoe UI", 14F),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 6, 0, 0)
            };
            optionPanel.Controls.Add(input);
        }

        private void SaveCurrentAnswer()
        {
            if (optionPanel == null || test.Questions == null || currentIndex < 0 || currentIndex >= questionOrder.Count)
            {
                return;
            }

            var question = GetQuestionAtDisplayIndex(currentIndex);

            if (question is SingleChoiceQuestion)
            {
                var selected = optionPanel.Controls.OfType<RadioButton>().FirstOrDefault(x => x.Checked);
                if (selected?.Tag is int index)
                {
                    selectedOptions[currentIndex] = new HashSet<int> { index };
                }
            }
            else if (question is MultipleChoiceQuestion)
            {
                selectedOptions[currentIndex] = optionPanel.Controls
                    .OfType<CheckBox>()
                    .Where(x => x.Checked)
                    .Select(x => x.Tag)
                    .OfType<int>()
                    .ToHashSet();
            }
            else if (question is FillBlankQuestion)
            {
                var input = optionPanel.Controls.OfType<TextBox>().FirstOrDefault();
                fillAnswers[currentIndex] = input?.Text ?? string.Empty;
            }
        }

        private void MovePrevious()
        {
            SaveCurrentAnswer();
            if (currentIndex > 0)
            {
                currentIndex--;
                ShowQuestion();
            }
        }

        private void MoveNext()
        {
            SaveCurrentAnswer();
            if (currentIndex < questionOrder.Count - 1)
            {
                currentIndex++;
                ShowQuestion();
                return;
            }

            MessageBox.Show("You are on the last question. Submit when ready.", "Quiz Arena", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SubmitQuiz(bool timeExpired = false)
        {
            if (isSubmitted)
            {
                return;
            }

            SaveCurrentAnswer();

            if (!timeExpired)
            {
                var confirm = MessageBox.Show("Submit your answers now?", "Quiz Arena", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm != DialogResult.Yes)
                {
                    return;
                }
            }

            isSubmitted = true;
            quizTimer?.Stop();

            double perQuestionScore = 10.0 / questionOrder.Count;
            double score = 0;
            var submission = new TestExamination.StudentSubmission
            {
                StudentId = studentId,
                StudentName = studentName,
                IsGraded = true
            };

            for (int i = 0; i < questionOrder.Count; i++)
            {
                var question = GetQuestionAtDisplayIndex(i);
                bool isCorrect = IsAnswerCorrect(i, question);
                if (isCorrect)
                {
                    score += perQuestionScore;
                }

                submission.Answers.Add(new TestExamination.StudentAnswer
                {
                    QuestionContent = question.Content,
                    UserAnswer = BuildAnswerSummary(i, question, isCorrect)
                });
            }

            submission.Score = Math.Round(score, 2);
            ExamManager.Submissions.Add(submission);

            string prefix = timeExpired ? "Time is up. Your test was submitted automatically.\n\n" : string.Empty;
            MessageBox.Show($"{prefix}Your score: {submission.Score:0.##}/10", "Quiz Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        private bool IsAnswerCorrect(int index, Question question)
        {
            if (question is SingleChoiceQuestion singleChoice)
            {
                if (!selectedOptions.TryGetValue(index, out var selected) || selected.Count != 1)
                {
                    return false;
                }

                int selectedIndex = selected.First();
                return selectedIndex >= 0 && selectedIndex < singleChoice.Options.Count && singleChoice.Options[selectedIndex].IsCorrect;
            }

            if (question is MultipleChoiceQuestion multipleChoice)
            {
                var selected = selectedOptions.TryGetValue(index, out var choices) ? choices : new HashSet<int>();
                var correct = multipleChoice.Options
                    .Select((option, optionIndex) => new { option, optionIndex })
                    .Where(x => x.option.IsCorrect)
                    .Select(x => x.optionIndex)
                    .ToHashSet();

                return selected.SetEquals(correct);
            }

            if (question is FillBlankQuestion fillBlank)
            {
                string answer = fillAnswers.TryGetValue(index, out var value) ? value : string.Empty;
                return string.Equals(answer.Trim(), fillBlank.CorrectAnswer, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private string BuildAnswerSummary(int index, Question question, bool isCorrect)
        {
            string status = isCorrect ? "Correct" : "Incorrect";

            if (question is SingleChoiceQuestion singleChoice)
            {
                var selected = selectedOptions.TryGetValue(index, out var choices) ? choices.FirstOrDefault(-1) : -1;
                string answer = selected >= 0 && selected < singleChoice.Options.Count
                    ? $"{GetDisplayedOptionLabel(index, selected)}. {singleChoice.Options[selected].Content}"
                    : "No answer";
                return $"{answer} ({status})";
            }

            if (question is MultipleChoiceQuestion multipleChoice)
            {
                var selected = selectedOptions.TryGetValue(index, out var choices)
                    ? choices.OrderBy(x => GetDisplayedOptionPosition(index, x)).ToList()
                    : new List<int>();
                string answer = selected.Count == 0
                    ? "No answer"
                    : string.Join(", ", selected.Select(x => x >= 0 && x < multipleChoice.Options.Count ? $"{GetDisplayedOptionLabel(index, x)}. {multipleChoice.Options[x].Content}" : "?"));
                return $"{answer} ({status})";
            }

            if (question is FillBlankQuestion)
            {
                string answer = fillAnswers.TryGetValue(index, out var value) && !string.IsNullOrWhiteSpace(value)
                    ? value.Trim()
                    : "No answer";
                return $"{answer} ({status})";
            }

            return status;
        }

        private int GetDisplayedOptionPosition(int displayQuestionIndex, int originalOptionIndex)
        {
            var optionOrder = GetOptionOrderForDisplayIndex(displayQuestionIndex);
            int position = optionOrder.IndexOf(originalOptionIndex);
            return position >= 0 ? position : originalOptionIndex;
        }

        private char GetDisplayedOptionLabel(int displayQuestionIndex, int originalOptionIndex)
        {
            return (char)('A' + GetDisplayedOptionPosition(displayQuestionIndex, originalOptionIndex));
        }

        private Label CreateLabel(string text, float size, FontStyle style, Color color)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", size, style),
                ForeColor = color,
                BackColor = Color.Transparent
            };
        }

        private Button CreateButton(string text, Color backColor, Color foreColor)
        {
            var button = new Button
            {
                Text = text,
                BackColor = backColor,
                ForeColor = foreColor,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseDownBackColor = backColor;
            button.FlatAppearance.MouseOverBackColor = backColor;
            button.FlatAppearance.CheckedBackColor = backColor;

            return button;
        }
    }
}
//