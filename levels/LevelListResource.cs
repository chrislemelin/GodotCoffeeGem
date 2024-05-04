using Godot;
using Godot.Collections;
using System.Collections.Generic;
[GlobalClass, Tool]
public partial class LevelListResource : Resource
{
	[Export] public Array<LevelResource> levelResources = new Array<LevelResource>();
}
