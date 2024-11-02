using ControlSharp.Api.Config;
using ControlSharp.Api.Config.Model;
using ControlSharp.Api.Controllers.Model;
using ControlSharp.Controllers;
using ControlSharp.Services;
using Microsoft.AspNetCore.Mvc;
using DBUser = ControlSharp.Api.Config.Model.User;
using User = ControlSharp.Api.Controllers.Model.User;

namespace ControlSharp.Api.Controllers;

[ApiController]
[Route("api/0.1/[controller]")]
[Produces("application/json")]
public class LoginController  : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly ILogger<AssetController> _logger;
    
    public LoginController(DatabaseContext context, ILogger<AssetController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateNewAsset(User User, CancellationToken token)
    {
        IQueryable<DBUser> Login = _context.User.Where(user =>
            user.UserName.Equals(User.UserName) && user.Password.Equals(User.Password) && user.Active == true);

        if (Login.Count() == 1)
        {
            Session Session = new Session()
            {
                Id = Guid.NewGuid(),
                Type = SessionType.User,
                User = Login.First(),
                ApiKey = new ApiKey()
                {
                    Id = Guid.NewGuid(),
                    Active = true,
                    Created = DateTimeOffset.Now,
                    Role = AccessRole.Admin,
                    Key = SecretManager.CreateAdminToken()
                }
            };
            
            _context.Session.Add(Session);
            
            DBUser LoginUser = Login.First();
            LoginUser.LastOnline = DateTimeOffset.Now;
            _context.User.Update(LoginUser);
            
            await _context.SaveChangesAsync(token);
            
            _logger.LogInformation($"User logged in Successfully {User.UserName}");
            _logger.LogDebug($"Created new Session Entry {Session.Id}");

            UserSession LoginSession = new UserSession()
            {
                Username = User.UserName,
                Token = Session.ApiKey.Key
            };
            
            return Ok(LoginSession);
        }
        else
        {
            _logger.LogWarning($"Denied Access for Login User {User.UserName} from IP {HttpContext.Connection.RemoteIpAddress.ToString()}");
            return new NotFoundResult();
        }
    }
}