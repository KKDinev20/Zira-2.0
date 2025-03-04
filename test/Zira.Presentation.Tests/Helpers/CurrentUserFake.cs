using System;
using Microsoft.Extensions.Configuration;
using Zira.Services.Identity.Contracts;

namespace Zira.Presentation.Tests;

public class CurrentUserFake : ICurrentUser
{
    private readonly IConfiguration configuration;
    public CurrentUserFake(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public Guid? UserId => this.configuration.GetValue<Guid>("Id");
    
    public bool Exists => this.UserId.HasValue;
}