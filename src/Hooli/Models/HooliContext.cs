using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Framework.OptionsModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hooli.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string RelationshipStatus { get; set; }
        public byte[] ProfilePicture { get; set; }
        public virtual List<Post> Posts { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public virtual List<Event> Events { get; set; }
        public virtual List<Group> Groups { get; set; }
        public virtual List<ApplicationUser> Following { get; set; }
        public virtual List<ApplicationUser> Followers { get; set; }
    }

    public class HooliContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ApplicationUser> User { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<FollowRelation> FollowRelation { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Post>().Key(p => p.PostId);
            builder.Entity<Comment>().Key(p => p.CommentId);
            builder.Entity<Event>().Key(e => e.EventId);
            builder.Entity<Group>().Key(g => g.GroupId);
            builder.Entity<FollowRelation>().Key(f => new { f.FollowerId, f.FollowingId });
            base.OnModelCreating(builder);
        }
    }
}
