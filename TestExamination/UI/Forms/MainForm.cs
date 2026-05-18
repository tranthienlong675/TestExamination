using System;
using System.Windows.Forms;
using TestExamination.model;

namespace TestExamination.UI.Forms
{
    public partial class MainForm : Form
    {
        private Test? currentTest;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            labelTitle.Text = "Quiz System";
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                currentTest = JsonParser.Parse("tests/test.json");

                if (currentTest == null)
                {
                    MessageBox.Show("Test is not valid or failed to load.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using var testForm = new TestForm(currentTest);
                testForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to start test: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
