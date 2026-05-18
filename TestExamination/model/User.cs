namespace TestExamination.model
{
    public enum UserRole { Student, Teacher }

    public class User
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string FullName { get; set; } = string.Empty;

        public User() { }
        
        public User(string username, string password, UserRole role, string fullName)
        {
            Id = Guid.NewGuid().ToString();
            Username = username;
            Password = password;
            Role = role;
            FullName = fullName;
        }
    }
}