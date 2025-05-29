using ForeignGeneer.Assets.Scripts;
using ForeignGeneer.Assets.Scripts.Interface;
using ForeignGeneer.Assets.Scripts.Interface.Slot;
using Godot;
public partial class CraftingRecipeChoiceUi: Control, BaseUi
{
    [Signal]
    public delegate void RecipeClickedEventHandler(Recipe recipe);
        
    [Export] private MiniSlotUi _outputRecipeChoiceUi;
    [Export] private GridContainer _gridContainer;
    [Export] private PackedScene _slotUi;
    [Export] private Label _nameLabel;
    [Export] private Button button;
    private Recipe _recipe;
    public void initialize(object data)
    {
        _recipe = data as Recipe;
        foreach (StackItem stackItem in _recipe.input)
        {
            MiniSlotUi miniSlotUi = _slotUi.Instantiate<MiniSlotUi>();
            miniSlotUi.initialize(stackItem);
            _gridContainer.AddChild(miniSlotUi);
        }
        _outputRecipeChoiceUi.initialize(_recipe.output);
        _nameLabel.Text = _recipe.output.getResource().GetName();
        button.MouseFilter = Control.MouseFilterEnum.Pass;
        button.Connect("pressed", new Callable(this, nameof(onButtonPressed)));
    }
    public void update(InterfaceType? interfaceType)
    {
    }

    public void detach()
    {
        
    }

    public void onButtonPressed()
    {
        EmitSignal(nameof(RecipeClicked), _recipe);
    }
    
}