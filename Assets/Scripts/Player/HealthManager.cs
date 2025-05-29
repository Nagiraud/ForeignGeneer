using Godot;

public partial class HealthManager : Node
{
	private HealthData _healthData = new HealthData();
	private float _previousVelocityY = 0.0f;
	private double _hungerTimer = 10.0;

	public HealthData HealthData => _healthData;
	public static HealthManager Instance { get; private set; }

	public override void _Ready()
	{
		if (Instance != null && Instance != this)
		{
			QueueFree(); 
			return;
		}
		Instance = this;

		
		_healthData.modifyHealth(100); 
		_healthData.modifyHunger(0);
		_healthData.modifyPollution(0);

		GD.Print("HealthManager initialized");
	}
	public override void _PhysicsProcess(double delta)
	{
		if (Player.Instance == null) return;
		HandleFallDamage();
		_previousVelocityY = Player.Instance.Velocity.Y;
	}

	private void HandleFallDamage()
	{
		if (Player.Instance == null || !Player.Instance.IsOnFloor()) return;

		if (_previousVelocityY < -8)
		{
			float fallDamage = Mathf.Abs(_previousVelocityY);
			_healthData.modifyHealth(-(fallDamage * 2 + 1));
		}
	}

	public override void _Process(double delta)
	{
		UpdateHungerSystem(delta);
		CheckHealthStatus();
	}

	private void UpdateHungerSystem(double delta)
	{
		_hungerTimer -= delta;

		if (_hungerTimer <= 0)
		{
			_hungerTimer = 10.0;
			GD.Print($"Before hunger update - Hunger: {_healthData.Hunger}");

			if (_healthData.Hunger > 80 && _healthData.Health < HealthData.MAXHEALTH)
			{
				_healthData.modifyHealth(10);
			}

			if (_healthData.Hunger <= 1)
			{
				_healthData.modifyHealth(-5);
			}

			_healthData.modifyHunger(-1);
			GD.Print($"After hunger update - Hunger: {_healthData.Hunger}");
		}
	}

	private void CheckHealthStatus()
	{
		if (_healthData.Health <= 0)
		{
			//ResetPlayer();
		}
	}

	private void ResetPlayer()
	{
		if (Player.Instance == null) return;

		Player.Instance.GlobalPosition = Vector3.Zero;

		// Directly set values instead of modifying by difference
		_healthData.modifyHealth(HealthData.MAXHEALTH);
		_healthData.modifyHunger(HealthData.MAXHUNGER);

		GD.Print($"Player reset - Health: {_healthData.Health}, Hunger: {_healthData.Hunger}");
	}
}
