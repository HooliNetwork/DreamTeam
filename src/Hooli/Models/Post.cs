using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hooli.Models
{
    public class Post
    {
        private readonly HooliContext _dbContext;
        [Key]
        public int PostId { get; set; }

        [Required]
        [StringLength(160, MinimumLength = 2)]
        public string Title { get; set; }

        public string Text { get; set; }

        [Display(Name = "Image URL")]
        [StringLength(1024)]
        public string ImgUrl { get; set; }

        public int UpVotes { get; set; }
        public int DownVotes { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }

        public virtual Group Group { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public Post(HooliContext dbContext)
        {
            _dbContext = dbContext;
            this.DateCreated = DateTime.UtcNow;
        }

        public async Task<List<Comment>> GetPostComments()
        {
            return await _dbContext.Comment
                Where(cart => cart.CartId == ShoppingCartId).
                Include(c => c.Album).
                ToListAsync();
        }
    }
}
