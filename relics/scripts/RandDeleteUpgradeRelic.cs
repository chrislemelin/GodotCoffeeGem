using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
[GlobalClass, Tool]
public partial class RandDeleteUpgradeRelic : RelicResource
{
	HashSet<CardResource> cardsAddedPassiveTo = new HashSet<CardResource>();
	public override void cardDrawn(CardResource cardResource)
	{
		if (cardResource.cardEffect is CardEffectColorChangeRandom) {
			addPassiveToCard(cardResource);
		}
		if (cardResource.cardEffect is CardEffectRainbowChangeRandom) {
			addPassiveToCard(cardResource);
		}
	}

	private void addPassiveToCard(CardResource cardResource) {
		if (!cardsAddedPassiveTo.Contains(cardResource)) {
			CardPassive cardPassive = new CardPassive();
			cardPassive.valueModification = 1;
			cardResource.cardEffect.addPassive(cardPassive);
			cardsAddedPassiveTo.Add(cardResource);
		}

	}

}
