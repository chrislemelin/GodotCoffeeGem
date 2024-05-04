using Godot;
using System;
using System.Diagnostics.Contracts;

[GlobalClass, Tool]
public partial class MatchEffectResource : Resource
{
	[Export]
	protected int value;
	public virtual void execute(Node node, Match match) {
		
	}
}
