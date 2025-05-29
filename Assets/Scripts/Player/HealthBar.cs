using Godot;

public partial class HealthBar : Control
{
	[Export] public TextureProgressBar HealthProgressBar;
	[Export] public TextureProgressBar PollutionProgressBar;
	[Export] public TextureProgressBar HungerProgressBar;

	private HealthData _healthData;
	public override void _Ready()
	{
		HealthProgressBar.MinValue = 0;
		HealthProgressBar.MaxValue = 100;
		HungerProgressBar.MinValue = 0;
		HungerProgressBar.MaxValue = 100;
		PollutionProgressBar.MinValue = 0;
		PollutionProgressBar.MaxValue = 10000;

		if (HealthManager.Instance != null)
		{
			Initialize(HealthManager.Instance.HealthData);
		}
		else
		{
			GD.Print("HealthManager not ready yet - will need manual initialization");
		}
	}

	public void Initialize(HealthData healthData)
	{
		if (_healthData != null)
		{
			_healthData.HealthChanged -= updateHealthDisplay;
			_healthData.HungerChanged -= updateHungerDisplay;
			_healthData.PollutionChanged -= updatePollutionDisplay;
		}

		_healthData = healthData;

		if (_healthData != null)
		{
			_healthData.HealthChanged += updateHealthDisplay;
			_healthData.HungerChanged += updateHungerDisplay;
			_healthData.PollutionChanged += updatePollutionDisplay;

			CallDeferred(nameof(updateDisplays));
			GD.Print("HealthBar initialized with HealthData");
		}
		else
		{
			GD.PrintErr("Failed to initialize HealthBar - HealthData is null");
		}
	}

	private void updateDisplays()
	{
		updateHealthDisplay();
		updateHungerDisplay();
		updatePollutionDisplay();
	}

	private void updateHealthDisplay()
	{
		GD.Print($"Updating health display: {_healthData.Health}");
		HealthProgressBar.Value = _healthData.Health;
	}

	private void updateHungerDisplay()
	{
		GD.Print($"Updating hunger display: {_healthData.Hunger}");
		HungerProgressBar.Value = _healthData.Hunger;
	}

	private void updatePollutionDisplay()
	{
		GD.Print($"Updating pollution display: {_healthData.Pollution}");
		PollutionProgressBar.Value = _healthData.Pollution;
	}

	public override void _ExitTree()
	{
		if (_healthData != null)
		{
			_healthData.HealthChanged -= updateHealthDisplay;
			_healthData.HungerChanged -= updateHungerDisplay;
			_healthData.PollutionChanged -= updatePollutionDisplay;
		}
	}
}
