using ForeignGeneer.Assets.Scripts.block.playerStructure;
using Godot;
using ForeignGeneer.Assets.Scripts.manager;

public partial class SolarCentral : PlayerBaseStructure, IPlayerStructure<FactoryStatic>
{
    [Export] public string factoryUiName { get; set; }
    [Export]
    public FactoryStatic itemStatic
    {
        get => base.itemStatic as FactoryStatic;
        set => SetItemStatic(value);
    }

    private WindUi _centralUi;

    [Export] public float SunlightIntensity { get; set; } = 1.0f;
    [Export] public float PanelEfficiency { get; set; } = 0.2f;
    [Export] public float PanelSurfaceArea { get; set; } = 1.0f;
    [Export] public float TimeExposure { get; set; } = 1.0f;

    public float PowerGenerated { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        CalculatePowerGenerated(); 
        EnergyManager.getInstance().addGlobalElectricity(PowerGenerated); 
    }

    private void CalculatePowerGenerated()
    {
        PowerGenerated = SunlightIntensity * PanelEfficiency * PanelSurfaceArea * TimeExposure;
    }

    public override void openUi()
    {
        closeUi();
        UiManager.instance.openUi(factoryUiName, this);
        _centralUi = (WindUi)UiManager.instance.getUi(factoryUiName);
    }

    public void closeUi()
    {
        detach(_centralUi);
        _centralUi = null;
        UiManager.instance.closeUi();
    }
}