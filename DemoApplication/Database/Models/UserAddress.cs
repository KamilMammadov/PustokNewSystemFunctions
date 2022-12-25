using DemoApplication.Database.Models.Common;

namespace DemoApplication.Database.Models
{
	public class UserAddress : BaseEntity<int>
	{
		public Guid UserId { get; set; }
		public User User { get; set; }
		public string Address { get; set; }
		public string PhoneNumber { get; set; }
	}
}
