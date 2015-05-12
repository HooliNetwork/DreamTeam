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
        [Display(Name = "ImageURI")]
        [StringLength(1024)]
        public string ProfilePicture { get; set; }
        [ForeignKey("UserID")]
        public virtual List<Post> Posts { get; set; }
        public virtual List<Event> Events { get; set; }

        [ForeignKey("UserID")]
        public virtual List<GroupMember> GroupsMember { get; set; }

        public virtual List<FollowRelation> Following { get; set; }
        public virtual List<FollowRelation> Followers { get; set; }
    

        public ApplicationUser()
        {
            Following = new List<FollowRelation>();
            Followers = new List<FollowRelation>();
            GroupsMember = new List<GroupMember>();
        }
    }


    public class HooliContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<FollowRelation> FollowRelations { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Post>()
              .Reference(c => c.User)
              .InverseCollection(c => c.Posts)
              .ForeignKey(c => c.UserId);
            builder.Entity<Event>().Key(e => e.EventId);
            builder.Entity<Group>().Key(g => g.GroupId);
            builder.Entity<FollowRelation>()
                .Key(k => new { k.FollowerId, k.FollowingId });
            builder.Entity<FollowRelation>()
                .Reference(c => c.Follower)
                .InverseCollection(c => c.Followers)
                .ForeignKey(c => c.FollowerId);
            builder.Entity<FollowRelation>()
                .Reference(c => c.Following)
                .InverseCollection(c => c.Following)
                .ForeignKey(c => c.FollowingId);

            builder.Entity<GroupMember>()
                .Key(k => new { k.UserId, k.GroupId });
            builder.Entity<GroupMember>()
                .Reference(c => c.Member)
                .InverseCollection(g => g.GroupsMember)
                .ForeignKey(c => c.UserId);
            base.OnModelCreating(builder);
        }
    }
}