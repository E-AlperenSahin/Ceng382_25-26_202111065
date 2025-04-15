using System.ComponentModel.DataAnnotations;

namespace LabProject5.Models
{
    public class ClassInformationModel
    {
        private static int _nextId = 1;

        public ClassInformationModel()
        {
            Id = _nextId++;
        }

        public int Id { get; set; }


        [Required]
        public string ClassName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Student count must be at least 1")]
        public int StudentCount { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
