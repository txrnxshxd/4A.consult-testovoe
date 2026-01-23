using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Models
{
    public class BookDetails
    {
        public int Id { get; set; }
        [Display(Name = "Название")]
        public string? Name { get; set; }
        [Display(Name = "Автор")]
        public string? Author { get; set; }
        [Display(Name = "Категория")]
        public string? Category { get; set; }
        [Display(Name = "Год издания")]
        public int? Year { get; set; }
        [Display(Name = "Оглавление")]
        [Column(TypeName = "xml")]
        public string? Contents { get; set; }
    }
}
