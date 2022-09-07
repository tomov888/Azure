namespace AzureFunctions.Shared.Middleware.Models
{
	public record UserSession
	{
		public int UserId { get; set; }
		public List<Permission> Permissions { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
	}
}