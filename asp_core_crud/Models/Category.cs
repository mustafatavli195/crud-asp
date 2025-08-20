using System.ComponentModel.DataAnnotations;

namespace asp_core_crud.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<Product> Products { get; set; }
    }
}
