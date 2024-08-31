using Godot;
using System;

public partial class AudioPlayer : AudioStreamPlayer2D
{
	[Export] bool soundEffect;
	[Export] float minValue = -20.0f;
	[Export] float maxValue = 40.0f;
	public float baseVolumeMult = 1.0f;

	public override void _Ready()
	{
		base._Ready();
		setUp();
	}

	public void setUp () {
		SettingsMenu settingsMenu = FindObjectHelper.getSettingsMenu(this);
		if (settingsMenu != null) {
			if (soundEffect) {
				settingsMenu.sfxSlider.ValueChanged += (double value) => {
					VolumeDb = (float)(minValue + (maxValue - minValue) * settingsMenu.sfxSlider.Value);
				};
			} else {
				settingsMenu.musicSlider.ValueChanged += (double value) => {
					VolumeDb = (float)(minValue + (maxValue - minValue) * value);
				};
			}
		recalculateVolume();
		}
	}

	private void recalculateVolume() {
		SettingsMenu settingsMenu = FindObjectHelper.getSettingsMenu(this);
		if (settingsMenu != null) {
			float settingsVolume;
			if (soundEffect) {
			settingsVolume = (float)(minValue + (maxValue - minValue) * (settingsMenu.sfxSlider.Value * baseVolumeMult));
			} else {
				settingsVolume = (float)(minValue + (maxValue - minValue) * settingsMenu.musicSlider.Value * baseVolumeMult);
			}
			VolumeDb = settingsVolume;
		}
	}

	public void setBaseVolumeMult(float value) {
		baseVolumeMult = value;
		recalculateVolume();
	}

}
