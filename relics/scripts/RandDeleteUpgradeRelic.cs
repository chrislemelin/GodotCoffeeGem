using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
[GlobalClass, Tool]
public partial class RandDeleteUpgradeRelic : RelicResource
{
	public override void cardDrawn(CardResource cardResource)
	{
		if (cardResource.cardEffect is CardEffectColorChangeRandom) {
			CardPassive cardPassive = new CardPassive();
			cardPassive.valueModification = 1;
			cardResource.cardEffect.addPassive(cardPassive);
		}
		if (cardResource.cardEffect is CardEffectRainbowChangeRandom) {
			CardPassive cardPassive = new CardPassive();
			cardPassive.valueModification = 1;
			cardResource.cardEffect.addPassive(cardPassive);
		}
	}

}
