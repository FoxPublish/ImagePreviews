using System.ComponentModel.DataAnnotations;

namespace ImagePreviews.WebApp.Settings
{
    public class ImagePathSettings
    {
        [Required]
        public string DataShareBasePath { get; set; } = String.Empty;

        [Required]
        public string TempPath { get; set; } = String.Empty;
    }
}
