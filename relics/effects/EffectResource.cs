using Godot;
using System;
using System.Diagnostics.Contracts;

[GlobalClass, Tool]
public partial class EffectResource : Resource
{
	[Export]
	protected int value;
	public virtual void execute(Node node) {
		
	}
}
