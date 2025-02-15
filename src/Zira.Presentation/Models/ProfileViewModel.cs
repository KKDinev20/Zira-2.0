using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Zira.Presentation.Models
{
    public class ProfileViewModel
    {
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        public required DateTime BirthDate { get; set; }

        public string AvatarUrl { get; set; }
        public IFormFile AvatarFile { get; set; }
    }
}
