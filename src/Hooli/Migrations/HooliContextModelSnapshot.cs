using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Builders;
using Microsoft.Data.Entity.Relational.Migrations.Infrastructure;
using Hooli.Models;

namespace Hooli.Migrations
{
    [ContextType(typeof(HooliContext))]
    partial class HooliContextModelSnapshot : ModelSnapshot
    {
        public override IModel Model
        {
            get
            {
                var builder = new BasicModelBuilder()
                    .Annotation("SqlServer:ValueGeneration", "Identity");
                
                builder.Entity("Hooli.Models.ApplicationUser", b =>
                    {
                        b.Property<int>("AccessFailedCount")
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<string>("ConcurrencyStamp")
                            .ConcurrencyToken()
                            .Annotation("OriginalValueIndex", 1);
                        b.Property<DateTime>("DateOfBirth")
                            .Annotation("OriginalValueIndex", 2);
                        b.Property<string>("Email")
                            .Annotation("OriginalValueIndex", 3);
                        b.Property<bool>("EmailConfirmed")
                            .Annotation("OriginalValueIndex", 4);
                        b.Property<string>("FirstName")
                            .Annotation("OriginalValueIndex", 5);
                        b.Property<string>("Id")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 6);
                        b.Property<string>("LastName")
                            .Annotation("OriginalValueIndex", 7);
                        b.Property<bool>("LockoutEnabled")
                            .Annotation("OriginalValueIndex", 8);
                        b.Property<DateTimeOffset?>("LockoutEnd")
                            .Annotation("OriginalValueIndex", 9);
                        b.Property<string>("NormalizedEmail")
                            .Annotation("OriginalValueIndex", 10);
                        b.Property<string>("NormalizedUserName")
                            .Annotation("OriginalValueIndex", 11);
                        b.Property<string>("PasswordHash")
                            .Annotation("OriginalValueIndex", 12);
                        b.Property<string>("PhoneNumber")
                            .Annotation("OriginalValueIndex", 13);
                        b.Property<bool>("PhoneNumberConfirmed")
                            .Annotation("OriginalValueIndex", 14);
                        b.Property<byte[]>("ProfilePicture")
                            .Annotation("OriginalValueIndex", 15);
                        b.Property<string>("RelationshipStatus")
                            .Annotation("OriginalValueIndex", 16);
                        b.Property<string>("SecurityStamp")
                            .Annotation("OriginalValueIndex", 17);
                        b.Property<bool>("TwoFactorEnabled")
                            .Annotation("OriginalValueIndex", 18);
                        b.Property<string>("UserName")
                            .Annotation("OriginalValueIndex", 19);
                        b.Key("Id");
                        b.Annotation("Relational:TableName", "AspNetUsers");
                    });
                
                builder.Entity("Hooli.Models.Event", b =>
                    {
                        b.Property<DateTime>("DateCreated")
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<string>("Description")
                            .Annotation("OriginalValueIndex", 1);
                        b.Property<DateTime>("EndTime")
                            .Annotation("OriginalValueIndex", 2);
                        b.Property<int>("EventId")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 3)
                            .Annotation("SqlServer:ValueGeneration", "Default");
                        b.Property<string>("EventName")
                            .Annotation("OriginalValueIndex", 4);
                        b.Property<string>("ImgUrl")
                            .Annotation("OriginalValueIndex", 5);
                        b.Property<string>("Location")
                            .Annotation("OriginalValueIndex", 6);
                        b.Property<bool>("Private")
                            .Annotation("OriginalValueIndex", 7);
                        b.Property<DateTime>("StartTime")
                            .Annotation("OriginalValueIndex", 8);
                        b.Key("EventId");
                    });
                
                builder.Entity("Hooli.Models.File", b =>
                    {
                        b.Property<byte[]>("Content")
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<string>("ContentType")
                            .Annotation("OriginalValueIndex", 1);
                        b.Property<int>("FileId")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 2)
                            .Annotation("SqlServer:ValueGeneration", "Default");
                        b.Property<string>("FileName")
                            .Annotation("OriginalValueIndex", 3);
                        b.Property<int>("FileType")
                            .Annotation("OriginalValueIndex", 4);
                        b.Key("FileId");
                    });
                
                builder.Entity("Hooli.Models.FollowRelation", b =>
                    {
                        b.Property<string>("FollowerId")
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<string>("FollowingId")
                            .Annotation("OriginalValueIndex", 1);
                        b.Key("FollowerId", "FollowingId");
                    });
                
                builder.Entity("Hooli.Models.Group", b =>
                    {
                        b.Property<DateTime>("DateCreated")
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<string>("Description")
                            .Annotation("OriginalValueIndex", 1);
                        b.Property<int>("GroupId")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 2)
                            .Annotation("SqlServer:ValueGeneration", "Default");
                        b.Property<string>("GroupName")
                            .Annotation("OriginalValueIndex", 3);
                        b.Property<byte[]>("GroupPicture")
                            .Annotation("OriginalValueIndex", 4);
                        b.Property<bool>("Private")
                            .Annotation("OriginalValueIndex", 5);
                        b.Key("GroupId");
                    });
                
                builder.Entity("Hooli.Models.GroupMember", b =>
                    {
                        b.Property<int>("GroupId")
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<string>("UserId")
                            .Annotation("OriginalValueIndex", 1);
                        b.Property<bool>("banned")
                            .Annotation("OriginalValueIndex", 2);
                        b.Key("UserId", "GroupId");
                    });
                
                builder.Entity("Hooli.Models.Post", b =>
                    {
                        b.Property<DateTime>("DateCreated")
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<int?>("GroupGroupId")
                            .Annotation("OriginalValueIndex", 1)
                            .Annotation("ShadowIndex", 0);
                        b.Property<string>("ImgUrl")
                            .Annotation("OriginalValueIndex", 2);
                        b.Property<int?>("ParentPostId")
                            .Annotation("OriginalValueIndex", 3);
                        b.Property<int>("Points")
                            .Annotation("OriginalValueIndex", 4);
                        b.Property<int>("PostId")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 5)
                            .Annotation("SqlServer:ValueGeneration", "Default");
                        b.Property<string>("Text")
                            .Annotation("OriginalValueIndex", 6);
                        b.Property<string>("Title")
                            .Annotation("OriginalValueIndex", 7);
                        b.Property<string>("UserId")
                            .Annotation("OriginalValueIndex", 8);
                        b.Key("PostId");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRole", b =>
                    {
                        b.Property<string>("ConcurrencyStamp")
                            .ConcurrencyToken()
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<string>("Id")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 1);
                        b.Property<string>("Name")
                            .Annotation("OriginalValueIndex", 2);
                        b.Property<string>("NormalizedName")
                            .Annotation("OriginalValueIndex", 3);
                        b.Key("Id");
                        b.Annotation("Relational:TableName", "AspNetRoles");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", b =>
                    {
                        b.Property<string>("ClaimType")
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<string>("ClaimValue")
                            .Annotation("OriginalValueIndex", 1);
                        b.Property<int>("Id")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 2)
                            .Annotation("SqlServer:ValueGeneration", "Default");
                        b.Property<string>("RoleId")
                            .Annotation("OriginalValueIndex", 3);
                        b.Key("Id");
                        b.Annotation("Relational:TableName", "AspNetRoleClaims");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", b =>
                    {
                        b.Property<string>("ClaimType")
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<string>("ClaimValue")
                            .Annotation("OriginalValueIndex", 1);
                        b.Property<int>("Id")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 2)
                            .Annotation("SqlServer:ValueGeneration", "Default");
                        b.Property<string>("UserId")
                            .Annotation("OriginalValueIndex", 3);
                        b.Key("Id");
                        b.Annotation("Relational:TableName", "AspNetUserClaims");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", b =>
                    {
                        b.Property<string>("LoginProvider")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<string>("ProviderDisplayName")
                            .Annotation("OriginalValueIndex", 1);
                        b.Property<string>("ProviderKey")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 2);
                        b.Property<string>("UserId")
                            .Annotation("OriginalValueIndex", 3);
                        b.Key("LoginProvider", "ProviderKey");
                        b.Annotation("Relational:TableName", "AspNetUserLogins");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", b =>
                    {
                        b.Property<string>("RoleId")
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<string>("UserId")
                            .Annotation("OriginalValueIndex", 1);
                        b.Key("UserId", "RoleId");
                        b.Annotation("Relational:TableName", "AspNetUserRoles");
                    });
                
                builder.Entity("Hooli.Models.FollowRelation", b =>
                    {
                        b.ForeignKey("Hooli.Models.ApplicationUser", "FollowerId");
                        b.ForeignKey("Hooli.Models.ApplicationUser", "FollowingId");
                    });
                
                builder.Entity("Hooli.Models.GroupMember", b =>
                    {
                        b.ForeignKey("Hooli.Models.Group", "GroupId");
                        b.ForeignKey("Hooli.Models.ApplicationUser", "UserId");
                    });
                
                builder.Entity("Hooli.Models.Post", b =>
                    {
                        b.ForeignKey("Hooli.Models.Group", "GroupGroupId");
                        b.ForeignKey("Hooli.Models.Post", "ParentPostId");
                        b.ForeignKey("Hooli.Models.ApplicationUser", "UserId");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", b =>
                    {
                        b.ForeignKey("Microsoft.AspNet.Identity.EntityFramework.IdentityRole", "RoleId");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", b =>
                    {
                        b.ForeignKey("Hooli.Models.ApplicationUser", "UserId");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", b =>
                    {
                        b.ForeignKey("Hooli.Models.ApplicationUser", "UserId");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", b =>
                    {
                        b.ForeignKey("Microsoft.AspNet.Identity.EntityFramework.IdentityRole", "RoleId");
                        b.ForeignKey("Hooli.Models.ApplicationUser", "UserId");
                    });
                
                return builder.Model;
            }
        }
    }
}
