using ForeignGeneer.Assets.Scripts.block.playerStructure;
using Godot;
using ForeignGeneer.Assets.Scripts.manager;

public partial class WindCentral : PlayerBaseStructure, IPlayerStructure<FactoryStatic>
{
    [Export] public string factoryUiName { get; set; }
    [Export] public FactoryStatic itemStatic
    {
        get => base.itemStatic as FactoryStatic;
        set => SetItemStatic(value);
    }

    private WindUi _centralUi;
    private float coef = 0.2f;
    public float powerGenerated;
    public override void _Ready()
    {
        base._Ready();
        powerGenerated = itemStatic.electricalCost *
                         (1 + coef * Mathf.Log(1 + GlobalPosition.Y));
        powerGenerated = Mathf.Round(powerGenerated * 10)/10;
        EnergyManager.getInstance().addGlobalElectricity(powerGenerated);
    }

    public override void openUi()
    {
        closeUi();
        UiManager.instance.openUi(factoryUiName, this);
        _centralUi = (WindUi)UiManager.instance.getUi(factoryUiName);
    }

    public override void closeUi()
    {
        detach(_centralUi);
        _centralUi = null;
        UiManager.instance.closeUi();
    }
    
}