using Godot;
using System;

public partial class RunResource : Resource
{
	public string name;
	public int score;
	public bool completed;

	public  RunResource(){
	}

	public  RunResource(string name, int score, bool completed){
		this.name = name;
		this.score = score;
		this.completed = completed;
	}

	public String toJson() {
		Godot.Collections.Dictionary<String, String> data = new Godot.Collections.Dictionary<String, String>();
		data.Add("name", name);
		data.Add("score", score.ToString());
		data.Add("completed", completed.ToString());

		return Json.Stringify(data);
	}

	public static RunResource fromJson(Json json) {
		Godot.Collections.Dictionary<String, String> data = new Godot.Collections.Dictionary<String, String>((Godot.Collections.Dictionary)json.Data);

		RunResource runResource = new RunResource();
		runResource.name = data["name"];
		runResource.score = int.Parse(data["score"]);
		runResource.completed = bool.Parse(data["completed"]);

		return runResource;
	}
}
