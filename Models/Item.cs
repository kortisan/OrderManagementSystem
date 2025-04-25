using System.ComponentModel.DataAnnotations;

namespace OrderManagementSystem.Models
{
    public class Item
    {
        [Key]
        public long SO_ITEM_ID { get; set; }

        [Required]
        public string ITEM_NAME { get; set; } = string.Empty;

        [Required]
        public int QUANTITY { get; set; }

        [Required]
        public double PRICE { get; set; }

        [Required]
        public long SO_ORDER_ID { get; set; }

        public Order? Order { get; set; }
    }
}
