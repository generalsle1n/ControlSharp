namespace ControlSharp.Ui.Models;

public class ApiIdentity
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public bool Success { get; set; }
}