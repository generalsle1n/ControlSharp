
namespace ControlSharp.Ui.Models
{
    public class UserProperty
    {
        internal List<KeyValuePair<string, string>> Claims { get; set; } = new List<KeyValuePair<string, string>>();
        internal List<KeyValuePair<string, string>> Properties { get; set; } = new List<KeyValuePair<string, string>>();

    }
}
