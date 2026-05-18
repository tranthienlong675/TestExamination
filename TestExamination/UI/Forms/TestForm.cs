using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using TestExamination.model;

namespace TestExamination.UI.Forms
{
    public partial class TestForm : Form
    {
        private Test test;
        private int currentIndex = 0;
        private double perQuestionScore = 0;
        private double totalScore = 10;

        public TestForm(Test test)
        {
            this.test = test;
            InitializeComponent();
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            lblTitle.Text = test.Title;
            perQuestionScore = 10.0 / test.Questions.Count;
            ShowQuestion();
        }

        private void ShowQuestion()
        {
            var q = test.Questions[currentIndex];
            txtContent.Text = q.Content;
            pnlOptions.Controls.Clear();

            if (q is SingleChoiceQuestion scq)
            {
                int i = 0;
                foreach (var opt in scq.Options)
                {
                    var rb = new RadioButton();
                    rb.Text = $"{(char)('a' + i)}. {opt.Content}";
                    rb.Tag = i;
                    rb.AutoSize = true;
                    rb.Location = new Point(10, 10 + i * 30);
                    pnlOptions.Controls.Add(rb);
                    i++;
                }
            }
            else if (q is MultipleChoiceQuestion mcq)
            {
                int i = 0;
                foreach (var opt in mcq.Options)
                {
                    var cb = new CheckBox();
                    cb.Text = $"{(char)('a' + i)}. {opt.Content}";
                    cb.Tag = i;
                    cb.AutoSize = true;
                    cb.Location = new Point(10, 10 + i * 30);
                    pnlOptions.Controls.Add(cb);
                    i++;
                }
            }
            else if (q is FillBlankQuestion fbq)
            {
                var tb = new TextBox();
                tb.Name = "txtAnswer";
                tb.Width = pnlOptions.Width - 20;
                tb.Location = new Point(10, 10);
                pnlOptions.Controls.Add(tb);
            }

            lblProgress.Text = $"Question {currentIndex + 1}/{test.Questions.Count}";
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            var q = test.Questions[currentIndex];

            bool isCorrect = false;

            if (q is SingleChoiceQuestion scq)
            {
                var rb = pnlOptions.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
                if (rb != null)
                {
                    int idx = (int)rb.Tag;
                    isCorrect = scq.Options[idx].IsCorrect;
                }
            }
            else if (q is MultipleChoiceQuestion mcq)
            {
                var checkedIdx = pnlOptions.Controls.OfType<CheckBox>().Where(c => c.Checked).Select(c => (int)c.Tag).ToHashSet();
                var correctIdx = mcq.Options.Select((o, idx) => new { o, idx }).Where(x => x.o.IsCorrect).Select(x => x.idx).ToHashSet();
                isCorrect = checkedIdx.SetEquals(correctIdx);
            }
            else if (q is FillBlankQuestion fbq)
            {
                var tb = pnlOptions.Controls.OfType<TextBox>().FirstOrDefault();
                if (tb != null)
                {
                    isCorrect = string.Equals(tb.Text.Trim(), fbq.CorrectAnswer, StringComparison.OrdinalIgnoreCase);
                }
            }

            if (!isCorrect)
            {
                totalScore -= perQuestionScore;
            }

            currentIndex++;

            if (currentIndex >= test.Questions.Count)
            {
                MessageBox.Show($"Your score: {totalScore:0.##}", "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                return;
            }

            ShowQuestion();
        }
    }
}
