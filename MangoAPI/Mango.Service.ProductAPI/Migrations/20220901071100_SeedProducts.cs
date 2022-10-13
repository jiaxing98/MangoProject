using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mango.Service.ProductAPI.Migrations
{
    public partial class SeedProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryName", "Description", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Apptizer", "Praesent scelerisque, mi sed ultrices condimentum", "https://upload.wikimedia.org/wikipedia/commons/c/cb/Samosachutney.jpg", "Samosa", 16.0 },
                    { 2, "Apptizer", "Praesent scelerisque, mi sed ultrices condimentum", "https://en.wikipedia.org/wiki/Paneer_tikka#/media/File:Paneer_Tikka_Masala.jpg", "Paneer Tikka", 13.99 },
                    { 3, "Dessert", "Praesent scelerisque, mi sed ultrices condimentum", "https://en.wikipedia.org/wiki/Apple_pie#/media/File:Apple_pie_14.jpg", "Apple Pie", 10.99 },
                    { 4, "Entree", "Praesent scelerisque, mi sed ultrices condimentum", "https://en.wikipedia.org/wiki/Pav_bhaji#/media/File:Pav_Bhaji.jpg", "Pav Bhaji", 15.0 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4);
        }
    }
}
