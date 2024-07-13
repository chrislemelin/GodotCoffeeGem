using Godot;
using System;

public partial class DialogueSceneGlobal
{
	public Optional<DialougeSceneResource> dialougeSceneResource = Optional.None<DialougeSceneResource>();

	public Optional<DialougeSceneResource> getDialouge()
	{
		return dialougeSceneResource;
	}

	public void clearDialouge()
	{
		dialougeSceneResource = Optional.None<DialougeSceneResource>();
	}

	public void setDialouge(DialougeSceneResource scene)
	{
		dialougeSceneResource = Optional.Some(scene);
	}
	
}
