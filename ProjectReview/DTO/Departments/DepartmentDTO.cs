using ProjectReview.Models.Entities;

namespace ProjectReview.DTO.Departments
{
	public class DepartmentDTO
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public string Phone { get; set; }
		public int Status { get; set; }
		public DateTime CreateDate { get; set; }
		public Boolean isDelete { get; set; }
	}
}
