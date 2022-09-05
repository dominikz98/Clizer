namespace CLIzer.Run;

public class StudentEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime Birthday { get; set; }

    public StudentEntity(int id, string name, DateTime birthday)
    {
        Id = id;
        Name = name;
        Birthday = birthday;
    }
}
