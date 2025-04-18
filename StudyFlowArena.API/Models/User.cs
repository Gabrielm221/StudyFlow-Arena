public class User{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; }
    public string PasswordHash{ get; set; } = null!;
    public string Role{ get; set; } = "Student";
    public int XP{ get; set; } = 0;
    public int Level{ get; set;} = 1;
}