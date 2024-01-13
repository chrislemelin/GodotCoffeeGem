using Godot;
using System;

public partial class MatchScoreText : lerp
{
	[Export] AnimationPlayer animationPlayer;
	[Export] RichTextLabel textLabel;

	public int value;
	public float newMultValue;
	public int valueAfterMult;

	public Vector2 multPosition;
	public Vector2 scorePosition;
	public Score score;
	public Mult mult;

	Boolean movingToMult = true;


	public override void _Ready()
	{
		animationPlayer.Play("RiseAndFade");
	}

	public void setText(String text) {
		textLabel.Text = text;
	}

	public void init(int value, int valueAfterMult, float newMultValue,  Vector2 multPosition, Vector2 scorePosition, Score score, Mult mult) {
		this.value = value;
		this.newMultValue = newMultValue;
		this.valueAfterMult = valueAfterMult;
		this.multPosition =  multPosition;
		this.scorePosition = scorePosition;
		this.score = score;
		this.mult = mult;

		setText(value.ToString());
		moveToPostion(multPosition);
	}

	protected override void doneMoving()
	{
		base.doneMoving();
		if (movingToMult) {
			mult.setMult(newMultValue);
			setText(valueAfterMult.ToString());
			movingToMult = false;
			moveToPostion(scorePosition);
		} else {
			QueueFree();
		}
	}




}
