using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeAppBackend.Migrations
{
    /// <inheritdoc />
    public partial class DeletedJoinTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.RenameTable(
            //    name: "Recipe",
            //    newName: "Recipes");

            migrationBuilder.RenameIndex(
                name: "IX_Recipe_UserId",
                table: "Recipes",
                newName: "IX_Recipes_UserId");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Recipe_RecipeId",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipe_Users_UserId",
                table: "Recipes");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeKeywords_Recipe_RecipeId",
                table: "RecipeKeywords");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Recipe_RecipeId",
                table: "Reviews");

            migrationBuilder.DropTable(
                name: "RecipeImages");

            migrationBuilder.DropTable(
                name: "RecipeIngredients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Recipe",
                table: "Recipes");

            

            migrationBuilder.AddColumn<int>(
                name: "RecipeId",
                table: "Ingredients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RecipeId",
                table: "Images",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Recipes",
                table: "Recipes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_RecipeId",
                table: "Ingredients",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_RecipeId",
                table: "Images",
                column: "RecipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Recipes_RecipeId",
                table: "Favorites",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Recipes_RecipeId",
                table: "Images",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_Recipes_RecipeId",
                table: "Ingredients",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeKeywords_Recipes_RecipeId",
                table: "RecipeKeywords",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Users_UserId",
                table: "Recipes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Recipes_RecipeId",
                table: "Reviews",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Recipes_RecipeId",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Recipes_RecipeId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_Recipes_RecipeId",
                table: "Ingredients");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeKeywords_Recipes_RecipeId",
                table: "RecipeKeywords");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Users_UserId",
                table: "Recipes");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Recipes_RecipeId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_RecipeId",
                table: "Ingredients");

            migrationBuilder.DropIndex(
                name: "IX_Images_RecipeId",
                table: "Images");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Recipes",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "RecipeId",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "RecipeId",
                table: "Images");

            migrationBuilder.RenameTable(
                name: "recipes",
                newName: "recipe");

            migrationBuilder.RenameIndex(
                name: "ix_recipes_userid",
                table: "recipe",
                newName: "ix_recipe_userid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Recipe",
                table: "Recipes",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "RecipeImages",
                columns: table => new
                {
                    RecipeId = table.Column<int>(type: "int", nullable: false),
                    ImageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeImages", x => new { x.RecipeId, x.ImageId });
                    table.ForeignKey(
                        name: "FK_RecipeImages_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecipeImages_Recipe_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecipeIngredients",
                columns: table => new
                {
                    RecipeId = table.Column<int>(type: "int", nullable: false),
                    IngredientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeIngredients", x => new { x.RecipeId, x.IngredientId });
                    table.ForeignKey(
                        name: "FK_RecipeIngredients_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecipeIngredients_Recipe_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeImages_ImageId",
                table: "RecipeImages",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_IngredientId",
                table: "RecipeIngredients",
                column: "IngredientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Recipe_RecipeId",
                table: "Favorites",
                column: "RecipeId",
                principalTable: "Recipe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipe_Users_UserId",
                table: "Recipe",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeKeywords_Recipe_RecipeId",
                table: "RecipeKeywords",
                column: "RecipeId",
                principalTable: "Recipe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Recipe_RecipeId",
                table: "Reviews",
                column: "RecipeId",
                principalTable: "Recipe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
