using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class RelicResource : Resource
{
	[Export] public String title;
	[Export(PropertyHint.MultilineText)] public String description;
	[Export] public Texture2D image;

	[Export(PropertyHint.MultilineText)] public int cost = 0;


	[Export] private Array<Passive> gameStartPassives = new Array<Passive>();
	[Export] private Array<EffectResource> gameStartExePassives = new Array<EffectResource>();


	[Export] private Array<Passive> turnStartPassives = new Array<Passive>();
	[Export] private Array<EffectResource> turnStartExePassives = new Array<EffectResource>();

	[Export] private Array<EffectResource> addToInvEffects = new Array<EffectResource>();

	[Signal] public delegate void CounterChangedEventHandler(int newCount);
	[Export] public int counterMax;
	public int counter = 1;

	public List<T> getStartPassives<T>() where T : Passive
	{
		return gameStartPassives.Where(passive => passive is T).Select(passive => (T)passive).ToList();
	}

	public void incrementTurnCounter()
	{
		counter++;
		if (counter > counterMax)
		{
			counter = 1;
		}
		EmitSignal(SignalName.CounterChanged, counter);
	}

	public void resetCounter()
	{
		counter = 1;
		EmitSignal(SignalName.CounterChanged, counter);
	}

	public List<EffectResource> getGameStartExePassives()
	{
		return gameStartExePassives.ToList();
	}

	public List<EffectResource> getAddToInventoryExePassives()
	{
		return addToInvEffects.ToList();
	}

	public void executeAddedToInvEffects(Node node)
	{
		foreach (EffectResource executablePassive in addToInvEffects)
		{
			executablePassive.execute(node);
		}
	}


	public List<EffectResource> getTurnStartEffects()
	{
		if (counterMax == 0 || counter == counterMax)
		{
			return turnStartExePassives.ToList();
		}
		return new List<EffectResource>();
	}

}
