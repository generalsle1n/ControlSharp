namespace ControlSharp.Api.Extension;

public class MapApiFeatures
{
    internal bool Register { get; set; }
    internal bool Login { get; set; }
    internal bool Refresh { get; set; }
    internal bool ConfirmMail { get; set; }
    internal bool ResendConfirmMail { get; set; }
    internal bool ForgotPassword { get; set; }
    internal bool ResetPassword { get; set; }
    internal bool Manage { get; set; }
    internal bool TwoFactor { get; set; }
    internal bool Info { get; set; }
}