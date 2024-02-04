using Godot;
using System;
[GlobalClass, Tool]
public partial class CardResource : Resource
{
	[Export] public string Title { get; set; }
	[Export(PropertyHint.MultilineText)] public string Description { private get; set; }
	[Export] private int Cost { get; set; }
	[Export] public CardRarity rarity = CardRarity.Common;
	[Export] public Texture2D Picture { get; private set; }
	[Export] private int coinCost { get; set; }


	[Export] public CardEffectIF cardEffect { get; set; }

	public CardResource() : this(null, null, 0, null) { }

	public CardResource(string title, string description, int cost, CardEffectIF cardEffect)
	{
		this.Title = title;
		this.Description = description;
		this.Cost = cost;
		this.cardEffect = cardEffect;
	}

	public SelectionType getSelectionType()
	{
		return cardEffect.getSelectionType();
	}

	public int getEnergyCost()
	{
		int cost = Cost;
		foreach (CardPassive cardPassive in cardEffect.getPassives())
		{
			cost += cardPassive.costModification;
		}
		return Math.Max(cost, 0);
	}

	public string getEnergyCostString()
	{
		int cost = getEnergyCost();
		if (cost != Cost) {
			return "[color=#2c8518]" + cost.ToString() + "[/color]" ;
		}
		return cost.ToString();
	}

	public int getCoinCost()
	{
		if (coinCost == 0) {
			if(rarity == CardRarity.Common) {
				return 30;
			}
			if(rarity == CardRarity.Uncommon) {
				return 45;
			}
			if(rarity == CardRarity.Rare) {
				return 60;
			}
		}
		return coinCost;
	}

	public String getDescription()
	{
		String newDescription = Description.Replace("$value", cardEffect.getValueString());
		if (cardEffect.consume) {
			newDescription += " " + TextHelper.toolTip("Consume.", "Consume cards are trashed untill the next level");
		}
		if (cardEffect.retain) {
			newDescription += " " + TextHelper.toolTip("Retain.", "Retain cards arn't discarded at the end of the turn");
		}
		//return ":"
		return newDescription;
	}

	public bool equalToCard(CardResource cardResource)
	{
		return Title.Equals(cardResource.Title);
	}

}

