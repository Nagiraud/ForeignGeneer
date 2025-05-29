using Godot;
public partial class RecipeChoiceUi : Control
{
    [Signal]
    public delegate void RecipeClickedEventHandler(Recipe recipe);
    [Export] private TextureRect _itemIcon;
    [Export] private Label _itemNameLabel;

    private Recipe _recipe;

    public override void _Ready()
    {
        Button button = GetNode<Button>("Button");
        button.Connect("pressed", new Callable(this, nameof(onButtonPressed)));
    }
    
    /// <summary>
    /// Initialise l'UI avec les détails de la recette.
    /// </summary>
    /// <param name="recipe">La recette à afficher.</param>
    public void init(Recipe recipe)
    {
        _recipe = recipe;

        // Vérifier si la recette est valide
        if (_recipe == null)
        {
            GD.PrintErr("La recette est null.");
            SetDefaultDisplay();
            return;
        }

        if (_recipe.output != null && _recipe.output.getResource() != null)
        {
            _itemIcon.Texture = _recipe.output.getResource().getInventoryIcon;
            _itemNameLabel.Text = _recipe.output.getResource().GetName();
        }
        else if (_recipe.input != null && _recipe.input.Count > 0 && _recipe.input[0] != null && _recipe.input[0].getResource() != null)
        {
            var inputItem = _recipe.input[0];
            _itemIcon.Texture = inputItem.getResource().getInventoryIcon;
            _itemNameLabel.Text = inputItem.getResource().GetName();
        }
        else
        {
            GD.PrintErr("La recette n'a ni sortie (output) ni entrée (input) valide.");
            SetDefaultDisplay();
        }
    }

    /// <summary>
    /// Affiche une icône et un texte par défaut.
    /// </summary>
    private void SetDefaultDisplay()
    {
        _itemIcon.Texture = null;
        _itemNameLabel.Text = "Recette invalide";
    }

    /// <summary>
    /// Événement déclenché lorsque l'utilisateur clique sur cette recette.
    /// </summary>
    public void onButtonPressed()
    {
        EmitSignal(nameof(RecipeClicked), _recipe);
    }

}