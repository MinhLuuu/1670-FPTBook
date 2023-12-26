using Microsoft.AspNetCore.Mvc.Rendering;

namespace minh.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Book>? Book { get; set; }
    }
}
