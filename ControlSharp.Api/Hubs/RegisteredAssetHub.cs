using ControlSharp.Api.Hubs.Interfaces;
using ControlSharp.Database.Identity.Model;
using ControlSharp.Database.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace ControlSharp.Api.Hubs;

[Authorize]
public class RegisteredAssetHub : Hub<IRegisteredAssetClient>
{
    private readonly ILogger<RegisteredAssetHub> _logger;
    private readonly IConfiguration _configuration;
    private DatabaseContext _context;
    private SignInManager<User> _signInManager;

    public RegisteredAssetHub(ILogger<RegisteredAssetHub> logger, IConfiguration configuration, DatabaseContext context, SignInManager<User> signInManager)
    {
        _logger = logger;
        _configuration = configuration;
        _context = context;
        _signInManager = signInManager;
    }
}