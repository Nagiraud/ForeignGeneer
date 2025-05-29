using ForeignGeneer.Assets.Scripts.Block;
namespace ForeignGeneer.Assets.Scripts.block.playerStructure;

public interface IPlayerStructure<TItemStatic> : IInteractable
    where TItemStatic : ItemStatic
{
    TItemStatic itemStatic { get; }
    void dismantle();
}