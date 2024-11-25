using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Zira.Data;

public class EntityContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public EntityContext(DbContextOptions<EntityContext> options)
        : base(options)
    {
    }
}