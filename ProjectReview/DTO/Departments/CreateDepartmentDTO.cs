using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace ProjectReview.DTO.Departments
{
	public class CreateDepartmentDTO
	{

		public string Name { get; set; }

		public string? Address { get; set; }

		public string? Phone { get; set; }
	}
}
