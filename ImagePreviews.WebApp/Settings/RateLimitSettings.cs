using System.ComponentModel.DataAnnotations;

namespace ImagePreviews.WebApp.Settings
{
	public class RateLimitSettings
	{
		[Required]
		public int PermitLimit { get; set; }

		[Required]
		public int WindowTimeInSeconds { get; set; }

		[Required]
		public int SegmentsPerWindow { get; set; }

		public int QueueLimit { get; set; } = 0;
	}
}