using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Remotely.Server.Hubs;
using Remotely.Server.Services;
using Remotely.Shared.Models;
using Remotely.Server.Auth;

namespace Remotely.Server.API;

[Route("api/[controller]")]
[ApiController]
public class SystemController : ControllerBase
{
    public IDataService DataService { get; }
    public IHubContext<AgentHub> AgentHubContext { get; }
    public IApplicationConfig AppConfig { get; }
    public SignInManager<RemotelyUser> SignInManager { get; }

    public SystemController(
      IDataService dataService,
      IHubContext<AgentHub> agentHub,
      IApplicationConfig appConfig,
      SignInManager<RemotelyUser> signInManager)
    {
        DataService = dataService;
        AgentHubContext = agentHub;
        AppConfig = appConfig;
        SignInManager = signInManager;
    }

    [HttpGet("agent-version")]
    [ServiceFilter(typeof(ApiAuthorizationFilter))]
    public IActionResult GetAgentVersion()
    {
        return Ok(AppConfig.AgentVersion);
    }
}