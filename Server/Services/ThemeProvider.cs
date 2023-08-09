using Microsoft.Identity.Client;
using Remotely.Shared.Enums;
using System.Threading.Tasks;

namespace Remotely.Server.Services;

public interface IThemeProvider
{
    Task<Theme> GetEffectiveTheme();
}

public class ThemeProvider : IThemeProvider
{
    private readonly IAuthService _authService;
    private readonly IApplicationConfig _appConfig;

    public ThemeProvider(IAuthService authService, IApplicationConfig appConfig)
    {
        _authService = authService;
        _appConfig = appConfig;
    }

    public async Task<Theme> GetEffectiveTheme()
    {
        if (await _authService.IsAuthenticated())
        {
            var userResult = await _authService.GetUser();
            if (userResult.IsSuccess)
            {
                return userResult.Value.UserOptions?.Theme ?? _appConfig.Theme;
            }
        }

        return _appConfig.Theme;
    }
}
