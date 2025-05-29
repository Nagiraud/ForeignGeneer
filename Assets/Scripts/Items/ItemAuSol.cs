using Godot;
using System;
using ForeignGeneer.Assets.Scripts.Block;

public partial class ItemAuSol : StaticBody3D, IInteractable
{
	public StackItem stackItem { get; set; }

	public override void _Ready()
	{
		base._Ready();
	
		// Assurez-vous que le CollisionShape3D est bien présent
		var collisionShape = GetNode<CollisionShape3D>("CollisionShape3D");
		if (collisionShape != null && collisionShape.Shape is BoxShape3D boxShape)
		{
			// Définir la taille de la boîte de collision à 0.2m
			boxShape.Size = new Vector3(0.2f, 0.2f, 0.2f);
		}
		else
		{
			GD.PrintErr("CollisionShape3D non trouvé ou la forme n'est pas une BoxShape3D.");
		}
	}

	public void Initialize(StackItem item)
	{
		stackItem = item;
	}

	public void interact(InteractType interactType)
	{
		switch (interactType)
		{
			case InteractType.Interact:
				InventoryManager.Instance.addItemToInventory(stackItem);
				QueueFree();
				break;
		}
	}
}
