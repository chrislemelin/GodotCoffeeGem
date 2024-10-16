using Godot;
using Godot.Collections;
using System;

public partial class DeckTutorial : Node
{
	[Export] 
	public bool tutorial = false;
	[Export]
	protected CardList tutorialCards;
}
