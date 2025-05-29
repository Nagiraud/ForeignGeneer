using Godot;
using System;
using System.Collections.Generic;

public partial class SpawnerManager : Node3D
{


	public StaticBody3D map;
	public CollisionShape3D mapShape;
	[Export] 
	public Node _manager;

	[Export]
	public PackedScene[] objectList = new PackedScene[0]; 

	[Export]
	public int SpawnCountPerObject = 100; 

	private Vector2 MapSize;

	public override void _Ready()
	{
		if (map == null)
		{
			map = _manager.GetParent().GetNodeOrNull<StaticBody3D>("Terrain");
			if (map != null)
			{
				GD.Print("Map assigned successfully");
				InitializeSpawner();
				SetProcess(false);
			}
		}

		mapShape = map.GetNodeOrNull<CollisionShape3D>("TerrainCollision");

		if (mapShape == null)
		{
			GD.PrintErr("No collisionShape on terrain");
			return;
		}


		if (mapShape.Shape is BoxShape3D boxShape)
		{
			Vector3 extents = boxShape.Size / 2; 
			MapSize = new Vector2(extents.X, extents.Z);
			GD.Print($"Map size: {MapSize}");
		}
		else
		{
			GD.PrintErr("TerrainCollision does not have a BoxShape3D!");
		}

		SpawnObjects();
	}


	private void SpawnObjects()
	{
		if (objectList.Length == 0)
		{
			GD.PrintErr("No objects to spawn!");
			return;
		}

		RandomNumberGenerator rng = new RandomNumberGenerator();
		int a = 0; //TODO
		foreach (PackedScene prefab in objectList)
		{
			a++;//TODO Retire le test
			
			if (prefab == null)
				continue;

			for (int i = 0; i < SpawnCountPerObject; i++)
			{

				//float x = rng.RandfRange(-MapSize.X+100, MapSize.X-100);
				float x = (a+1)*10;
				//float z = rng.RandfRange(-MapSize.Y+100, MapSize.Y-100);
				float z = 0;
				float y = GetGroundHeight(new Vector3(x, 10, z)); 

				Vector3 spawnPosition = new Vector3(x, y, z);

				Node3D instance = (Node3D)prefab.Instantiate();
				instance.Position = spawnPosition;
				AddChild(instance);
			}
		}
	}

	private float GetGroundHeight(Vector3 position)
	{
		PhysicsRayQueryParameters3D query = new PhysicsRayQueryParameters3D
		{
			From = position,
			To = new Vector3(position.X, -10, position.Z),
			CollideWithBodies = true
		};

		var space = GetWorld3D().DirectSpaceState;
		var result = space.IntersectRay(query);

		if (result.Count > 0 && result.ContainsKey("position"))
		{
			return ((Vector3)result["position"]).Y;
		}

		return 0;
	}
	private void InitializeSpawner()
	{
		CollisionShape3D collisionShape = map.GetNode<CollisionShape3D>("TerrainCollision");

		if (collisionShape == null)
		{
			GD.PrintErr("No collisionShape on terrain");
			return;
		}

		Vector3 extents = ((BoxShape3D)collisionShape.Shape).Size / 2;
		MapSize = new Vector2(extents.X, extents.Z);

		SpawnObjects();
	}
}
