using System.Collections.Generic;
using Microsoft.Data.Entity.Relational.Migrations;
using Microsoft.Data.Entity.Relational.Migrations.Builders;
using Microsoft.Data.Entity.Relational.Migrations.Operations;

namespace Hooli.Migrations
{
    public partial class M : Migration
    {
        public override void Up(MigrationBuilder migration)
        {
            migration.CreateTable(
                name: "FollowRelation",
                columns: table => new
                {
                    FollowerId = table.Column(type: "nvarchar(450)", nullable: true),
                    FollowingId = table.Column(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowRelation", x => new { x.FollowerId, x.FollowingId });
                    table.ForeignKey(
                        name: "FK_FollowRelation_AspNetUsers_FollowerId",
                        columns: x => x.FollowerId,
                        referencedTable: "AspNetUsers",
                        referencedColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FollowRelation_AspNetUsers_FollowingId",
                        columns: x => x.FollowingId,
                        referencedTable: "AspNetUsers",
                        referencedColumn: "Id");
                });
            
        }
        
        public override void Down(MigrationBuilder migration)
        {
            migration.DropTable("FollowRelation");
        }
    }
}
