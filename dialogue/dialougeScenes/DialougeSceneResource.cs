using DialogueManagerRuntime;
using Godot;
using System;

[GlobalClass, Tool]
public partial class DialougeSceneResource : Resource
{
	[Export] Resource dialougeResource;
	[Export] string nextSceneString;
}
