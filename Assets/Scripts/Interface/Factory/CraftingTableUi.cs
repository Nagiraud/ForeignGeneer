using ForeignGeneer.Assets.Scripts.Block;
using ForeignGeneer.Assets.Scripts.block.playerStructure.Factory;
using Godot;
using ForeignGeneer.Assets.Scripts;

namespace ForeignGeneer.Assets.Scripts.Interface.Factory;

public partial class CraftingTableUi : Control,BaseUi
{
    [Export] private SlotUI _slot;
    [Export] private GridContainer _gridContainer;
    [Export] private ProgressBar _progressBar;
    [Export] private PackedScene _recipeUiPacked;
    
    private CraftingTable _craftingTable;
    public void initialize(object data)
    {
        _craftingTable = data as CraftingTable;
        initRecipeList();
        initOutputSlot();
    }
    
    public void updateUi()
    {
        updateProgressBar();
        updateOutputSlot();
        _progressBar.Value = 0;
    }

    private void initRecipeList()
    {
        foreach (Recipe recipe in _craftingTable.itemStatic.recipeList)
        {
            CraftingRecipeChoiceUi recipeUi = _recipeUiPacked.Instantiate<CraftingRecipeChoiceUi>();
            recipeUi.initialize(recipe);
            recipeUi.Connect(nameof(CraftingRecipeChoiceUi.RecipeClicked), new Callable(this, nameof(onRecipeClicked)));
            _gridContainer.AddChild(recipeUi);
        }
    }
    private void updateOutputSlot()
    {
        if (_slot != null)
        {
            if (_craftingTable.output.getItem(0) != null)
            {
                _slot.updateUi();  
            }
            else
            {
                _slot.clearSlot(); 
            }
        }
    }
    private void initOutputSlot()
    {
        _slot.initialize(_craftingTable.output, 0, true);
    }
    public void close()
    {
        _craftingTable.interact(InteractType.Close);
    }

    private void updateProgressBar()
    {
        _progressBar.Value =  _craftingTable.craft.craftProgress* 100;
    }

    public void onRecipeClicked(Recipe recipe)
    {
        _craftingTable.setCraft(recipe);
    }
    
    public void update(InterfaceType? interfaceType)
    {
        switch (interfaceType)
        {
            case InterfaceType.Energy:
                break;
            case InterfaceType.Progress:
                updateProgressBar();
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
        _craftingTable.detach(this);
    }
}