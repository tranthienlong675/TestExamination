using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using MiniExcelLibs; 
using TestExamination.model; 

namespace TestExamination
{
    // =========================================================================
    // 0. ĐỊNH NGHĨA CỦA CÁC LỚP LƯU TRỮ BÀI LÀM (GỘP CHUNG)
    // =========================================================================
    public class StudentAnswer
    {
        public string QuestionContent { get; set; } = string.Empty;
        public string UserAnswer { get; set; } = string.Empty;
    }

    public class StudentSubmission
    {
        public string StudentId { get; set; } = string.Empty; 
        public string StudentName { get; set; } = string.Empty; 
        public double Score { get; set; } 
        public bool IsGraded { get; set; } 
        public List<StudentAnswer> Answers { get; set; } = new List<StudentAnswer>();
    }

    // =========================================================================
    // 1. LỚP QUẢN LÝ DỮ LIỆU THI (XEM BÀI, IMPORT/EXPORT)
    // =========================================================================
    public static class ExamManager
    {
        public static List<StudentSubmission> Submissions { get; set; } = new List<StudentSubmission>();

        public static void ReviewStudentSubmission(string studentId)
        {
            var submission = Submissions.FirstOrDefault(s => s.StudentId.Equals(studentId, StringComparison.OrdinalIgnoreCase));
            if (submission == null)
            {
                Console.WriteLine($"[ERROR] Không tìm thấy bài nộp nào cho MSSV: {studentId}");
                return;
            }

            Console.WriteLine($"\n=== CHI TIẾT BÀI LÀM: {submission.StudentName} ({submission.StudentId}) ===");
            Console.WriteLine($"Trạng thái: {(submission.IsGraded ? "Đã chấm" : "Chưa chấm")} | Điểm số: {submission.Score}/10");
            Console.WriteLine("-----------------------------------------------------------------");
            
            int idx = 1;
            foreach (var ans in submission.Answers)
            {
                Console.WriteLine($"Câu {idx}: {ans.QuestionContent}");
                Console.WriteLine($" => Học sinh phản hồi: {ans.UserAnswer}");
                Console.WriteLine("-----------------------------------------------------------------");
                idx++;
            }
        }

        public static void ExportSubmissionToExcel(string studentId)
        {
            var submission = Submissions.FirstOrDefault(s => s.StudentId.Equals(studentId, StringComparison.OrdinalIgnoreCase));
            if (submission == null)
            {
                Console.WriteLine("[ERROR] Không thể xuất file. Không tìm thấy học sinh.");
                return;
            }

            Console.WriteLine("Đang mở cửa sổ lưu file... (Kiểm tra dưới thanh Taskbar nếu bị che)");
            
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.FileName = $"Submission_{studentId}.xlsx"; 

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    var excelData = submission.Answers.Select((ans, index) => new
                    {
                        STT = index + 1,
                        NoiDungCauHoi = ans.QuestionContent,
                        DapAnHocSinh = ans.UserAnswer
                    }).ToList();

                    try
                    {
                        MiniExcel.SaveAs(filePath, excelData);
                        Console.WriteLine($"[SUCCESS] Đã xuất file thành công tại địa chỉ bạn chọn: {filePath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Lỗi xuất file: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("[INFO] Bạn đã hủy tiến trình lưu file Excel.");
                }
            }
        }

        public static void ImportQuestions(Test currentTest, string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("[ERROR] Đường dẫn file không tồn tại!");
                return;
            }

            string extension = Path.GetExtension(filePath).ToLower();

            try
            {
                if (extension == ".txt")
                {
                    string[] lines = File.ReadAllLines(filePath);
                    int count = 0;
                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line) || !line.Contains("|")) continue;
                        
                        var parts = line.Split('|');
                        if (parts.Length < 2) continue;

                        string type = parts[0].Trim().ToLower();
                        string content = parts[1].Trim();

                        try
                        {
                            if (type == "fill" && parts.Length >= 3)
                            {
                                string answer = parts[2].Trim();
                                currentTest.Questions.Add(new FillBlankQuestion(content, answer));
                                count++;
                            }
                            else if (type == "single" && parts.Length >= 4)
                            {
                                var options = ParseOptions(parts, 2);
                                if (options.Count >= 2 && options.Count(o => o.IsCorrect) == 1)
                                {
                                    currentTest.Questions.Add(new SingleChoiceQuestion(content, options));
                                    count++;
                                }
                            }
                            else if (type == "multiple" && parts.Length >= 4)
                            {
                                var options = ParseOptions(parts, 2);
                                if (options.Count >= 2)
                                {
                                    currentTest.Questions.Add(new MultipleChoiceQuestion(content, options));
                                    count++;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[WARNING] Dòng không hợp lệ: {line} - {ex.Message}");
                        }
                    }
                    Console.WriteLine($"[SUCCESS] Đã import thành công {count} câu hỏi từ file TXT.");
                    
                    // Lưu vào file JSON
                    JsonParser.SaveTestToJson(currentTest, "tests/test.json");
                }
                else if (extension == ".xlsx")
                {
                    var rows = MiniExcel.Query(filePath).ToList();
                    int count = 0;
                    for (int i = 1; i < rows.Count; i++)
                    {
                        var rowData = rows[i] as IDictionary<string, object>;
                        if (rowData == null) continue;

                        string type = rowData.Values.ElementAtOrDefault(0)?.ToString() ?? "";
                        string content = rowData.Values.ElementAtOrDefault(1)?.ToString() ?? "";
                        string answer = rowData.Values.ElementAtOrDefault(2)?.ToString() ?? "";

                        try
                        {
                            if (type.ToLower() == "fill" && !string.IsNullOrEmpty(content))
                            {
                                currentTest.Questions.Add(new FillBlankQuestion(content, answer));
                                count++;
                            }
                            else if (type.ToLower() == "single" && !string.IsNullOrEmpty(content))
                            {
                                var optionsJson = rowData.Values.ElementAtOrDefault(3)?.ToString() ?? "[]";
                                var options = JsonSerializer.Deserialize<List<Option>>(optionsJson) ?? new List<Option>();
                                if (options.Count >= 2 && options.Count(o => o.IsCorrect) == 1)
                                {
                                    currentTest.Questions.Add(new SingleChoiceQuestion(content, options));
                                    count++;
                                }
                            }
                            else if (type.ToLower() == "multiple" && !string.IsNullOrEmpty(content))
                            {
                                var optionsJson = rowData.Values.ElementAtOrDefault(3)?.ToString() ?? "[]";
                                var options = JsonSerializer.Deserialize<List<Option>>(optionsJson) ?? new List<Option>();
                                if (options.Count >= 2)
                                {
                                    currentTest.Questions.Add(new MultipleChoiceQuestion(content, options));
                                    count++;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[WARNING] Dòng không hợp lệ: {ex.Message}");
                        }
                    }
                    Console.WriteLine($"[SUCCESS] Đã import thành công {count} câu hỏi từ file Excel.");
                    
                    // Lưu vào file JSON
                    JsonParser.SaveTestToJson(currentTest, "tests/test.json");
                }
                else
                {
                    Console.WriteLine("[ERROR] Định dạng file không hỗ trợ. Chỉ nhận .txt hoặc .xlsx");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Có lỗi xảy ra khi import: {ex.Message}");
            }
        }

        private static List<Option> ParseOptions(string[] parts, int startIndex)
        {
            var options = new List<Option>();
            for (int i = startIndex; i < parts.Length; i++)
            {
                string optionPart = parts[i].Trim();
                var optionData = optionPart.Split(',');
                
                if (optionData.Length >= 2)
                {
                    string optionContent = optionData[0].Trim();
                    bool isCorrect = bool.TryParse(optionData[1].Trim(), out var result) && result;
                    
                    options.Add(new Option(optionContent, isCorrect));
                }
            }
            return options;
        }

        public static void GenerateMockData()
        {
            var mockSubmission = new StudentSubmission
            {
                StudentId = "B22DCCN001",
                StudentName = "Nguyen Van A",
                Score = 10,
                IsGraded = true
            };
            mockSubmission.Answers.Add(new StudentAnswer { QuestionContent = "Lập trình OOP viết tắt của từ gì?", UserAnswer = "Object Oriented Programming" });
            mockSubmission.Answers.Add(new StudentAnswer { QuestionContent = "Từ khóa kế thừa lớp cha trong C# là gì?", UserAnswer = "base" });
            
            Submissions.Add(mockSubmission);
        }
    }

    // =========================================================================
    // 2. LỚP ĐIỀU HƯỚNG CHƯƠNG TRÌNH CHÍNH (APP)
    // =========================================================================
    class App
    {
        public static User? CurrentUser { get; set; }
        public static Test? SharedTest { get; set; } 

        public static void Validate()
        {
            if (SharedTest == null)
            {
                SharedTest = JsonParser.Parse("tests/test.json");
            }

            if (SharedTest != null)
            {
                Init(SharedTest);
            }
            else
            {
                Console.WriteLine("Test closed");
            }
        }

        public static void Init(Test test)
        {
            Console.WriteLine("\n==================================================");
            Console.WriteLine("Welcome to the Test Examination system.");
            Console.WriteLine("==================================================");
            
            UserRole role = UserRole.Student;
            string fullName = "Guest";
            string username = "user01";

            while (true)
            {
                Console.WriteLine("Bạn đăng nhập với tư cách là ai?");
                Console.WriteLine("1. Student (Học sinh)");
                Console.WriteLine("2. Teacher (Giáo viên)");
                Console.Write("Lựa chọn của bạn (1 hoặc 2): ");
                string? choice = Console.ReadLine();

                if (choice == "1")
                {
                    role = UserRole.Student;
                    
                    Console.WriteLine("\n--- ĐĂNG NHẬP HỌC SINH ---");
                    Console.Write("Nhập Họ và Tên của bạn: ");
                    fullName = Console.ReadLine() ?? "Học sinh Khách";

                    Console.Write("Nhập Mã số sinh viên (MSSV): ");
                    username = Console.ReadLine() ?? "SV001";
                    break;
                }
                else if (choice == "2")
                {
                    role = UserRole.Teacher;
                    
                    Console.WriteLine("\n--- ĐĂNG NHẬP GIÁO VIÊN SYSTEM ---");
                    while (true)
                    {
                        Console.Write("Nhập tài khoản quản trị: ");
                        string? inputUser = Console.ReadLine();

                        Console.Write("Nhập mật khẩu: ");
                        string? inputPass = Console.ReadLine();

                        if (inputUser == "admin" && inputPass == "teacher123")
                        {
                            fullName = "Hội đồng Giáo viên";
                            username = "admin_teacher";
                            break; 
                        }
                        else
                        {
                            Console.WriteLine("[ERROR] Sai tài khoản hoặc mật khẩu giáo viên! Vui lòng thử lại.\n");
                        }
                    }
                    break;
                }
                else
                {
                    Console.WriteLine("[ERROR] Lựa chọn không hợp lệ. Vui lòng gõ phím 1 hoặc 2.\n");
                }
            }

            CurrentUser = new User(username, "123456", role, fullName);

            Console.WriteLine("\n[SUCCESS] Xác thực hồ sơ thành công:");
            Console.WriteLine($"- Người dùng: {CurrentUser.FullName}");
            Console.WriteLine($"- Quyền hạn: {CurrentUser.Role}");
            Console.WriteLine("--------------------------------------------------");

            if (CurrentUser.Role == UserRole.Teacher)
            {
                TeacherMenu(test);
            }
            else
            {
                Console.WriteLine($"\nSẵn sàng làm bài thi: {test.Title}");
                Console.WriteLine($"Tổng số câu hỏi trong đề: {test.Questions?.Count ?? 0} câu.");
                Console.WriteLine("\nNhấn phím [ENTER] để chính thức làm bài bắt đầu tính điểm.\n");
                
                var key = Console.ReadKey();
                while (key.Key != ConsoleKey.Enter)
                {
                    key = Console.ReadKey();
                }
                Run(test);
            }
        }

        public static void TeacherMenu(Test test)
        {
            while (true)
            {
                Console.WriteLine("\n==================================================");
                Console.WriteLine($"TEACHER DASHBOARD - Welcome, {CurrentUser?.FullName}");
                Console.WriteLine("==================================================");
                Console.WriteLine("1. Xem tiêu đề bài kiểm tra & Số lượng câu hỏi hiện tại");
                Console.WriteLine("2. Xem danh sách câu hỏi trong hệ thống");
                Console.WriteLine("3. Xem lại bài làm của học sinh qua MSSV");
                Console.WriteLine("4. Chọn vị trí xuất chi tiết bài làm học sinh ra file Excel");
                Console.WriteLine("5. Import thêm câu hỏi từ File (.txt hoặc .xlsx)");
                Console.WriteLine("6. XÓA CÂU HỎI HIỆN TẠI (MỚI)"); // <--- OPTION MỚI ĐÂY BRO
                Console.WriteLine("7. Đăng xuất / Thay đổi tài khoản");
                Console.Write("Nhập tùy chọn (1-7): ");
                
                string? option = Console.ReadLine();
                Console.WriteLine("--------------------------------------------------");

                if (option == "1")
                {
                    Console.WriteLine($"Tên bài thi: {test.Title}");
                    Console.WriteLine($"Tổng số câu hỏi: {test.Questions?.Count ?? 0}");
                }
                else if (option == "2")
                {
                    Console.WriteLine($"Danh sách câu hỏi trong bộ đề [{test.Title}]:\n");
                    if (test.Questions == null || test.Questions.Count == 0)
                    {
                        Console.WriteLine("Chưa có câu hỏi nào.");
                    }
                    else
                    {
                        int index = 1;
                        foreach (var q in test.Questions)
                        {
                            Console.WriteLine($"Câu {index} ({q.Description}):");
                            q.Display();
                            Console.WriteLine();
                            index++;
                        }
                    }
                }
                else if (option == "3")
                {
                    Console.Write("Nhập MSSV học sinh muốn xem lại: ");
                    string? id = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(id)) ExamManager.ReviewStudentSubmission(id.Trim());
                }
                else if (option == "4")
                {
                    Console.Write("Nhập MSSV học sinh muốn xuất file Excel: ");
                    string? id = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(id)) ExamManager.ExportSubmissionToExcel(id.Trim());
                }
                else if (option == "5")
                {
                    Console.WriteLine("Đang mở cửa sổ chọn file... (Kiểm tra dưới thanh Taskbar nếu cửa sổ bị che)");
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "File câu hỏi (*.txt;*.xlsx)|*.txt;*.xlsx|All files (*.*)|*.*";
                        openFileDialog.FilterIndex = 1;
                        openFileDialog.RestoreDirectory = true;

                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            string selectedFilePath = openFileDialog.FileName;
                            Console.WriteLine($"[INFO] Bạn đã chọn file: {selectedFilePath}");
                            ExamManager.ImportQuestions(test, selectedFilePath);
                        }
                        else
                        {
                            Console.WriteLine("[INFO] Bạn đã hủy chọn file.");
                        }
                    }
                }
                // =========================================================================
                // CHỨC NĂNG MỚI: XÓA CÂU HỎI THEO CHỈ SỐ (STT)
                // =========================================================================
                else if (option == "6")
                {
                    if (test.Questions == null || test.Questions.Count == 0)
                    {
                        Console.WriteLine("[INFO] Hệ thống hiện tại không có câu hỏi nào để xóa.");
                        continue;
                    }

                    Console.WriteLine("--- DANH SÁCH CÂU HỎI ĐANG CÓ ---");
                    for (int i = 0; i < test.Questions.Count; i++)
                    {
                        Console.WriteLine($"[{i + 1}] - {test.Questions[i].Content}");
                    }
                    Console.WriteLine("---------------------------------");
                    Console.Write("Nhập Số Thứ Tự câu hỏi bạn muốn xóa (Hoặc ấn Enter để hủy): ");
                    string? inputIdx = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(inputIdx))
                    {
                        Console.WriteLine("[INFO] Đã hủy tiến trình xóa câu hỏi.");
                        continue;
                    }

                    // Ép kiểu chuỗi nhập vào sang số nguyên và kiểm tra tính hợp lệ
                    if (int.TryParse(inputIdx, out int indexToDelete))
                    {
                        // Vì danh sách hiển thị từ 1 -> N nhưng Index trong List chạy từ 0 -> N-1
                        int realIndex = indexToDelete - 1;

                        if (realIndex >= 0 && realIndex < test.Questions.Count)
                        {
                            string removedContent = test.Questions[realIndex].Content;
                            test.Questions.RemoveAt(realIndex); // Xóa phần tử khỏi danh sách trên RAM
                            Console.WriteLine($"[SUCCESS] Đã xóa thành công Câu số {indexToDelete}: \"{removedContent}\"");
                        }
                        else
                        {
                            Console.WriteLine($"[ERROR] Số thứ tự {indexToDelete} nằm ngoài phạm vi hiện có!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("[ERROR] Vui lòng nhập vào một số nguyên hợp lệ!");
                    }
                }
                else if (option == "7")
                {
                    Console.WriteLine("Đang quay trở lại màn hình đăng nhập chính...");
                    Validate(); 
                    break;
                }
                else
                {
                    Console.WriteLine("Lựa chọn không hợp lệ. Vui lòng chọn lại.");
                }
            }
        }

        public static void Run(Test test)
        {
            Console.WriteLine("Starting the test...\n");
            
            if (test.Questions == null || test.Questions.Count == 0)
            {
                Console.WriteLine("No questions found in this test.");
                return;
            }

            var currentSubmission = new StudentSubmission
            {
                StudentId = CurrentUser?.Username ?? "UnknownID",
                StudentName = CurrentUser?.FullName ?? "UnknownStudent"
            };

            double scorePerQuestion = 10.0 / test.Questions.Count;
            double finalScore = 0;

            foreach (var item in test.Questions)
            {
                item.Display(); 
                item.GetUserAnswer(); 

                currentSubmission.Answers.Add(new StudentAnswer
                {
                    QuestionContent = item.Content,
                    UserAnswer = item.IsCorrect ? "Đúng (Chính xác)" : "Sai hoặc sai cú pháp đáp án mẫu"
                });

                if (item.IsCorrect)
                {
                    finalScore += scorePerQuestion;
                }
            }

            currentSubmission.Score = Math.Round(finalScore, 2);
            currentSubmission.IsGraded = true;
            
            ExamManager.Submissions.Add(currentSubmission);

            Console.WriteLine("\n==================================================");
            Console.WriteLine($"EXAM RESULT FOR: {CurrentUser?.FullName.ToUpper()} ({CurrentUser?.Username})");
            Console.WriteLine($"Your score is: {currentSubmission.Score} / 10");
            Console.WriteLine("Bài làm của bạn đã được lưu vào hệ thống.");
            Console.WriteLine("==================================================");

            Console.WriteLine("\nBấm phím bất kỳ để quay lại màn hình phân quyền đăng nhập...");
            Console.ReadKey();
            Validate();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ExamManager.GenerateMockData();
            Application.Run(new TestExamination.UI.Forms.MainForm());
            return;
        }
    }
}
