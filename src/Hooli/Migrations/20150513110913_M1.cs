using System.Collections.Generic;
using Microsoft.Data.Entity.Relational.Migrations;
using Microsoft.Data.Entity.Relational.Migrations.Builders;
using Microsoft.Data.Entity.Relational.Migrations.Operations;

namespace Hooli.Migrations
{
    public partial class M1 : Migration
    {
        public override void Up(MigrationBuilder migration)
        {
            migration.CreateTable(
                name: "VoteRelation",
                columns: table => new
                {
                    PostId = table.Column(type: "nvarchar(450)", nullable: true),
                    PostPostId = table.Column(type: "int", nullable: true),
                    UserId = table.Column(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteRelation", x => new { x.UserId, x.PostId });
                    table.ForeignKey(
                        name: "FK_VoteRelation_Post_PostPostId",
                        columns: x => x.PostPostId,
                        referencedTable: "Post",
                        referencedColumn: "PostId");
                    table.ForeignKey(
                        name: "FK_VoteRelation_AspNetUsers_UserId",
                        columns: x => x.UserId,
                        referencedTable: "AspNetUsers",
                        referencedColumn: "Id");
                });
        }
        
        public override void Down(MigrationBuilder migration)
        {
            migration.DropTable("VoteRelation");
        }
    }
}
