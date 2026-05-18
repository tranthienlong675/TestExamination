using System;
using System.Collections.Generic;

namespace TestExamination.model
{
    // Lớp lưu vết từng câu trả lời cụ thể của học sinh
    public class StudentAnswer
    {
        public string QuestionContent { get; set; } = string.Empty;
        public string UserAnswer { get; set; } = string.Empty;
    }

    // Lớp tổng hợp toàn bộ bài thi của học sinh phục vụ việc xem lại và xuất Excel
    public class StudentSubmission
    {
        public string StudentId { get; set; } = string.Empty; // Mã số sinh viên / Username
        public string StudentName { get; set; } = string.Empty; // Họ và tên
        public double Score { get; set; } // Điểm số đạt được
        public bool IsGraded { get; set; } // Trạng thái đã chấm bài hay chưa
        public List<StudentAnswer> Answers { get; set; } = new List<StudentAnswer>();
    }
}