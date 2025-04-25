using System.ComponentModel.DataAnnotations;
public class ClassInformationModel
{
    private static int _nextId = 1;

    public ClassInformationModel()
    {
        Id = _nextId++;
    }

    public int Id { get; private set; }

    [Required]
    public string ClassName { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Student count must be at least 1")]
    public int StudentCount { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;
}
