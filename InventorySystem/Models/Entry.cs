using System;
using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
    public class Entry
    {
        [Key]
        public int EntryId { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public DateTime EntryDate { get; set; }
        public string? Notes { get; set; }
    }
}
