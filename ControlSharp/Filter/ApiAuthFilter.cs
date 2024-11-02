using ControlSharp.Api.Config;
using ControlSharp.Api.Config.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace ControlSharp.Api.Filter;

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
                IQueryable<Session> DatabaseResult = _context.Session.Where(item =>
                        item.Asset.Id == AssetID && item.ApiKey.Key == SingleHeader.Value.First() &&
                        item.ApiKey.Active == true)
                    .Include(item => item.Asset)
                    .Include(item => item.ApiKey);

                if (DatabaseResult.Count() == 1)
                {
                    LoginFailed = false;
                    
                    Session Current = DatabaseResult.First();
                    
                    Current.Asset.LastOnline = DateTimeOffset.Now;
                    Current.Asset.Ip = context.HttpContext.Connection.RemoteIpAddress.ToString();
                    
                    _context.Asset.Update(Current.Asset);
                    await _context.SaveChangesAsync();
                    
                    context.HttpContext.Items.Add("ApiKey", Current.ApiKey);
                    
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