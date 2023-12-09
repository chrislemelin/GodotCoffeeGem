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

	[Export] private Array<Passive> gameStartPassives = new Array<Passive>();
	[Export] private Array<ExecutablePassive> gameStartExePassives = new Array<ExecutablePassive>();


	[Export] private Array<Passive> turnStartPassives = new Array<Passive>();
	[Export] private Array<ExecutablePassive> turnStartExePassives = new Array<ExecutablePassive>() ;

	//[Export] public Array<Passive> customActivationPassives;
	// todo Enum PassiveActivationType

	[Signal] public delegate void CounterChangedEventHandler(int newCount);
	[Export] public int counterMax;
	public int counter;

	public List<T> getStartPassives<T>() where T: Passive{
		return gameStartPassives.Where(passive => passive is T).Select(passive =>(T)passive).ToList();
	}

	public void incrementTurnCounter() {
		counter++;
		if (counter > counterMax) {
			counter = 1;
		}
		EmitSignal(SignalName.CounterChanged, counter);
	}

	public void resetCounter() {
		counter = 1;
		GetSignalConnectionList(SignalName.CounterChanged);
		
		EmitSignal(SignalName.CounterChanged, counter);
	}

	public List<ExecutablePassive> getGameStartExePassives() {
		return gameStartExePassives.ToList();
	}

	public List<ExecutablePassive> getTurnStartExePassives() {
		if (counterMax == 0 || counter == counterMax) {
			return turnStartExePassives.ToList();
		}
		return new List<ExecutablePassive>();
	}

}
