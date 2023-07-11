namespace ProjectReview.Common
{
	public interface ICurrentUser
	{
		long UserId { get; set; }
		string UserName { get; set; }	
		string FullName { get; set; }
		string Email { get; set; }
		string Action { get; set; }
		string Detail { get; set; }
		string Message { get; set; }
	}

	public class CurrentUser : ICurrentUser
	{
		public long UserId { get; set; }
		public string UserName { get; set; }
		public string FullName { get; set; }
		public string Email { get; set; }
		public string Action { get; set; }
		public string Message { get; set; }
		public string Detail { get; set; }
	}
}
