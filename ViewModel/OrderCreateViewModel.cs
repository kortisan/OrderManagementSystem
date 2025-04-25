using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OrderManagementSystem.ViewModels
{
    public class OrderCreateViewModel
    {
        [Required]
        public string ORDER_NO { get; set; } = string.Empty;

        [Required]
        public DateTime ORDER_DATE { get; set; }

        [Required]
        public int COM_CUSTOMER_ID { get; set; }

        public List<ItemViewModel> Items { get; set; } = new();

        [NotMapped]
        public string? ItemsJson { get; set; }

        public List<SelectListItem> Customers { get; set; } = new();
    }

    public class ItemViewModel
    {
        [Required]
        public string ITEM_NAME { get; set; } = string.Empty;

        [Required]
        public int QUANTITY { get; set; }

        [Required]
        public double PRICE { get; set; }
    }
}
