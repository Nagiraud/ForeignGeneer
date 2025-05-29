using ForeignGeneer.Assets.Scripts.manager;
using Godot;

namespace ForeignGeneer.Assets.Scripts.Block;

public partial class PreviewObject : StaticBody3D
{
    private Color _validColor = new Color(0, 1, 0, 0.5f);
    private Color _invalidColor = new Color(1, 0, 0, 0.5f);

    private bool _isValidPos = true;
    private MeshInstance3D _meshInstance = null;
    private RayCast3D _raycast;
    private CollisionShape3D _collisionShape3D;
    private Area3D _collisionArea;

    /// <summary>
    /// Initializes the preview object with a 3D model and sets up collision detection.
    /// </summary>
    /// <param name="item">The ItemStatic to preview.</param>
    public void Initialize(ItemStatic item)
    {
        StaticBody3D model = item.instantiate();
        AddChild(model);
        model.SetCollisionLayerValue(1, false);
        model.SetCollisionMaskValue(1, false);

        _meshInstance = model.GetNodeOrNull<MeshInstance3D>("MeshInstance3D");
        if (_meshInstance != null)
        {
            ApplyColor(_validColor);
        }

        _raycast = new RayCast3D();
        _raycast.Enabled = true;
        _raycast.TargetPosition = Vector3.Down * 0.1f;
        AddChild(_raycast);

        _collisionShape3D = model.GetNodeOrNull<CollisionShape3D>("CollisionShape3D");

        _collisionArea = new Area3D();
        CollisionShape3D areaCollisionShape = new CollisionShape3D();
        areaCollisionShape.Shape = _collisionShape3D.Shape.Duplicate() as Shape3D;
        areaCollisionShape.Position = _collisionShape3D.Position;
        areaCollisionShape.Rotation = _collisionShape3D.Rotation;
        areaCollisionShape.Scale = _collisionShape3D.Scale;

        _collisionArea.AddChild(areaCollisionShape);
        AddChild(_collisionArea);

        _collisionArea.Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
        _collisionArea.Connect("body_exited", new Callable(this, nameof(OnBodyExited)));

        SetProcess(true);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!IsInsideTree())
        {
            return;
        }

        if (Player.Instance?.raycast != null && InterractionManager.instance.getWorldCursorPosition() != GlobalPosition)
        {
            UpdatePosition(InterractionManager.instance.getWorldCursorPosition());
        }
    }

    /// <summary>
    /// Applies a color to the MeshInstance3D.
    /// </summary>
    /// <param name="color">The color to apply.</param>
    private void ApplyColor(Color color)
    {
        if (_meshInstance != null)
        {
            var material = new StandardMaterial3D
            {
                AlbedoColor = color,
                Transparency = BaseMaterial3D.TransparencyEnum.Alpha
            };
            foreach (MeshInstance3D mesh in _meshInstance.GetChildren())
            {
                mesh.MaterialOverride = material;
            }
        }
    }

    /// <summary>
    /// Updates the position of the preview object.
    /// </summary>
    /// <param name="pos">The new position.</param>
    public void UpdatePosition(Vector3 pos)
    {
        GlobalPosition = pos;
        ValidatePos();
    }

    /// <summary>
    /// Validates the current position of the object.
    /// </summary>
    private void ValidatePos()
    {
        if (_raycast == null || !IsInsideTree())
        {
            return;
        }

        _raycast.GlobalPosition = GlobalPosition + new Vector3(0, 0.1f, 0);
        _raycast.ForceRaycastUpdate();

        bool isOnValidSurface = _raycast.IsColliding();
        bool newIsValidPos = isOnValidSurface && !IsAreaColliding();

        if (newIsValidPos != _isValidPos)
        {
            _isValidPos = newIsValidPos;
            ApplyColor(_isValidPos ? _validColor : _invalidColor);
        }
    }

    /// <summary>
    /// Checks if the Area3D is colliding with any bodies.
    /// </summary>
    /// <returns>True if colliding, otherwise false.</returns>
    private bool IsAreaColliding()
    {
        var bodies = _collisionArea.GetOverlappingBodies();
        return bodies.Count > 0;
    }

    /// <summary>
    /// Called when a body enters the Area3D.
    /// </summary>
    /// <param name="body">The body that entered.</param>
    private void OnBodyEntered(Node body)
    {
        ValidatePos();
    }
    
    /// <summary>
    /// Called when a body exits the Area3D.
    /// </summary>
    /// <param name="body">The body that exited.</param>
    private void OnBodyExited(Node body)
    {
        ValidatePos();
    }

    /// <summary>
    /// Checks if the object can be placed at the current position.
    /// </summary>
    /// <returns>True if the position is valid, otherwise false.</returns>
    public bool CanPlace()
    {
        return _isValidPos;
    }

    public void rotate(float angle)
    {
        Rotate(new Vector3(0, 1, 0), angle);
    }

    /// <summary>
    /// Destroys the preview object.
    /// </summary>
    public void Destroy()
    {
        QueueFree();
    }
}