using System.ComponentModel.DataAnnotations;

namespace ImagePreviews.WebApp.Settings
{
    public class ProjectSettings
    {
        [Required, Url]
        public string Url { get; set; } = String.Empty;
    }
}
