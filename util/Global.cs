using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;

public partial class Global : Node
{
	public const String LOAD_STRING = "/root/Globals/";
	public CardList deckCardList = null;
	[Export]
	public Array<ColorUpgrade> colorUpgrades = new Array<ColorUpgrade>();
	public List<RelicResource> relics = new List<RelicResource>();

	public int currentLevel = 1;
	public int currentHealth = 2;
	public int currentCoins = 0;

}
