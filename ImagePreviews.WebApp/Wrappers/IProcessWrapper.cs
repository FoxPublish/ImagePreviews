using System.Diagnostics;

namespace ImagePreviews.WebApp.Wrappers
{
    public interface IProcessWrapper
    {
        bool Run(ProcessStartInfo startInfo, int timeout);
    }
}
