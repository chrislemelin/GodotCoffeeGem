using Godot;
using System;
using System.ComponentModel;
[GlobalClass, Tool]
public partial class ColorUpgrade : Resource
{
	[Export] public int baseIncrease;
	[Export] public float multIncrease;
	[Export] public GemType gemType;
	public ColorUpgrade() {

	}

	public ColorUpgrade add(ColorUpgrade colorUpgrade) {
		ColorUpgrade results = new ColorUpgrade();
		results.gemType = gemType;
		results.multIncrease = multIncrease + colorUpgrade.multIncrease;
		results.baseIncrease = baseIncrease + colorUpgrade.baseIncrease;
		return results;
	}
}
