using Godot;
using MonoCustomResourceRegistry;

[RegisteredType(nameof(FoodStatic), "", nameof(ItemStatic))]
[GlobalClass]
public partial class FoodStatic : ItemStatic
{

	[Export]
	public float FeedPower { get; private set; } = 1f;

	private HealthManager _healthManager;


	public override void RightClick()
	{
		base.RightClick();

		_healthManager= HealthManager.Instance;

		var inventoryManager = InventoryManager.Instance;

		if (!CanEat())
		{
			return;
		}

		ConsumeFood(inventoryManager);
	}

	private bool CanEat()
	{
		if (_healthManager == null || _healthManager.HealthData == null)
		{
			GD.Print(_healthManager.HealthData, "<= health data  health manager=>", _healthManager);
			GD.PrintErr("HealthManager or HealthData not available!");
			return false;
		}

		return _healthManager.HealthData.Hunger < HealthData.MAXHUNGER;
	}

	private void ConsumeFood(InventoryManager inventoryManager)
	{
		int currentSlot = inventoryManager.currentSlotHotbar;
		inventoryManager.hotbar.removeItem(currentSlot, 1);

		_healthManager.HealthData.modifyHunger(FeedPower);

	}


}
