using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Zira.Data.Models;

namespace Zira.Presentation.Models
{
    public class ProfileViewModel
    {
        [Required(
            ErrorMessageResourceType = typeof(Common.AccountText),
            ErrorMessageResourceName = "FieldIsRequired")]
        public string FirstName { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Common.AccountText),
            ErrorMessageResourceName = "FieldIsRequired")]
        public string LastName { get; set; }

        [EmailAddress]
        [Required(
            ErrorMessageResourceType = typeof(Common.AccountText),
            ErrorMessageResourceName = "FieldIsRequired")]
        public required string Email { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Common.AccountText),
            ErrorMessageResourceName = "FieldIsRequired")]
        public required DateTime BirthDate { get; set; }

        public string AvatarUrl { get; set; }
        public IFormFile AvatarFile { get; set; }

        public Currency? PreferredCurrency { get; set; }
        public string? PreferredCurrencyCode { get; set; }
    }
}