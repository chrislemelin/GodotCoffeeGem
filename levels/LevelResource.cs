using Godot;
using System;

public partial class LevelResource : Resource
{
	[Export]
	public int score;
	[Export]
	public RecipeResource bossRecipe;
	[Export]
	public bool makeRandomBossRecipe;

	public RecipeResource getBossRecipe(){
		if (bossRecipe != null) {
			return bossRecipe;
		}
		if (makeRandomBossRecipe) {
			RecipeResource recipeResource = new RecipeResource();
			recipeResource.ingredientList.Add(GemTypeHelper.getRandomColor());
			recipeResource.ingredientList.Add(GemTypeHelper.getRandomColor());
			return recipeResource;
		}
		return null;
	}
}
