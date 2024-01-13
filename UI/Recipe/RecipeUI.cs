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

	RecipeResource recipeResource;
	List<Boolean> completedComponents = new List<Boolean>();

	public void loadRecipe(RecipeResource recipeResource) {
		this.Visible = true;
		this.recipeResource = recipeResource;
		completedComponents.Clear();
		for(int count = 0; count < recipeResource.ingredientList.Count; count++) {
			completedComponents.Add(false);
		}
		renderIngredientList();

	}
	private void renderIngredientList() {
		for(int count = 0; count < recipeComponentHolder.GetChildCount(); count ++) {
			TextureRect textureRect = (TextureRect)recipeComponentHolder.GetChild(count);
			if (count >= recipeResource.ingredientList.Count) {
				textureRect.Visible = false;
				continue;
			}
			GemType gemType = recipeResource.ingredientList[count];
			textureRect.Texture = gemType.getTexture2D();
			if (!completedComponents[count]) {
				textureRect.Modulate = inactiveColor;
			} else {
				textureRect.Modulate = new Color(1.0f,1.0f,1.0f);
			}
		}
	}
	public bool complete(){
		return completedComponents.All(value => value);
	}
	public void evaulateMatch(GemType match){
		for(int count = 0; count < completedComponents.Count; count++) {
			if (!completedComponents[count] && recipeResource.ingredientList[count] == match) {
				completedComponents[count] = true;
				break;
			}
		}
		renderIngredientList();
	}
}
