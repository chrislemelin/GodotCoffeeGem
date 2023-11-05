using Godot;
using System;

public partial class CardResource : Resource
{
	[Export] public string Title { get; set; }
	[Export] public string Description { get; set; }
	[Export] public int Cost { get; set; }

	public CardResource() : this(null, null, 0) { }

	public CardResource(string title, string description, int cost)
	{
		this.Title = title;
		this.Description = description;
		this.Cost = cost;
	}
}

