using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectReview.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreateUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentType",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreateUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Documnet",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreateUserId = table.Column<long>(type: "bigint", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    DateIssued = table.Column<DateTime>(type: "datetime", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentTypeId = table.Column<long>(type: "bigint", nullable: false),
                    Receiver = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Density = table.Column<int>(type: "int", nullable: false),
                    Urgency = table.Column<int>(type: "int", nullable: false),
                    NumberPaper = table.Column<int>(type: "int", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Signer = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Position = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAssign = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documnet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documnent_DocumentType",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Handler",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    JobId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Handler", x => new { x.UserId, x.JobId });
                });

            migrationBuilder.CreateTable(
                name: "Job",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HostId = table.Column<long>(type: "bigint", nullable: false),
                    InstructorId = table.Column<long>(type: "bigint", nullable: false),
                    CreateUserId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime", nullable: false),
                    Request = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Job_Document",
                        column: x => x.DocumentId,
                        principalTable: "Documnet",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JobProfile",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<long>(type: "bigint", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    NumberPaper = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreateUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProfileDocument",
                columns: table => new
                {
                    JobProfileId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileDocument", x => new { x.JobProfileId, x.DocumentId });
                    table.ForeignKey(
                        name: "FK_ProfileDocument_Document",
                        column: x => x.DocumentId,
                        principalTable: "Documnet",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProfileDocument_JobProfile",
                        column: x => x.JobProfileId,
                        principalTable: "JobProfile",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Opinion",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreateUserId = table.Column<long>(type: "bigint", nullable: false),
                    JobId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opinion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Opinion_Job",
                        column: x => x.JobId,
                        principalTable: "Job",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PermissionGroup",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreateUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Position",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreateUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Position", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Profile",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Symbol = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Expiry = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OrderBy = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreateUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rank",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreateUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rank", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Birthday = table.Column<DateTime>(type: "datetime", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PositionId = table.Column<long>(type: "bigint", nullable: false),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: false),
                    PermissionGroupId = table.Column<long>(type: "bigint", nullable: false),
                    RankId = table.Column<long>(type: "bigint", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreateUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_CreateUser",
                        column: x => x.CreateUserId,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Department",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_PermissionGroup",
                        column: x => x.PermissionGroupId,
                        principalTable: "PermissionGroup",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Position",
                        column: x => x.PositionId,
                        principalTable: "Position",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Rank",
                        column: x => x.RankId,
                        principalTable: "Rank",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Department_CreateUserId",
                table: "Department",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentType_CreateUserId",
                table: "DocumentType",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Documnet_CreateUserId",
                table: "Documnet",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Documnet_DocumentTypeId",
                table: "Documnet",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Handler_JobId",
                table: "Handler",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Job_CreateUserId",
                table: "Job",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Job_DocumentId",
                table: "Job",
                column: "DocumentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Job_HostId",
                table: "Job",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_Job_InstructorId",
                table: "Job",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_JobProfile_CreateUserId",
                table: "JobProfile",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobProfile_ProfileId",
                table: "JobProfile",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Opinion_CreateUserId",
                table: "Opinion",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Opinion_JobId",
                table: "Opinion",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionGroup_CreateUserId",
                table: "PermissionGroup",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Position_CreateUserId",
                table: "Position",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Profile_CreateUserId",
                table: "Profile",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileDocument_DocumentId",
                table: "ProfileDocument",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Rank_CreateUserId",
                table: "Rank",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_CreateUserId",
                table: "User",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_DepartmentId",
                table: "User",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_User_PermissionGroupId",
                table: "User",
                column: "PermissionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_User_PositionId",
                table: "User",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_User_RankId",
                table: "User",
                column: "RankId");

            migrationBuilder.AddForeignKey(
                name: "FK_Department_User",
                table: "Department",
                column: "CreateUserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentType_User",
                table: "DocumentType",
                column: "CreateUserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Documnent_User",
                table: "Documnet",
                column: "CreateUserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Handler_Job",
                table: "Handler",
                column: "JobId",
                principalTable: "Job",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Handler_User",
                table: "Handler",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Job_CreateUser",
                table: "Job",
                column: "CreateUserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Job_HostUser",
                table: "Job",
                column: "HostId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Job_InstructorUser",
                table: "Job",
                column: "InstructorId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobProfile_Profile",
                table: "JobProfile",
                column: "ProfileId",
                principalTable: "Profile",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobProfile_User",
                table: "JobProfile",
                column: "CreateUserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Opinion_User",
                table: "Opinion",
                column: "CreateUserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionGroup_User",
                table: "PermissionGroup",
                column: "CreateUserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Position_User",
                table: "Position",
                column: "CreateUserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Profile_User",
                table: "Profile",
                column: "CreateUserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rank_User",
                table: "Rank",
                column: "CreateUserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Department_User",
                table: "Department");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionGroup_User",
                table: "PermissionGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_Position_User",
                table: "Position");

            migrationBuilder.DropForeignKey(
                name: "FK_Rank_User",
                table: "Rank");

            migrationBuilder.DropTable(
                name: "Handler");

            migrationBuilder.DropTable(
                name: "Opinion");

            migrationBuilder.DropTable(
                name: "ProfileDocument");

            migrationBuilder.DropTable(
                name: "Job");

            migrationBuilder.DropTable(
                name: "JobProfile");

            migrationBuilder.DropTable(
                name: "Documnet");

            migrationBuilder.DropTable(
                name: "Profile");

            migrationBuilder.DropTable(
                name: "DocumentType");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "PermissionGroup");

            migrationBuilder.DropTable(
                name: "Position");

            migrationBuilder.DropTable(
                name: "Rank");
        }
    }
}
