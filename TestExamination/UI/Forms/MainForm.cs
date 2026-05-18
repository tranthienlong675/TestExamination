using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TestExamination.model;

namespace TestExamination.UI.Forms
{
    public partial class MainForm : Form
    {
        private readonly Color pageBack = Color.FromArgb(247, 247, 252);
        private readonly Color ink = Color.FromArgb(33, 33, 44);
        private readonly Color muted = Color.FromArgb(104, 105, 125);
        private readonly Color quizPurple = Color.FromArgb(123, 76, 255);
        private readonly Color quizCoral = Color.FromArgb(255, 96, 112);
        private readonly Color quizTeal = Color.FromArgb(0, 175, 170);
        private readonly Color softLine = Color.FromArgb(226, 226, 236);

        private Test? currentTest;
        private Panel root = null!;
        private Panel contentPanel = null!;
        private FlowLayoutPanel? navigationPanel;
        private ListBox? questionList;
        private ListBox? submissionList;
        private TextBox? submissionDetail;

        public MainForm()
        {
            InitializeComponent();
            LoadTest();
            BuildShell();
            RenderHome();
        }

        private void LoadTest()
        {
            try
            {
                currentTest = JsonParser.Parse("tests/test.json");
            }
            catch (Exception ex)
            {
                currentTest = null;
                MessageBox.Show("Cannot load test data: " + ex.Message, "Quiz Arena", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BuildShell()
        {
            BackColor = pageBack;
            MinimumSize = new Size(1040, 680);

            root = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24),
                BackColor = pageBack
            };

            Controls.Clear();
            Controls.Add(root);
        }

        private void SetContent()
        {
            root.Controls.Clear();
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = pageBack
            };
            root.Controls.Add(contentPanel);
        }

        private void RenderHome()
        {
            SetContent();

            var hero = CreatePanel(Color.White, new Padding(34), DockStyle.Fill);
            contentPanel.Controls.Add(hero);

            var title = CreateLabel("Quiz Arena", 34, FontStyle.Bold, ink);
            title.Location = new Point(36, 34);
            title.AutoSize = true;
            hero.Controls.Add(title);

            var subtitle = CreateLabel("Choose your role and jump into the exam room.", 13, FontStyle.Regular, muted);
            subtitle.Location = new Point(40, 88);
            subtitle.AutoSize = true;
            hero.Controls.Add(subtitle);

            var stats = CreateLabel(GetTestSummary(), 12, FontStyle.Bold, quizTeal);
            stats.Location = new Point(40, 124);
            stats.AutoSize = true;
            hero.Controls.Add(stats);

            var studentCard = CreateRoleCard(
                "Student",
                "Take the quiz, review progress, and submit your score.",
                quizPurple,
                new Point(40, 190),
                RenderStudentLogin);
            hero.Controls.Add(studentCard);

            var teacherCard = CreateRoleCard(
                "Teacher",
                "Manage questions, imports, exports, and submissions.",
                quizCoral,
                new Point(420, 190),
                RenderTeacherLogin);
            hero.Controls.Add(teacherCard);

            var footer = CreateLabel("Teacher demo account: admin / teacher123", 10, FontStyle.Regular, muted);
            footer.Location = new Point(42, 514);
            footer.AutoSize = true;
            hero.Controls.Add(footer);
        }

        private string GetTestSummary()
        {
            if (currentTest == null)
            {
                return "No test loaded";
            }

            string shuffle = currentTest.ShuffleQuestions || currentTest.ShuffleOptions ? "shuffle on" : "shuffle off";
            return $"{currentTest.Title} - {currentTest.Questions?.Count ?? 0} questions ready - {currentTest.TimeLimitMinutes} minute limit - {shuffle}";
        }

        private Panel CreateRoleCard(string title, string body, Color accent, Point location, Action onClick)
        {
            var card = CreatePanel(Color.FromArgb(252, 252, 255), new Padding(24), DockStyle.None);
            card.Location = location;
            card.Size = new Size(340, 260);
            card.Cursor = Cursors.Hand;

            var accentBar = new Panel
            {
                BackColor = accent,
                Location = new Point(0, 0),
                Size = new Size(340, 10)
            };
            card.Controls.Add(accentBar);

            var titleLabel = CreateLabel(title, 22, FontStyle.Bold, ink);
            titleLabel.Location = new Point(26, 34);
            titleLabel.AutoSize = true;
            card.Controls.Add(titleLabel);

            var bodyLabel = CreateLabel(body, 11, FontStyle.Regular, muted);
            bodyLabel.Location = new Point(28, 88);
            bodyLabel.Size = new Size(280, 64);
            card.Controls.Add(bodyLabel);

            var button = CreateButton("Continue", accent, Color.White);
            button.Location = new Point(28, 182);
            button.Size = new Size(150, 42);
            button.Click += (_, _) => onClick();
            card.Controls.Add(button);

            card.Click += (_, _) => onClick();
            titleLabel.Click += (_, _) => onClick();
            bodyLabel.Click += (_, _) => onClick();
            accentBar.Click += (_, _) => onClick();

            return card;
        }

        private void RenderStudentLogin()
        {
            SetContent();

            var panel = CreatePanel(Color.White, new Padding(34), DockStyle.Fill);
            contentPanel.Controls.Add(panel);

            AddTopBar(panel, "Student Check-in", "Enter your profile before starting the quiz.", RenderHome);

            var nameBox = CreateInput(panel, "Full name", new Point(44, 160));
            var idBox = CreateInput(panel, "Student ID", new Point(44, 250));

            var start = CreateButton("Start Quiz", quizPurple, Color.White);
            start.Location = new Point(44, 350);
            start.Size = new Size(160, 44);
            start.Click += (_, _) =>
            {
                if (currentTest == null || currentTest.Questions == null || currentTest.Questions.Count == 0)
                {
                    MessageBox.Show("No valid questions are available.", "Quiz Arena", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string fullName = string.IsNullOrWhiteSpace(nameBox.Text) ? "Guest Student" : nameBox.Text.Trim();
                string studentId = string.IsNullOrWhiteSpace(idBox.Text) ? "SV001" : idBox.Text.Trim();

                using var quiz = new TestForm(currentTest, studentId, fullName);
                quiz.ShowDialog(this);
                RenderHome();
            };
            panel.Controls.Add(start);
        }

        private void RenderTeacherLogin()
        {
            SetContent();

            var panel = CreatePanel(Color.White, new Padding(34), DockStyle.Fill);
            contentPanel.Controls.Add(panel);

            AddTopBar(panel, "Teacher Sign-in", "Use the admin account to open the dashboard.", RenderHome);

            var usernameBox = CreateInput(panel, "Username", new Point(44, 160));
            var passwordBox = CreateInput(panel, "Password", new Point(44, 250));
            passwordBox.UseSystemPasswordChar = true;

            var login = CreateButton("Open Dashboard", quizCoral, Color.White);
            login.Location = new Point(44, 350);
            login.Size = new Size(180, 44);
            login.Click += (_, _) =>
            {
                if (usernameBox.Text == "admin" && passwordBox.Text == "teacher123")
                {
                    RenderTeacherDashboard("overview");
                    return;
                }

                MessageBox.Show("Wrong teacher account or password.", "Quiz Arena", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            panel.Controls.Add(login);
        }

        private TextBox CreateInput(Control parent, string caption, Point location)
        {
            var label = CreateLabel(caption, 11, FontStyle.Bold, ink);
            label.Location = location;
            label.AutoSize = true;
            parent.Controls.Add(label);

            var box = new TextBox
            {
                Location = new Point(location.X, location.Y + 30),
                Size = new Size(360, 34),
                Font = new Font("Segoe UI", 12F),
                BorderStyle = BorderStyle.FixedSingle
            };
            parent.Controls.Add(box);
            return box;
        }

        private void RenderTeacherDashboard(string section)
        {
            root.Controls.Clear();

            var sidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 245,
                BackColor = Color.FromArgb(39, 38, 56),
                Padding = new Padding(18)
            };
            root.Controls.Add(sidebar);

            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = pageBack,
                Padding = new Padding(24)
            };
            root.Controls.Add(contentPanel);
            contentPanel.BringToFront();

            var brand = CreateLabel("Quiz Arena", 22, FontStyle.Bold, Color.White);
            brand.Location = new Point(18, 24);
            brand.AutoSize = true;
            sidebar.Controls.Add(brand);

            var role = CreateLabel("Teacher dashboard", 10, FontStyle.Regular, Color.FromArgb(198, 197, 216));
            role.Location = new Point(20, 66);
            role.AutoSize = true;
            sidebar.Controls.Add(role);

            navigationPanel = new FlowLayoutPanel
            {
                Location = new Point(18, 120),
                Size = new Size(208, 360),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = sidebar.BackColor
            };
            sidebar.Controls.Add(navigationPanel);

            AddNavButton("Overview", "overview", section);
            AddNavButton("Questions", "questions", section);
            AddNavButton("Settings", "settings", section);
            AddNavButton("Submissions", "submissions", section);
            AddNavButton("Import", "import", section);
            AddNavButton("Export", "export", section);

            var logout = CreateButton("Log out", Color.FromArgb(68, 67, 88), Color.White);
            logout.Location = new Point(18, 540);
            logout.Size = new Size(208, 42);
            logout.Click += (_, _) => RenderHome();
            sidebar.Controls.Add(logout);

            if (section == "questions")
            {
                RenderQuestionsSection();
            }
            else if (section == "settings")
            {
                RenderSettingsSection();
            }
            else if (section == "submissions")
            {
                RenderSubmissionsSection();
            }
            else if (section == "import")
            {
                RenderImportSection();
            }
            else if (section == "export")
            {
                RenderExportSection();
            }
            else
            {
                RenderOverviewSection();
            }
        }

        private void AddNavButton(string text, string key, string active)
        {
            var isActive = key == active;
            var button = CreateButton(text, isActive ? quizPurple : Color.FromArgb(51, 50, 70), Color.White);
            button.Margin = new Padding(0, 0, 0, 12);
            button.Size = new Size(208, 42);
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.Padding = new Padding(14, 0, 0, 0);
            button.Click += (_, _) => RenderTeacherDashboard(key);
            navigationPanel?.Controls.Add(button);
        }

        private void RenderOverviewSection()
        {
            contentPanel.Controls.Clear();
            AddDashboardTitle("Overview", "A quick look at the active test and stored submissions.");

            var questionCount = currentTest?.Questions?.Count ?? 0;
            var timeLimit = currentTest?.TimeLimitMinutes ?? 0;
            var submissionCount = ExamManager.Submissions.Count;
            var averageScore = submissionCount == 0 ? 0 : ExamManager.Submissions.Average(s => s.Score);

            contentPanel.Controls.Add(CreateMetricCard("Questions", questionCount.ToString(), quizPurple, new Point(24, 112)));
            contentPanel.Controls.Add(CreateMetricCard("Time limit", $"{timeLimit}m", quizTeal, new Point(238, 112)));
            contentPanel.Controls.Add(CreateMetricCard("Submissions", submissionCount.ToString(), quizCoral, new Point(452, 112)));
            contentPanel.Controls.Add(CreateMetricCard("Average score", averageScore.ToString("0.##"), Color.FromArgb(45, 45, 64), new Point(666, 112)));

            var info = CreatePanel(Color.White, new Padding(22), DockStyle.None);
            info.Location = new Point(24, 310);
            info.Size = new Size(792, 170);
            contentPanel.Controls.Add(info);

            var label = CreateLabel(GetTestSummary(), 17, FontStyle.Bold, ink);
            label.Location = new Point(24, 22);
            label.AutoSize = true;
            info.Controls.Add(label);

            var body = CreateLabel("The WinForms interface uses the existing JSON parser, question models, and ExamManager data store.", 11, FontStyle.Regular, muted);
            body.Location = new Point(26, 72);
            body.Size = new Size(700, 50);
            info.Controls.Add(body);
        }

        private Panel CreateMetricCard(string title, string value, Color accent, Point location)
        {
            var card = CreatePanel(Color.White, new Padding(20), DockStyle.None);
            card.Location = location;
            card.Size = new Size(190, 150);

            var valueLabel = CreateLabel(value, 32, FontStyle.Bold, accent);
            valueLabel.Location = new Point(22, 24);
            valueLabel.AutoSize = true;
            card.Controls.Add(valueLabel);

            var titleLabel = CreateLabel(title, 11, FontStyle.Bold, muted);
            titleLabel.Location = new Point(26, 92);
            titleLabel.AutoSize = true;
            card.Controls.Add(titleLabel);

            return card;
        }

        private void RenderSettingsSection()
        {
            contentPanel.Controls.Clear();
            AddDashboardTitle("Settings", "Adjust timing and randomization for the next student attempts.");

            var panel = CreatePanel(Color.White, new Padding(24), DockStyle.None);
            panel.Location = new Point(24, 112);
            panel.Size = new Size(650, 330);
            contentPanel.Controls.Add(panel);

            var label = CreateLabel("Time limit in minutes", 12, FontStyle.Bold, ink);
            label.Location = new Point(24, 28);
            label.AutoSize = true;
            panel.Controls.Add(label);

            var minutes = new NumericUpDown
            {
                Location = new Point(24, 66),
                Size = new Size(160, 34),
                Font = new Font("Segoe UI", 12F),
                Minimum = 1,
                Maximum = 240,
                Value = currentTest?.TimeLimitMinutes > 0 ? currentTest.TimeLimitMinutes : 15
            };
            panel.Controls.Add(minutes);

            var shuffleQuestions = new CheckBox
            {
                Text = "Shuffle question order",
                Location = new Point(24, 118),
                Size = new Size(260, 28),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = ink,
                BackColor = Color.White,
                Checked = currentTest?.ShuffleQuestions ?? true
            };
            panel.Controls.Add(shuffleQuestions);

            var shuffleOptions = new CheckBox
            {
                Text = "Shuffle answer options",
                Location = new Point(24, 154),
                Size = new Size(260, 28),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = ink,
                BackColor = Color.White,
                Checked = currentTest?.ShuffleOptions ?? true
            };
            panel.Controls.Add(shuffleOptions);

            var hint = CreateLabel("The timer starts when a student opens the quiz. Shuffling creates a fresh order for every attempt without changing the original question list.", 10, FontStyle.Regular, muted);
            hint.Size = new Size(560, 50);
            hint.Location = new Point(24, 198);
            panel.Controls.Add(hint);

            var save = CreateButton("Save Settings", quizPurple, Color.White);
            save.Location = new Point(24, 250);
            save.Size = new Size(170, 42);
            save.Click += (_, _) =>
            {
                if (currentTest == null)
                {
                    MessageBox.Show("No test is loaded.", "Quiz Arena", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                currentTest.TimeLimitMinutes = (int)minutes.Value;
                currentTest.ShuffleQuestions = shuffleQuestions.Checked;
                currentTest.ShuffleOptions = shuffleOptions.Checked;
                MessageBox.Show("Test settings updated.", "Quiz Arena", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RenderTeacherDashboard("overview");
            };
            panel.Controls.Add(save);
        }

        private void RenderQuestionsSection()
        {
            contentPanel.Controls.Clear();
            AddDashboardTitle("Questions", "Review, import, or remove questions from the current in-memory test.");

            questionList = new ListBox
            {
                Location = new Point(24, 112),
                Size = new Size(710, 360),
                Font = new Font("Segoe UI", 11F),
                BorderStyle = BorderStyle.FixedSingle
            };
            contentPanel.Controls.Add(questionList);
            RefreshQuestionList();

            var delete = CreateButton("Delete Selected", quizCoral, Color.White);
            delete.Location = new Point(754, 112);
            delete.Size = new Size(170, 42);
            delete.Click += (_, _) => DeleteSelectedQuestion();
            contentPanel.Controls.Add(delete);

            var reload = CreateButton("Reload JSON", Color.FromArgb(45, 45, 64), Color.White);
            reload.Location = new Point(754, 168);
            reload.Size = new Size(170, 42);
            reload.Click += (_, _) =>
            {
                LoadTest();
                RefreshQuestionList();
                MessageBox.Show("Test data reloaded.", "Quiz Arena", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            contentPanel.Controls.Add(reload);
        }

        private void RefreshQuestionList()
        {
            if (questionList == null)
            {
                return;
            }

            questionList.Items.Clear();
            var questions = currentTest?.Questions;
            if (questions == null || questions.Count == 0)
            {
                questionList.Items.Add("No questions available.");
                return;
            }

            for (int i = 0; i < questions.Count; i++)
            {
                questionList.Items.Add($"{i + 1}. [{questions[i].GetType().Name}] {questions[i].Content}");
            }
        }

        private void DeleteSelectedQuestion()
        {
            if (currentTest?.Questions == null || currentTest.Questions.Count == 0 || questionList == null)
            {
                return;
            }

            int index = questionList.SelectedIndex;
            if (index < 0 || index >= currentTest.Questions.Count)
            {
                MessageBox.Show("Select a question first.", "Quiz Arena", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var question = currentTest.Questions[index];
            var confirm = MessageBox.Show("Delete this question?\n\n" + question.Content, "Quiz Arena", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            currentTest.Questions.RemoveAt(index);
            RefreshQuestionList();
        }

        private void RenderSubmissionsSection()
        {
            contentPanel.Controls.Clear();
            AddDashboardTitle("Submissions", "Inspect stored attempts from this app session.");

            submissionList = new ListBox
            {
                Location = new Point(24, 112),
                Size = new Size(300, 380),
                Font = new Font("Segoe UI", 11F)
            };
            submissionList.SelectedIndexChanged += (_, _) => ShowSelectedSubmission();
            contentPanel.Controls.Add(submissionList);

            submissionDetail = new TextBox
            {
                Location = new Point(348, 112),
                Size = new Size(560, 380),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 10F),
                BorderStyle = BorderStyle.FixedSingle
            };
            contentPanel.Controls.Add(submissionDetail);

            RefreshSubmissionList();
        }

        private void RefreshSubmissionList()
        {
            if (submissionList == null)
            {
                return;
            }

            submissionList.Items.Clear();
            foreach (var submission in ExamManager.Submissions)
            {
                submissionList.Items.Add($"{submission.StudentId} - {submission.StudentName} - {submission.Score:0.##}/10");
            }

            if (submissionList.Items.Count > 0)
            {
                submissionList.SelectedIndex = 0;
            }
        }

        private void ShowSelectedSubmission()
        {
            if (submissionList == null || submissionDetail == null || submissionList.SelectedIndex < 0)
            {
                return;
            }

            var submission = ExamManager.Submissions[submissionList.SelectedIndex];
            var lines = new List<string>
            {
                $"Student: {submission.StudentName}",
                $"ID: {submission.StudentId}",
                $"Score: {submission.Score:0.##}/10",
                $"Status: {(submission.IsGraded ? "Graded" : "Pending")}",
                "",
                "Answers:"
            };

            for (int i = 0; i < submission.Answers.Count; i++)
            {
                lines.Add($"{i + 1}. {submission.Answers[i].QuestionContent}");
                lines.Add("   " + submission.Answers[i].UserAnswer);
            }

            submissionDetail.Text = string.Join(Environment.NewLine, lines);
        }

        private void RenderImportSection()
        {
            contentPanel.Controls.Clear();
            AddDashboardTitle("Import", "Add fill-in questions from .txt or .xlsx files.");

            var panel = CreatePanel(Color.White, new Padding(24), DockStyle.None);
            panel.Location = new Point(24, 112);
            panel.Size = new Size(650, 220);
            contentPanel.Controls.Add(panel);

            var text = CreateLabel("TXT format: question | answer. Excel rows should contain type, content, and answer columns.", 11, FontStyle.Regular, muted);
            text.Location = new Point(24, 24);
            text.Size = new Size(560, 60);
            panel.Controls.Add(text);

            var import = CreateButton("Choose File", quizTeal, Color.White);
            import.Location = new Point(24, 112);
            import.Size = new Size(150, 42);
            import.Click += (_, _) =>
            {
                if (currentTest == null)
                {
                    MessageBox.Show("No test is loaded.", "Quiz Arena", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using var dialog = new OpenFileDialog
                {
                    Filter = "Question files (*.txt;*.xlsx)|*.txt;*.xlsx|All files (*.*)|*.*",
                    RestoreDirectory = true
                };

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    ExamManager.ImportQuestions(currentTest, dialog.FileName);
                    MessageBox.Show("Import completed. Check the Questions page for details.", "Quiz Arena", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };
            panel.Controls.Add(import);
        }

        private void RenderExportSection()
        {
            contentPanel.Controls.Clear();
            AddDashboardTitle("Export", "Export one student's stored answers to an Excel file.");

            var panel = CreatePanel(Color.White, new Padding(24), DockStyle.None);
            panel.Location = new Point(24, 112);
            panel.Size = new Size(650, 260);
            contentPanel.Controls.Add(panel);

            var idBox = CreateInput(panel, "Student ID", new Point(24, 28));

            var export = CreateButton("Export Excel", quizPurple, Color.White);
            export.Location = new Point(24, 130);
            export.Size = new Size(160, 42);
            export.Click += (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(idBox.Text))
                {
                    MessageBox.Show("Enter a student ID first.", "Quiz Arena", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ExamManager.ExportSubmissionToExcel(idBox.Text.Trim());
            };
            panel.Controls.Add(export);
        }

        private void AddDashboardTitle(string title, string subtitle)
        {
            var titleLabel = CreateLabel(title, 26, FontStyle.Bold, ink);
            titleLabel.Location = new Point(24, 22);
            titleLabel.AutoSize = true;
            contentPanel.Controls.Add(titleLabel);

            var subtitleLabel = CreateLabel(subtitle, 11, FontStyle.Regular, muted);
            subtitleLabel.Location = new Point(28, 68);
            subtitleLabel.AutoSize = true;
            contentPanel.Controls.Add(subtitleLabel);
        }

        private void AddTopBar(Control parent, string title, string subtitle, Action backAction)
        {
            var back = CreateButton("Back", Color.FromArgb(45, 45, 64), Color.White);
            back.Location = new Point(44, 38);
            back.Size = new Size(92, 36);
            back.Click += (_, _) => backAction();
            parent.Controls.Add(back);

            var titleLabel = CreateLabel(title, 27, FontStyle.Bold, ink);
            titleLabel.Location = new Point(44, 90);
            titleLabel.AutoSize = true;
            parent.Controls.Add(titleLabel);

            var subtitleLabel = CreateLabel(subtitle, 11, FontStyle.Regular, muted);
            subtitleLabel.Location = new Point(48, 130);
            subtitleLabel.AutoSize = true;
            parent.Controls.Add(subtitleLabel);
        }

        private Panel CreatePanel(Color backColor, Padding padding, DockStyle dock)
        {
            return new Panel
            {
                BackColor = backColor,
                Padding = padding,
                Dock = dock
            };
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
            return new Button
            {
                Text = text,
                BackColor = backColor,
                ForeColor = foreColor,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }
    }
}
