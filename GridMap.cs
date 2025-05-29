using Godot;

public partial class TerrainManager : Node3D
{
	[Export] public Material BaseMaterial; // Matériau de base (ex. : herbe)
	private MeshInstance3D terrainMesh; // Terrain cible

	public override void _Ready()
	{
		// Récupérer le MeshInstance dans la scène
		terrainMesh = GetNode<MeshInstance3D>("MeshInstance");

		// Assigner le matériau de base au terrain
		if (terrainMesh != null && BaseMaterial != null)
		{
			terrainMesh.MaterialOverride = BaseMaterial;
			GD.Print("Matériau de base appliqué au terrain.");
		}
	}
}
