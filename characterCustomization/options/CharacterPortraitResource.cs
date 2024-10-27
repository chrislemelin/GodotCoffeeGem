using Godot;
using System;

[GlobalClass, Tool]
public partial class CharacterPortraitResource : Resource
{
	[Export] public String name;
	[Export] public Color color;
	[Export] public Texture2D image;

	[Export] public bool unlockable;
	[Export] public int unlockCost;

}
