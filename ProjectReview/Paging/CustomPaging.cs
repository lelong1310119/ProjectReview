namespace ProjectReview.Paging
{
	public class CustomPaging<T>
	{
		public int TotalPage { get; set; }
		public int PageSize { get; set; }
		public List<T> Data { get; set; }	
	}
}
