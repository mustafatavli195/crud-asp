using System.ComponentModel.DataAnnotations;

namespace asp_core_crud.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        public string Name { get; set; }
        [Range(0.01,100000, ErrorMessage ="Fiyat pozitif olmalıdır")]
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public List<Tag>? Tags { get; set; }
        public ProductDetail? ProductDetail { get; set; }
    }
}
