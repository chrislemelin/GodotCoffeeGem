using Godot;
using System;
using System.Collections.Generic;

public partial class HighScoreInput : Node
{
	[Export] PackedScene scoreText;
	[Export] Control scoreHolder;


	public  override void _Ready() {
	}
}
