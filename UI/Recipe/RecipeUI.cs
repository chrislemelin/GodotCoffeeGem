using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class RecipeUI : Control
{
	[Export]
	private Control recipeComponentHolder;
	[Export]
	Color inactiveColor;
	[Export] PackedScene ingredientScene;

	RecipeResource recipeResource;
	//List<Boolean> completedComponents = new List<Boolean>();

	public void loadRecipe(RecipeResource recipeResource)
	{
		//this.Visible = true;
		this.recipeResource = recipeResource;
		renderIngredientList();
		recipeResource.statusChanged += renderIngredientList;
	}
	private void renderIngredientList()
	{
		foreach (Node node in recipeComponentHolder.GetChildren())
		{
			node.QueueFree();
		}
		foreach (Tuple<GemType, Boolean> ingredient in recipeResource.getCompletionStatus())
		{
			TextureRect textureRect = (TextureRect)ingredientScene.Instantiate();
			GemType gemType = ingredient.Item1;
			textureRect.Texture = gemType.getTexture2D();
			if (!ingredient.Item2)
			{
				textureRect.Modulate = inactiveColor;
			}
			else
			{
				textureRect.Modulate = new Color(1.0f, 1.0f, 1.0f);
			}
			recipeComponentHolder.AddChild(textureRect);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (recipeResource != null)
		{
			recipeResource.statusChanged -= renderIngredientList;
		}
		base.Dispose(disposing);
	}
}
