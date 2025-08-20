using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace asp_core_crud.Models
{
    public class ProductDetail
    {
        [Key]
        public int ProductId { get; set; }
        public string Description { get; set; }
        public string Manufacturer { get; set; }
        public Product Product { get; set; }
    }
}
