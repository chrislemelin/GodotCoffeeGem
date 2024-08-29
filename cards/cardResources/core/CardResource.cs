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
	//[Export] public bool playable { get; set; } = true; 
	[Export] public int coinPlayRequirement { get; set; } = 0;
	[Export] public int coinPlayCost { get; set; } = 0;
	public Node node;

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

	public bool canPlayCard() {
		// if(!playable) {
		// 	return false;
		// }
		GameManagerIF gameManager = FindObjectHelper.getGameManager(node);
		if (coinPlayRequirement > gameManager.getCoins()) {
			return false;
		}
		if (coinPlayCost > gameManager.getCoins()) {
			return false;
		}
		return true;
	}

	public string getEnergyCostString()
	{
		int cost = getEnergyCost();
		if (cardEffect.spendX){
			return "X";
		}
		if (cost != Cost)
		{
			return "[color=#2c8518]" + cost.ToString() + "[/color]";
		}
		return cost.ToString();
	}

	public int getCoinCost()
	{
		if (coinCost == 0)
		{
			if (rarity == CardRarity.Common)
			{
				return 30;
			}
			if (rarity == CardRarity.Uncommon)
			{
				return 45;
			}
			if (rarity == CardRarity.Rare)
			{
				return 60;
			}
		}
		return coinCost;
	}

	public String getDescription()
	{
		String newDescription = Description.Replace("$value", cardEffect.getValueString());
		newDescription = newDescription.Replace("$manaGems", cardEffect.getManaGems().ToString());
		newDescription = newDescription.Replace("$cardGems", cardEffect.getCardGems().ToString());
		newDescription = newDescription.Replace("$moneyGems", cardEffect.getCoinGems().ToString());
		newDescription = newDescription.Replace("$customText", cardEffect.getCustomText().ToString());

		//var values = Enum.GetValues(typeof(Foos));
		foreach(GemType gemType in Enum.GetValues(typeof(GemType))) {
			newDescription = newDescription.Replace(gemType.getString(), TextHelper.getIngredientImage(gemType));
		}
		newDescription = newDescription.Replace("$energy", TextHelper.getEnergyImage());
		newDescription = newDescription.Replace("$draw", TextHelper.getCardImage());
		//newDescription = newDescription.Replace("card", TextHelper.getCardImage());

		newDescription = newDescription.Replace("$coin", TextHelper.getCoinImage());


		// newDescription = newDescription.Replace("coffee", TextHelper.colorText("coffee", "brown"));
		// newDescription = newDescription.Replace("leaf", TextHelper.colorText("leaf", "green"));
		// newDescription = newDescription.Replace("tea", TextHelper.colorText("tea", "green"));
		// newDescription = newDescription.Replace("vanilla", TextHelper.colorText("vanilla", "yellow"));
		// newDescription = newDescription.Replace("sugar", TextHelper.colorText("sugar", "white"));
		// newDescription = newDescription.Replace("milk", TextHelper.colorText("milk", "purple"));

		// newDescription = newDescription.Replace("burnt", TextHelper.colorText("burnt", "grey"));




		if (cardEffect.consume)
		{
			newDescription += " " + TextHelper.toolTip("Consume.", "Consume cards are trashed until the next level");
		}
		if (cardEffect.retain)
		{
			newDescription += " " + TextHelper.toolTip("Retain.", "Retain cards arn't discarded at the end of the turn");
		}
		if (cardEffect.matchy)
		{
			newDescription += " " + TextHelper.toolTip("Matchy.", "If this card destroys 3+ ingredients of the same type count it as a match");
		}
		return newDescription;
	}

	public String getToolTip() {
		String returnString = "";
		if (cardEffect.consume)
		{
			returnString += "Consume cards are trashed until the next level\n";
		}
		if (cardEffect.retain)
		{
			returnString += "Retain cards are not discarded at the end of the turn\n";
		}
		if (cardEffect.matchy)
		{
			returnString += "Matchy - If this card destroys 3+ ingredients of the same type count it as a match\n";
		}
		return returnString;
	}


	public bool equalToCard(CardResource cardResource)
	{
		return Title.Equals(cardResource.Title);
	}
	public void init()
	{
		cardEffect.init();
	}

}

