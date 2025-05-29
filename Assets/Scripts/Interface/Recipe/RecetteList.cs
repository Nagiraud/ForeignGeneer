	using Godot;
using System;
using ForeignGeneer.Assets.Scripts;
using ForeignGeneer.Assets.Scripts.Block;
using ForeignGeneer.Assets.Scripts.block.playerStructure.Factory;
using ForeignGeneer.Assets.Scripts.Interface;

public partial class RecetteList : Control,BaseUi
{
	[Export] private PackedScene _recipeUiPacked;
	private GridContainer _scrollContainer;
	private IInputFactory<CraftingFactoryStatic> _recipeUser;

	public override void _Ready()
	{
		_scrollContainer = GetNode<GridContainer>("ScrollContainer/GridContainer");
	}

	/// <summary>
	/// Initialise la liste des recettes.
	/// </summary>
	/// <param name="data">L'usine associée à cette liste de recettes.</param>
	public void initialize(Object data)
	{
		_recipeUser = (IInputFactory<CraftingFactoryStatic>)data;

		if (_recipeUser.itemStatic.recipeList == null || _recipeUser.itemStatic.recipeList.Count == 0)
		{
			return;
		}

		foreach (Recipe recipe in _recipeUser.itemStatic.recipeList)
		{
			RecipeChoiceUi recipeUi = _recipeUiPacked.Instantiate<RecipeChoiceUi>();
			recipeUi.init(recipe);
			recipeUi.Connect(nameof(CraftingRecipeChoiceUi.RecipeClicked), new Callable(this, nameof(onRecipeClicked)));
			_scrollContainer.AddChild(recipeUi);
		}
	}
	
	public void close()
	{
		_recipeUser.interact(InteractType.Close);
	}

	private void onRecipeClicked(Recipe recipe)
	{
		_recipeUser.setCraft(recipe);
	}
	public void update(InterfaceType? interfaceType)
	{
		switch (interfaceType)
		{
			case InterfaceType.Close:
				close();
				break;
		}
	}

	public void detach()
	{
		IObservable factory = (IObservable)_recipeUser;
		factory.detach(this);
	}
}
