using Godot;
using Godot.Collections;

public partial class RecipeResource : Resource
{
	[Export]
	public Array<GemType> ingredientList = new Array<GemType>();
}
