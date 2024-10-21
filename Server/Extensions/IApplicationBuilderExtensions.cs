namespace Remotely.Server.Extensions;

public static class IApplicationBuilderExtensions
{
    /// <summary>
    /// <para>
    ///     Maps Razor pages and SignalR hubs.  The remote control viewer page will be mapped
    ///     to path "/Viewer", the desktop hub to "/hubs/desktop", and viewer hub
    ///     to "/hubs/viewer".
    /// </para>
    /// <para>
    ///     Important: This must be called after "app.UseRouting()".
    /// </para>
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseRemoteControlServer(this IApplicationBuilder app)
    {
   

        return app;
    }
}
