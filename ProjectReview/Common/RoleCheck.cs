namespace ProjectReview.Common
{
	public static class RoleCheck
	{
		public static bool CheckRole(string role)
		{
			var httpContextAccessor = new HttpContextAccessor();
			var httpContext = httpContextAccessor.HttpContext;

			var currentUser = httpContext.RequestServices.GetService<ICurrentUser>();

			var userId = currentUser.UserId;
			var userName = currentUser.UserName;
			var fullName = currentUser.FullName;
			var email = currentUser.Email;
			var roles = currentUser.Roles;
			foreach(var item in roles)
			{
				if(item == role) return true;
			}
			return false;
		}
	}
}
