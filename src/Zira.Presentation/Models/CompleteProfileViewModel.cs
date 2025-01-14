using System;

namespace Zira.Presentation.Models
{
    public class CompleteProfileViewModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? AvatarUrl { get; set; }
    }
}