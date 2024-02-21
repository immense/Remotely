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
    private readonly IDataService _dataService;

    public ThemeProvider(IAuthService authService, IDataService dataService)
    {
        _authService = authService;
        _dataService = dataService;
    }

    public async Task<Theme> GetEffectiveTheme()
    {
        var settings = await _dataService.GetSettings();
        if (await _authService.IsAuthenticated())
        {
            var userResult = await _authService.GetUser();
            if (userResult.IsSuccess)
            {
                return userResult.Value.UserOptions?.Theme ?? settings.Theme;
            }
        }

        return settings.Theme;
    }
}
