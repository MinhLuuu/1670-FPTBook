using System.ComponentModel.DataAnnotations.Schema;

namespace minh.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int GenreId { get; set; }
        public virtual Genre? Genre { get; set; }
        public virtual ICollection<Cart>? Cart { get; set; }
        public string? ProfilePicture { get; set; }
        [NotMapped]
        public IFormFile? ProfileImage { get; set; }
    }
}
