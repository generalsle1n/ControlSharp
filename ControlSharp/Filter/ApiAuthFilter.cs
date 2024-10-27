using ControlSharp.Config;
using ControlSharp.Config.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace ControlSharp.Filter;

public class ApiAuthFilter : IAsyncAuthorizationFilter
{
    private readonly DatabaseContext _context;
    private readonly ILogger<ApiAuthFilter> _logger;
    
    public ApiAuthFilter(DatabaseContext context, ILogger<ApiAuthFilter> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        bool LoginFailed = true;
        foreach (KeyValuePair<string, StringValues> SingleHeader in context.HttpContext.Request.Headers)
        {
            Guid AssetID;
            bool Result = Guid.TryParse(SingleHeader.Key, out AssetID);
            
            //Check if HeaderKey is an valid Guid
            if(Result)
            {
                IQueryable<Access> DatabaseResult = _context.Access.Where(item => item.Asset.Id == AssetID && item.Key.Key == SingleHeader.Value.First())
                    .Include(item => item.Asset);

                if (DatabaseResult.Count() == 1)
                {
                    LoginFailed = false;
                    
                    Asset Current = DatabaseResult.First().Asset;
                    
                    Current.LastOnline = DateTimeOffset.Now;
                    Current.Ip = context.HttpContext.Connection.RemoteIpAddress.ToString();
                    
                    _context.Asset.Update(Current);
                    await _context.SaveChangesAsync();
                    
                    break;
                }
            }
        }

        if (LoginFailed)
        {
            context.Result = new NotFoundResult();
            _logger.LogInformation($"Denied Access for IP {context.HttpContext.Connection.RemoteIpAddress} because no valid Credentials");
        }
    }
}