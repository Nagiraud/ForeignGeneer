using Godot;

public partial class BreakableResource : HarvestableResource
{
	[Export] private ItemStatic _resourceStatic { get; set; }

	private CollisionShape3D _collisionShape;

	public override void _Ready()
	{
		base._Ready();
		_collisionShape = GetNode<CollisionShape3D>("CollisionShape3D");
	}

	public override void Harvest()
	{
		if (IsActive) return;

		IsActive = true;
		Visible = false;
		_collisionShape.Disabled = true;
		InventoryManager.Instance.addItemToInventory(new StackItem(_resourceStatic));
		
	}

	public override void ResetResource()
	{
		IsActive = false;
		Visible = true;
		_collisionShape.Disabled = false;
		RespawnTimer = 10.0;
	}
}
