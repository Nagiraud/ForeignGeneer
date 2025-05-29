using Godot;
[GlobalClass]
public partial class ItemStatic : Resource
{
	[Export] private int _maxStack;
	[Export] public string _scenePath;
	[Export] private Material _material;
	[Export] private Texture2D _inventoryIcon;
	[Export] private string _description;
	[Export]
	public AudioStream useSoundEffect { get; private set; }
	private AudioManager _audioManager;
	public int getMaxStack
	{
		get => _maxStack;
		set => _maxStack = value;
	}

	public string getDescription()
	{
		return _description;
	}
	public string getPrefab
	{
		get => _scenePath;
		set => _scenePath = value;
	}

	public Material getMaterial
	{
		get => _material;
		set => _material = value;
	}

	public Texture2D getInventoryIcon
	{
		get => _inventoryIcon;
		set => _inventoryIcon = value;
	}

	public ItemStatic() {}

	public ItemStatic(Texture2D inventoryIcon, int maxStack)
	{
		_maxStack = maxStack;
		_inventoryIcon = inventoryIcon;
	}

	public StaticBody3D instantiate(Vector3 pos = new Vector3())
	{
		if (string.IsNullOrEmpty(_scenePath))
		{
			return null;
		}

		PackedScene scene = GD.Load<PackedScene>(_scenePath);
		if (scene == null)
		{
			return null;
		}

		var itemInstantiate = scene.Instantiate<StaticBody3D>();
		if (itemInstantiate == null)
		{
			return null;
		}

		if (_material != null)
		{
			MeshInstance3D meshInstance = itemInstantiate.GetNodeOrNull<MeshInstance3D>("MeshInstance3D");
			if (meshInstance != null)
			{
				meshInstance.MaterialOverride = _material;
			}
		}

		itemInstantiate.GlobalPosition = pos;
		return itemInstantiate;
	}
	
	public virtual void LeftClick()
	{
		
	}

	public virtual void RightClick()
	{
		_audioManager = AudioManager.instance;
		PlaySoundEffect();
		
	}

	public override string ToString()
	{
		return " name : " + GetName() + "\n scenePath : " + _scenePath + "\n inventoryIcon : " 
			   + _inventoryIcon.ToString() + "\n maxStack : " + _maxStack + "\n material : " + _material.ToString();
	}

	public virtual void PlaySoundEffect()
	{
		if (useSoundEffect != null && _audioManager != null)
		{
			GD.Print($"Playing eating sound: {useSoundEffect.ResourcePath}");
			_audioManager.playSound(useSoundEffect);
		}
	}





}
