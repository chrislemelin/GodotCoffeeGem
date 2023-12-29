using Godot;
using System;
using System.ComponentModel;
[GlobalClass, Tool]
public partial class ColorUpgrade : Resource
{
	[Export] public int baseIncrease = 0;
	[Export] public float multIncrease = 0;
	[Export] public GemType gemType;
	[Export] public float finalMult = 1;

	public ColorUpgrade() {

	}

	public ColorUpgrade add(ColorUpgrade colorUpgrade) {
		ColorUpgrade results = new ColorUpgrade();
		results.gemType = gemType;
		results.multIncrease = multIncrease + colorUpgrade.multIncrease;
		results.baseIncrease = baseIncrease + colorUpgrade.baseIncrease;
		results.finalMult = finalMult * colorUpgrade.finalMult;
		return results;
	}
}
