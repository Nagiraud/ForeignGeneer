using System;
using ForeignGeneer.Assets.Scripts;
using ForeignGeneer.Assets.Scripts.block.playerStructure.Factory;
using Godot;
using ForeignGeneer.Assets.Scripts.Interface;
using ForeignGeneer.Assets.Scripts.manager;

public partial class WindUi : Control,BaseUi
{
    private WindCentral _central;
    private ProgressBar _craftProgressBar;
    private Label _electricityLabel;

    public override void _Ready()
    {
        base._Ready();
        _craftProgressBar = GetNode<ProgressBar>("machine/ProgressBar");
        _electricityLabel = GetNode<Label>("machine/Quantity");
    }

    /// <summary>
    /// Initializes the central interface. This method should be called once to initialize the UI.
    /// </summary>
    /// <param name="data">The central node associated with this interface.</param>
    public void initialize(Object data)
    {
        _central = (WindCentral)data;
        updateElectricity();
    }

    /// <summary>
    /// Updates the central interface UI. This method is called on each UI update.
    /// </summary>
    public void updateUi()
    {
        updateElectricity();
    }

    public void close()
    {
        _central.closeUi();
    }
    
    /// <summary>
    /// Updates the electricity display.
    /// </summary>
    public void updateElectricity()
    {
        if (_electricityLabel != null && _central != null)
        {
            _electricityLabel.Text = $"Puissance : {_central.powerGenerated} kW/s || Électricité totale : {EnergyManager.getInstance().getGlobalElectricity()} kW/s";
        }
    }
    public void update(InterfaceType? interfaceType)
    {
        switch (interfaceType)
        {
            case InterfaceType.Energy:
                updateElectricity();
                break;
            case InterfaceType.Close:
                close();
                break;
            default:
                updateUi();
                break;
        }
    }
    public void detach()
    {
        _central.detach(this);
    }
}
