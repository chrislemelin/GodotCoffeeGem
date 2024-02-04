using Godot;
using System;

public static class TextHelper
{
	public static string centered(string text)
	{
		return "[center]" + text + "[/center]";
	}
	public static string bold(string text)
	{
		return "[b]" + text + "[/b]";
	}

	public static string toolTip(string text, string toolTip)
	{
		return "[hint={" + toolTip + "}]" + text + "[/hint]";
		// [hint={test}]blah blah blah[/hint]
	}
}
