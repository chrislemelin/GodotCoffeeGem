using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class GambleEffect : EffectResource
{	

	public GambleEffect() {
		 
	}
	
	protected override void executeEffect(Node node) {
		Score score = FindObjectHelper.getScore(node);
		List<GemType> gemTypes = GemTypeHelper.getRandomColors(2);

		ColorUpgrade colorUpgradeDouble = new ColorUpgrade();
		colorUpgradeDouble.gemType = gemTypes[0];
		colorUpgradeDouble.finalMult = 2;
		score.addColorUpgrade(colorUpgradeDouble);

		ColorUpgrade colorUpgradeZero = new ColorUpgrade();
		colorUpgradeZero.gemType = gemTypes[1];
		colorUpgradeZero.finalMult = 0;
		score.addColorUpgrade(colorUpgradeZero);
	}
}
