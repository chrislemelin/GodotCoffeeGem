using Godot;
using System;
using System.Text.RegularExpressions;

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
	public static string spliceText(string text, int lineLength) {
		return Regex.Replace(text, "(.{" + lineLength + "})", "$1" + System.Environment.NewLine);
	}
}
