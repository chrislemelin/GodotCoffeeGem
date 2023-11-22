using Godot;
using System;
[GlobalClass, Tool]
public partial class CardResource : Resource
{
	[Export] public string Title { get; set; }
	[Export] public string Description { get; set; }
	[Export] public int Cost { get; set; }
	[Export] public CardEffectIF cardEffect {get;set;}


	public CardResource() : this(null, null, 0, null) { }

	public CardResource(string title, string description, int cost, CardEffectIF cardEffect)
	{
		this.Title = title;
		this.Description = description;
		this.Cost = cost;
		this.cardEffect = cardEffect;
	}

	public SelectionType getSelectionType() {
		return cardEffect.getSelectionType();
	}
}

