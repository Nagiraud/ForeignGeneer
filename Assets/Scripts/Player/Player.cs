using ForeignGeneer.Assets.Scripts.Block;
using ForeignGeneer.Assets.Scripts.manager;
using Godot;

public partial class Player : CharacterBody3D
{
    #region Attributs

    /// <summary>
    /// Instance statique du joueur. Garantit qu'il n'y a qu'une seule instance du joueur dans le jeu.
    /// </summary>
    public static Player Instance { get; private set; }

    /// <summary>
    /// Vitesse du joueur. Défini par défaut à 5.0f.
    /// </summary>
    [Export] public float Speed = 5.0f;

    /// <summary>
    /// Valeur utilisée pour l'interpolation des mouvements du joueur. Utilisée pour rendre les mouvements plus fluides.
    /// </summary>
    public const float LerpVal = 0.5f;

    /// <summary>
    /// Indicateur du mode sprint du joueur.
    /// </summary>
    private bool _isSprinting = false;

    /// <summary>
    /// Référence à l'armature du joueur, utilisée pour la rotation du modèle 3D.
    /// </summary>
    private Node3D _armature;

    /// <summary>
    /// Référence au pivot de la caméra, utilisé pour contrôler la rotation de la caméra.
    /// </summary>
    private Node3D _pivot;

    /// <summary>
    /// Référence au bras de printemps qui maintient la caméra à une certaine distance du joueur.
    /// </summary>
    private SpringArm3D _springArm;

    /// <summary>
    /// Référence à l'arbre d'animation du joueur, qui gère les transitions d'animations.
    /// </summary>
    private AnimationTree _animTree;

    /// <summary>
    /// Direction du mouvement du joueur en 3D.
    /// </summary>
    private Vector3 _movementDirection = Vector3.Zero;

    /// <summary>
    /// Indicateur qui détermine si l'interface utilisateur est ouverte.
    /// </summary>
    private bool _isUiOpen = false;

    /// <summary>
    /// Référence au Raycast pour les interactions.
    /// </summary>
    public RayCast3D raycast;

    /// <summary>
    /// Indicateur du mode de vue (FPS ou TPS).
    /// </summary>
    private bool _isFirstPerson = false;

    #endregion

    #region Méthodes

    /// <summary>
    /// Méthode appelée lorsque l'objet Player est prêt dans la scène. 
    /// Initialise les nœuds nécessaires et s'abonne aux événements de changement d'état de l'UI.
    /// </summary>
    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            QueueFree(); // Assure qu'il n'y a qu'une seule instance du joueur.
        }

        _armature = GetNode<Node3D>("Armature_001");
        _pivot = GetNode<Node3D>("Pivot");
        _springArm = GetNode<SpringArm3D>("Pivot/SpringArm3D");
        _animTree = GetNode<AnimationTree>("AnimationTree");
        raycast = GetNode<RayCast3D>("Pivot/SpringArm3D/Camera3D/RayCast3D");
        Input.MouseMode = Input.MouseModeEnum.Captured; // Capture le curseur de la souris.
        UiManager.instance.onUiStateChanged += OnUiStateChanged; // S'abonne aux changements d'état de l'UI.

        // Initialise la caméra en mode TPS par défaut
        SetFirstPersonMode(false);
    }

    /// <summary>
    /// Méthode appelée lorsque l'état de l'UI change.
    /// Change le mode de la souris en fonction de l'état de l'UI.
    /// </summary>
    private void OnUiStateChanged(bool isUiOpen)
    {
        _isUiOpen = isUiOpen;
        Input.MouseMode = _isUiOpen ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
    }

    /// <summary>
    /// Active ou désactive le sprint du joueur.
    /// </summary>
    public void SetSprinting()
    {
        _isSprinting = !_isSprinting;
        Speed = _isSprinting ? 20.0f : 5.0f;
    }

    /// <summary>
    /// Permet au joueur de sauter s'il est sur le sol.
    /// </summary>
    public void Jump()
    {
        if (IsOnFloor())
        {
            Velocity = new Vector3(Velocity.X, 5.0f, Velocity.Z); // Applique une vélocité verticale.
        }
    }

    /// <summary>
    /// Gestion du clic gauche du joueur.
    /// </summary>
    public void LeftClick()
    {
        if (_isUiOpen) return;
        StackItem item = InventoryManager.Instance.getCurrentItem();
        if (item is not null)
            item.getResource().LeftClick();
    }

    /// <summary>
    /// Gestion du clic droit du joueur.
    /// </summary>
    public void RightClick()
    {
        if (_isUiOpen) return;
        StackItem item = InventoryManager.Instance.getCurrentItem();
        if (item != null)
            item.getResource().RightClick();
    }

    /// <summary>
    /// Gère la rotation de la caméra en fonction des déplacements de la souris.
    /// </summary>
    public void RotateCamera(Vector2 mouseDelta)
    {
        if (_isUiOpen) return; // Ignore la rotation si l'UI est ouverte.

        _pivot.RotateY(-mouseDelta.X * 0.005f); // Rotation horizontale.
        _springArm.RotateX(-mouseDelta.Y * 0.005f); // Rotation verticale.

        Vector3 springArmRotation = _springArm.Rotation;
        springArmRotation.X = Mathf.Clamp(springArmRotation.X, -Mathf.Pi / 3, Mathf.Pi / 3); // Limite la rotation verticale.
        _springArm.Rotation = springArmRotation;
    }

    /// <summary>
    /// Permet au joueur de se déplacer selon les entrées du clavier.
    /// </summary>
    public void Move(Vector2 inputDir)
    {
        if (_isUiOpen) return; // Ignore les mouvements si l'UI est ouverte.

        _movementDirection = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        _movementDirection = _movementDirection.Rotated(Vector3.Up, _pivot.Rotation.Y); // Applique la rotation de la caméra à la direction.
    }

    /// <summary>
    /// Arrête le mouvement du joueur.
    /// </summary>
    public void StopMoving()
    {
        _movementDirection = Vector3.Zero; // Arrête le mouvement.
    }

    /// <summary>
    /// Méthode appelée à chaque frame physique pour appliquer la gravité, le mouvement et les animations.
    /// </summary>
    public override void _PhysicsProcess(double delta)
    {
        if (_isUiOpen) return; 

        if (!IsOnFloor())
        {
            Velocity += GetGravity() * (float)delta; 
        }

        if (_movementDirection != Vector3.Zero)
        {
            Velocity = new Vector3(
                Mathf.Lerp(Velocity.X, _movementDirection.X * Speed, LerpVal),
                Velocity.Y,
                Mathf.Lerp(Velocity.Z, _movementDirection.Z * Speed, LerpVal)
            );

            float targetRotationY = Mathf.Atan2(-Velocity.X, -Velocity.Z);
            _armature.Rotation = new Vector3(
                _armature.Rotation.X,
                Mathf.LerpAngle(_armature.Rotation.Y, targetRotationY, LerpVal),
                _armature.Rotation.Z
            );
        }
        else
        {
            Velocity = new Vector3(
                Mathf.Lerp(Velocity.X, 0.0f, LerpVal),
                Velocity.Y,
                Mathf.Lerp(Velocity.Z, 0.0f, LerpVal)
            );
        }

        if (_animTree != null)
        {
            float blendPosition = Velocity.Length() / Speed;
            _animTree.Set("parameters/BlendSpace1D/blend_position", blendPosition);
        }

        MoveAndSlide(); // Applique le mouvement au joueur.
    }

    /// <summary>
    /// Méthode appelée lors de la suppression de l'objet de la scène.
    /// Désabonne les événements de l'UI.
    /// </summary>
    public override void _ExitTree()
    {
        UiManager.instance.onUiStateChanged -= OnUiStateChanged;
    }

    /// <summary>
    /// Bascule entre le mode FPS et TPS.
    /// </summary>
    public void ToggleViewMode()
    {
        SetFirstPersonMode(!_isFirstPerson);
    }

    /// <summary>
    /// Définit le mode de vue (FPS ou TPS).
    /// </summary>
    /// <param name="isFirstPerson">True pour le mode FPS, False pour le mode TPS.</param>
    private void SetFirstPersonMode(bool isFirstPerson)
    {
        _isFirstPerson = isFirstPerson;
        
        if (_isFirstPerson)
        {
            Visible = false;
            _springArm.SpringLength = -0.5f;
        }
        else
        {
            Visible = true;
            _springArm.SpringLength = 1.5f; 
        }
    }

    #endregion
}