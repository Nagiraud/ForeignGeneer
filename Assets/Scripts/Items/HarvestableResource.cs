using ForeignGeneer.Assets.Scripts.Block;
using Godot;

public abstract partial class HarvestableResource : StaticBody3D, IInteractable
{
    [Export] public bool IsActive { get; set; } = false;
    [Export] public double RespawnTimer { get; set; } = 5.0;
    [Export] public Resource ItemResource { get; protected set; }

    public abstract void Harvest();
    public abstract void ResetResource();

    public virtual void ProcessRespawn(double delta)
    {

        RespawnTimer -= delta;
        if (RespawnTimer <= 0)
        {
            ResetResource();

        }
    }

    public override void _Process(double delta)
    {
        ProcessRespawn(delta);
    }

    public void interact(InteractType interactType)
    {
        if (interactType == InteractType.Interact)
        {
            Harvest();
        }
    }

    public T GetItemResource<T>() where T : Resource
    {
        return ItemResource as T;
    }
}