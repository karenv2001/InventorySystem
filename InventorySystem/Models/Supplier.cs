using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string? Contact { get; set; }
    }
}
