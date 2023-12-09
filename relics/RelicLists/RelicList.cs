using Godot;
using Godot.Collections;
using System;

[GlobalClass, Tool]
public partial class RelicList : Resource
{
	[Export] public Array<RelicResource> allRelics = new Array<RelicResource>();

	public RelicList () {
	
	}
}
