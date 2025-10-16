using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Spotted.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    exam_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => x.exam_id);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    profile_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    display_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.profile_id);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    question_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    question_text = table.Column<string>(type: "text", nullable: false),
                    options = table.Column<string[]>(type: "jsonb", nullable: false),
                    correct_index = table.Column<int>(type: "integer", nullable: false),
                    exam_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.question_id);
                    table.ForeignKey(
                        name: "FK_Questions_Exams_exam_id",
                        column: x => x.exam_id,
                        principalTable: "Exams",
                        principalColumn: "exam_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    topic_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    exam_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.topic_id);
                    table.ForeignKey(
                        name: "FK_Topics_Exams_exam_id",
                        column: x => x.exam_id,
                        principalTable: "Exams",
                        principalColumn: "exam_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    profile_id = table.Column<int>(type: "integer", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_Users_Profiles_profile_id",
                        column: x => x.profile_id,
                        principalTable: "Profiles",
                        principalColumn: "profile_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserExams",
                columns: table => new
                {
                    user_exam_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    exam_id = table.Column<int>(type: "integer", nullable: false),
                    passed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExams", x => x.user_exam_id);
                    table.ForeignKey(
                        name: "FK_UserExams_Exams_exam_id",
                        column: x => x.exam_id,
                        principalTable: "Exams",
                        principalColumn: "exam_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserExams_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Exams",
                columns: new[] { "exam_id", "description", "title" },
                values: new object[,]
                {
                    { 1, "Foundational knowledge of Azure concepts, services, cloud concepts, security, privacy, pricing, and support.", "AZ-900" },
                    { 2, "Developing solutions for Microsoft Azure.", "AZ-204" }
                });

            migrationBuilder.InsertData(
                table: "Profiles",
                columns: new[] { "profile_id", "display_name" },
                values: new object[,]
                {
                    { 1, "Seth Behar" },
                    { 2, "Mike Scared" }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "question_id", "correct_index", "exam_id", "options", "question_text" },
                values: new object[,]
                {
                    { 1, 1, 1, new[] { "Azure SQL Database", "Azure Blob Storage", "Azure Table Storage", "Azure Queue Storage" }, "Which Azure service should you use to store unstructured data such as images and videos?" },
                    { 2, 1, 1, new[] { "Azure App Service", "Azure Virtual Machines", "Azure Kubernetes Service", "Azure Functions" }, "Which Azure service allows you to run virtualized Windows or Linux servers in the cloud?" },
                    { 3, 1, 1, new[] { "They provide faster internet connections.", "They protect applications and data from datacenter failures.", "They reduce storage costs for data.", "They automatically scale applications based on demand." }, "What is the main benefit of using Azure Availability Zones?" },
                    { 4, 1, 1, new[] { "Reserved Instances", "Pay-as-you-go", "Enterprise Agreement", "Free Tier" }, "Which pricing model allows you to pay only for the exact amount of resources you use?" },
                    { 5, 0, 1, new[] { "Azure Service Health", "Azure Monitor", "Azure Advisor", "Azure Security Center" }, "Which Azure tool allows you to view the status of all Azure services globally?" }
                });

            migrationBuilder.InsertData(
                table: "Topics",
                columns: new[] { "topic_id", "description", "exam_id", "name" },
                values: new object[,]
                {
                    { 1, "Understand cloud concepts (15-20%)", 1, "Cloud" },
                    { 2, "Understand core Azure services (30-35%)", 1, "AI" },
                    { 3, "Understand security, privacy, compliance, and trust (25-30%)", 1, "Security" },
                    { 4, "Understand Azure pricing and support (20-25%)", 1, "Data Governance" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "user_id", "email", "profile_id" },
                values: new object[,]
                {
                    { 1, "sethbehar@gmail.com", 1 },
                    { 2, "mike@scaredofthedark.com", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_exam_id",
                table: "Questions",
                column: "exam_id");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_exam_id",
                table: "Topics",
                column: "exam_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserExams_exam_id",
                table: "UserExams",
                column: "exam_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserExams_user_id",
                table: "UserExams",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_profile_id",
                table: "Users",
                column: "profile_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropTable(
                name: "UserExams");

            migrationBuilder.DropTable(
                name: "Exams");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Profiles");
        }
    }
}
