using Godot;
using System;

public class CardData
{
	public string title;
	public string description;
	public int cardCost;

	public CardData(string title, string description, int cardCost)
	{
		this.title = title;
		this.description = description;
		this.cardCost = cardCost;
	}
}
