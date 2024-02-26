using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class RelicResource : Resource
{
	[Export] public String title;
	[Export(PropertyHint.MultilineText)] public String description;
	[Export] public Texture2D image;
	[Export(PropertyHint.MultilineText)] public int cost = 0;

	[Export] private Array<EffectResource> gameStartEffects = new Array<EffectResource>();
	[Export] private Array<EffectResource> turnStartEffects = new Array<EffectResource>();
	[Export] private Array<EffectResource> addToInvEffects = new Array<EffectResource>();
	[Export] private Array<EffectResource> levelOverEffects = new Array<EffectResource>();
	[Export] private Array<EffectResource> effects = new Array<EffectResource>();

	[Signal] public delegate void CounterChangedEventHandler(int newCount);
	[Export] public int counterMax;
	[Export] public bool customCounter;
	public int counter = 1;
	public Node node;

	public void incrementTurnCounter()
	{
		if (!customCounter) {
			counter++;
			if (counter > counterMax)
			{
				counter = 1;
			}
			EmitSignal(SignalName.CounterChanged, counter);
		}
	}

	public void incrementCounter()
	{
		counter++;
		if (counter > counterMax)
		{
			counter = 1;
			foreach (EffectResource effect in effects)
			{
				effect.execute(node);
			}
		}
		EmitSignal(SignalName.CounterChanged, counter); 
	}

	public void executeEffects() {
		foreach (EffectResource effect in effects)
		{
			effect.execute(node);
		}
	}

	public void executeEffectsOrIncreaseCount() {
		if (customCounter) {
			incrementCounter();
		} else {
			foreach (EffectResource effect in effects)
			{
				effect.execute(node);
			}
		}
	}

	public virtual void ingredientDestroyed(Gem gem){

	}

	public virtual void cardDrawn(CardResource cardResource){

	}

	public virtual void multChanged(float mult){

	}

	public virtual void newTurn(){

	}
	public virtual void afterTurnCleanUp(){

	}
	public virtual void beforeTurnCleanUp(){

	}

	public virtual void ingredientsMatched(Match match){

	}

	public void resetCounter()
	{
		counter = 1;
		EmitSignal(SignalName.CounterChanged, counter);
	}

	public List<EffectResource> getGameStartExePassives()
	{
		return gameStartEffects.ToList();
	}

	public List<EffectResource> getAddToInventoryExePassives()
	{
		return addToInvEffects.ToList();
	}

	public void levelOver()
	{
		foreach (EffectResource executablePassive in levelOverEffects)
		{
			executablePassive.execute(node);
		}
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
		if (counterMax == 0 || counter == counterMax && !customCounter)
		{
			return turnStartEffects.ToList();
		}
		return new List<EffectResource>();
	}

}
