using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Remotely.Server.Services;
using System;

namespace Remotely.Server.Data;

public interface IAppDbFactory
{
    AppDb GetContext();
}

public class AppDbFactory(IServiceProvider _services) : IAppDbFactory
{
    public AppDb GetContext()
    {
        return _services.GetRequiredService<AppDb>();
    }
}
