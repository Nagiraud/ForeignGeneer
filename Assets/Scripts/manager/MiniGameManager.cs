using System;
using Godot;
using Godot.Collections;

namespace ForeignGeneer.Assets.Scripts.manager;

public partial class MiniGameManager:Node
{
    public static MiniGameManager instance { get; private set; }
    [Export] public Array<Recipe> listRecipe = new Array<Recipe>();    
    public Circuit circuit = new Circuit();
    public override void _Ready()
    {
        if (instance == null)
        {
            instance = this;
        }
        AddChild(circuit);
    }
    
}