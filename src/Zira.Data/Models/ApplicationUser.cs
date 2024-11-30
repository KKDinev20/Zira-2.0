using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Zira.Data.Models;

namespace Zira.Data;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}