using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderManagementSystem.Models
{
    public class Customer
    {
        [Key]
        public int COM_CUSTOMER_ID { get; set; }

        [Required]
        public string CUSTOMER_NAME { get; set; } = string.Empty;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
