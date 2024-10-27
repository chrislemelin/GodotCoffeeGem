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
	[Export] public int cost = 0;

	[Export] public bool hidden = false;
	[Export] public bool lastForOneTurn = false;
	[Export] public bool lastForOneLevel = false;

	public RelicUI relicUI;

	[Export] private Array<EffectResource> gameStartEffects = new Array<EffectResource>();
	[Export] private Array<EffectResource> turnStartEffects = new Array<EffectResource>();
	[Export] private Array<EffectResource> turnStartEffectsAfterCleanUp = new Array<EffectResource>();
	[Export] private Array<EffectResource> addToInvEffects = new Array<EffectResource>();
	[Export] private Array<EffectResource> levelOverEffects = new Array<EffectResource>();
	[Export] private Array<EffectResource> effects = new Array<EffectResource>();
	[Export] private Array<MatchEffectResource> matchEffects = new Array<MatchEffectResource>();


	[Signal] public delegate void CounterChangedEventHandler(int newCount);
	// Max value for the counter, we expect to excute the effetcs in the effects list when this is reached
	[Export] public int counterMax;
	// Flag to deterimine if the counter goes up when a new turn starts or if this goes up by a custom call
	[Export] public bool customCounter;
	// Flag to determine if the we should reset the counter when the level starts
	[Export] public bool counterResetOnLevelStart = false;
	// Flag to determine if the we should reset the counter when the turn starts
	[Export] public bool counterResetOnTurnStart = false;
	// Flag to determine if we should reset the counter after we reach the max
	[Export] public bool resetCounterAfterReachingMax = true;
	// Flag to determine if the we should reset the counter when the level starts
	[Export] public bool doEffectsOnTurnStartIfCounterAtMax = false;

	public int counter = 0;
	public Node node;

	public void incrementTurnCounter()
	{
		if (!customCounter)
		{
			counter++;
			if (counter > counterMax)
			{
				if (resetCounterAfterReachingMax) {
					counter = 1;
				} else {
					counter = counterMax;
				}
			}
			EmitSignal(SignalName.CounterChanged, counter);
		}
	}

	public void incrementCounter()
	{
		if (counterMax == 0)
		{
			executeEffects();
		}
		else
		{
			counter++;
			if (counter == counterMax)
			{
				executeEffects();
			}
			if (counter > counterMax)
			{
				if (resetCounterAfterReachingMax)
				{
					counter = 1;
				}
				else
				{
					counter = counterMax;
				}
			}
			EmitSignal(SignalName.CounterChanged, counter);
		}
	}

	private void setCounter(int counter)
	{
		this.counter = counter;
		EmitSignal(SignalName.CounterChanged, counter);
	}

	public void executeEffects()
	{
		if (relicUI != null && effects.Count != 0)
		{
			relicUI.activateAnimation();
		}
		foreach (EffectResource effect in effects)
		{
			effect.execute(node);
		}
	}

	public void executeEffectsOrIncreaseCount()
	{
		if (customCounter)
		{
			incrementCounter();
		}
		else
		{
			executeEffects();
		}
	}

	public virtual void ingredientDestroyed(Gem gem)
	{

	}

	public virtual void cardDrawn(CardResource cardResource)
	{

	}

	public virtual void multChanged(float mult)
	{

	}

	public void startLevel()
	{
		if (counterResetOnLevelStart)
		{
			resetCounter();
		}
	}

	public virtual void newTurn()
	{
		if (counterResetOnTurnStart)
		{
			resetCounter();
		}
	}
	public virtual void afterTurnCleanUp()
	{
		foreach (EffectResource executablePassive in turnStartEffectsAfterCleanUp)
		{
			executablePassive.execute(node);
		}

	}
	public virtual void beforeTurnCleanUp()
	{

	}

	public virtual void ingredientsMatched(Match match)
	{
		foreach (MatchEffectResource matchEffect in matchEffects)
		{
			matchEffect.execute(node, match);
		}
	}

	public void resetCounter()
	{
		counter = 0;
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
		if (counter == counterMax && doEffectsOnTurnStartIfCounterAtMax)
		{
			executeEffects();
		}
		return new List<EffectResource>();
	}

	public virtual void setUp(){
		
	}

}
