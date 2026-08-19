using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddtableinDbset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentModel_User_ManagerId",
                table: "DepartmentModel");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseApprovalModel_ExpenseModel_ExpenseId",
                table: "ExpenseApprovalModel");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseModel_CategoryModel_CategoryId",
                table: "ExpenseModel");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseModel_User_UserId",
                table: "ExpenseModel");

            migrationBuilder.DropForeignKey(
                name: "FK_User_DepartmentModel_DepartmentId",
                table: "User");

            migrationBuilder.DropTable(
                name: "CategoryModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExpenseModel",
                table: "ExpenseModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExpenseApprovalModel",
                table: "ExpenseApprovalModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DepartmentModel",
                table: "DepartmentModel");

            migrationBuilder.RenameTable(
                name: "ExpenseModel",
                newName: "Expense");

            migrationBuilder.RenameTable(
                name: "ExpenseApprovalModel",
                newName: "ExpenseApproval");

            migrationBuilder.RenameTable(
                name: "DepartmentModel",
                newName: "Department");

            migrationBuilder.RenameIndex(
                name: "IX_ExpenseModel_UserId",
                table: "Expense",
                newName: "IX_Expense_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ExpenseModel_CategoryId",
                table: "Expense",
                newName: "IX_Expense_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ExpenseApprovalModel_ExpenseId",
                table: "ExpenseApproval",
                newName: "IX_ExpenseApproval_ExpenseId");

            migrationBuilder.RenameIndex(
                name: "IX_DepartmentModel_ManagerId",
                table: "Department",
                newName: "IX_Department_ManagerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Expense",
                table: "Expense",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExpenseApproval",
                table: "ExpenseApproval",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Department",
                table: "Department",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsValid = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Department_User_ManagerId",
                table: "Department",
                column: "ManagerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Expense_Category_CategoryId",
                table: "Expense",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Expense_User_UserId",
                table: "Expense",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseApproval_Expense_ExpenseId",
                table: "ExpenseApproval",
                column: "ExpenseId",
                principalTable: "Expense",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Department_DepartmentId",
                table: "User",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Department_User_ManagerId",
                table: "Department");

            migrationBuilder.DropForeignKey(
                name: "FK_Expense_Category_CategoryId",
                table: "Expense");

            migrationBuilder.DropForeignKey(
                name: "FK_Expense_User_UserId",
                table: "Expense");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseApproval_Expense_ExpenseId",
                table: "ExpenseApproval");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Department_DepartmentId",
                table: "User");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExpenseApproval",
                table: "ExpenseApproval");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Expense",
                table: "Expense");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Department",
                table: "Department");

            migrationBuilder.RenameTable(
                name: "ExpenseApproval",
                newName: "ExpenseApprovalModel");

            migrationBuilder.RenameTable(
                name: "Expense",
                newName: "ExpenseModel");

            migrationBuilder.RenameTable(
                name: "Department",
                newName: "DepartmentModel");

            migrationBuilder.RenameIndex(
                name: "IX_ExpenseApproval_ExpenseId",
                table: "ExpenseApprovalModel",
                newName: "IX_ExpenseApprovalModel_ExpenseId");

            migrationBuilder.RenameIndex(
                name: "IX_Expense_UserId",
                table: "ExpenseModel",
                newName: "IX_ExpenseModel_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Expense_CategoryId",
                table: "ExpenseModel",
                newName: "IX_ExpenseModel_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Department_ManagerId",
                table: "DepartmentModel",
                newName: "IX_DepartmentModel_ManagerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExpenseApprovalModel",
                table: "ExpenseApprovalModel",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExpenseModel",
                table: "ExpenseModel",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DepartmentModel",
                table: "DepartmentModel",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CategoryModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsValid = table.Column<bool>(type: "boolean", nullable: false),
                    Title = table.Column<string>(type: "jsonb", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryModel", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentModel_User_ManagerId",
                table: "DepartmentModel",
                column: "ManagerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseApprovalModel_ExpenseModel_ExpenseId",
                table: "ExpenseApprovalModel",
                column: "ExpenseId",
                principalTable: "ExpenseModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseModel_CategoryModel_CategoryId",
                table: "ExpenseModel",
                column: "CategoryId",
                principalTable: "CategoryModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseModel_User_UserId",
                table: "ExpenseModel",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_DepartmentModel_DepartmentId",
                table: "User",
                column: "DepartmentId",
                principalTable: "DepartmentModel",
                principalColumn: "Id");
        }
    }
}
