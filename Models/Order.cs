using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderManagementSystem.Models
{
    public class Order
    {
        [Key]
        public long SO_ORDER_ID { get; set; }

        [Required]
        public string ORDER_NO { get; set; } = string.Empty;

        [Required]
        public DateTime ORDER_DATE { get; set; }

        [Required]
        public int COM_CUSTOMER_ID { get; set; }

        public Customer? Customer { get; set; }
        public List<Item> Items { get; set; } = new();
    }
}
