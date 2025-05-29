namespace ForeignGeneer.Assets.Scripts.block.playerStructure.Factory;

public interface IOutputFactory<TFactory> : IPlayerStructure<TFactory>
    where TFactory : FactoryStatic
{
    Inventory output { get; set; }
    public BaseCraft craft { get; set; }
    void setCraft(Recipe recipe);
}