using Godot;
using System;
using System.Diagnostics.Contracts;

public partial class Tile : Node2D
{

	[Export] public int x;
	[Export] public int y;
	[Export] public Sprite2D sprite2D;

	public Gem Gem
	{
		get; set;
	}
}
