using Godot;

public partial class MapTextureChanger3D : MeshInstance3D
{
	[Export] public CharacterBody3D Player;
	[Export] public Color OverlayColor = new Color(1, 0, 0, 1); // Example overlay color
	private ShaderMaterial _shaderMaterial;

	public override void _Ready()
	{
		if (MaterialOverride is ShaderMaterial material)
		{
			_shaderMaterial = material;
			_shaderMaterial.SetShaderParameter("overlay_color", OverlayColor);
		}
	}

	public override void _Process(double delta)
	{
		if (_shaderMaterial == null || Player == null) return;

		if (Input.IsKeyPressed(Key.R)) // 'R' key or remap in Input Map
		{
			_shaderMaterial.SetShaderParameter("player_position", Player.GlobalTransform.Origin);
			_shaderMaterial.SetShaderParameter("overlay_strength", 1.0f);
		}
		else if (Input.IsActionJustReleased("ui_accept"))
		{
			_shaderMaterial.SetShaderParameter("overlay_strength", 0.0f);
		}
	}
}
