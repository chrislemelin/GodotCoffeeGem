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

	public int getCost()
	{
		int cost = Cost;
		foreach (CardPassive cardPassive in cardEffect.getPassives())
		{
			cost += cardPassive.costModification;
		}
		return Math.Max(cost, 0);
	}
	public String getDescription()
	{
		String newDescription = Description.Replace("$value", cardEffect.getValue().ToString());
		return newDescription;
	}

	public bool equalToCard(CardResource cardResource)
	{
		return Title.Equals(cardResource.Title);
	}

}

