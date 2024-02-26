using Godot;
using System;

public partial class MatchScoreText : lerp
{
	[Export] AnimationPlayer animationPlayer;
	[Export] RichTextLabel textLabel;
	[Export] Color targetColor;
	[Export] Color baseColor;
	[Export] Gradient gradient;

	[Export] int targetColorScore;

	public float newMultValue;
	public int value;
	public int valueAfterMult;
	public int valueStep;

	public Vector2 multPosition;
	public Vector2 scorePosition;
	public Score score;
	public Mult mult;
	Timer timer;

	Boolean movingToMult = true;



	public override void _Ready()
	{
		animationPlayer.Play("RiseAndFade");
	}

	public void setText(String text) {
		textLabel.Text = text;
	}

	public void init(int value, int valueAfterMult) {
		this.value = value;
		this.valueAfterMult = valueAfterMult;
		valueStep = Math.Max(5, (valueAfterMult - value) / 20);
		setText(value.ToString());
		timer = new Timer();
		AddChild(timer);
		timer.WaitTime = .05;
		timer.Timeout += () => updateText();
		timer.Start();
		textLabel.Modulate = gradient.Sample(Math.Clamp((value - 100) / (float)value,0.0f,1.0f));

	}

	public void updateText () {
		value += valueStep;
		if (value > valueAfterMult) {
			value = valueAfterMult;
			timer.Stop();
		}
		setText(value.ToString());
		textLabel.Modulate = gradient.Sample(Math.Clamp((value - 100) / (float)targetColorScore,0.0f,1.0f));
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
