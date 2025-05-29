using Godot;
using System;

public partial class filterManager : CanvasLayer
{
    public static filterManager Instance { get; private set; }

    [Export]
    public ColorRect filter;

    public override void _Ready()
    {
        Instance = this;
    }

    public void SetFilter(int mode)
    {
        if (filter?.Material is ShaderMaterial shaderMaterial)
        {
            shaderMaterial.SetShaderParameter("mode", mode);
        }
    }

    public int GetMode()
    {
        if (filter?.Material is ShaderMaterial shaderMaterial)
        {
            return (int)shaderMaterial.GetShaderParameter("mode");
        }
        return 0;
    }
}