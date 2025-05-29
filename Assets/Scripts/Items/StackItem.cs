using Godot;
using System;

/// <summary>
/// Représente un item dans une pile.
/// Contient des informations sur l'item et son nombre dans une pile.
/// </summary>
[GlobalClass] 
public partial class StackItem: Resource
{
    // Champs privés
    private ItemStatic _resource;
    private int _stack=0;
    public event Action<StackItem> OnStackModified;
    public event Action<StackItem> OnItemChanged;
    [Export]
    public ItemStatic Resource
    {
        get => _resource;
        set
        {
            if (_resource != value)
            {
                _resource = value;
                OnItemChanged?.Invoke(this);
            }
        }
    }

    /// <summary>
    /// Obtient ou définit le nombre d'items dans la pile.
    /// </summary>
    [Export]
    public int Stack
    {
        get => _stack;
        set
        {
            if (_stack != value)
            {
                _stack = value;
                OnStackModified?.Invoke(this);
            }
        }
    }
    
    /// <summary>
    /// Obtient la ressource associée à cet item.
    /// </summary>
    public ItemStatic getResource()
    {
        return _resource;
    }
    /// <summary>
    /// Définit la ressource associée à cet item.
    /// </summary>
    public void setResource(ItemStatic value)
    {
        _resource = value;
    }

    /// <summary>
    /// Obtient le nombre d'items dans la pile.
    /// </summary>
    public int getStack()
    {
        return _stack;
    }
    /// <summary>
    /// Définit le nombre d'items dans la pile.
    /// </summary>
    public void setStack(int value)
    {
        Stack = value;
        
    }

    /// <summary>
    /// Constructeur par défaut.
    /// Nécessaire pour l'initialisation.
    /// </summary>
    public StackItem() {}

    /// <summary>
    /// Initialise une nouvelle instance de _stackItem.
    /// </summary>
    /// <param name="resource">La ressource statique associée à l'item.</param>
    /// <param name="nb">Le nombre initial d'items dans la pile (par défaut : 1).</param>
    public StackItem(ItemStatic resource, int nb = 1)
    {
        this._resource = resource;
        Stack = nb;
    }

    /// <summary>
    /// Ajoute un certain nombre d'items à la pile.
    /// </summary>
    /// <param name="nb">Le nombre d'items à ajouter.</param>
    /// <returns>
    /// Le nombre d'items restant non ajoutés si la pile atteint sa capacité maximale.
    /// </returns>
    public int add(int nb)
    {
        if (_stack + nb <= _resource.getMaxStack)
        {
            Stack += nb;
            return 0;
        }
            int overflow = _stack + nb - _resource.getMaxStack;
            Stack = _resource.getMaxStack;
            return overflow;
        
    }

    public bool canAdd(StackItem item)
    {
        if (item == null)
            return false;
        
        return _stack + item.getStack() <= _resource.getMaxStack;
    }
    /// <summary>
    /// Soustrait un certain nombre d'items de la pile.
    /// </summary>
    /// <param name="nb">Le nombre d'items à retirer.</param>
    public void subtract(int nb)
    {
        Stack = Math.Max(0, _stack - nb);
    }

    /// <summary>
    /// Vérifie si la pile est vide.
    /// </summary>
    /// <returns>True si la pile est vide, sinon False.</returns>
    public bool isEmpty()
    {
        return _stack <= 0;
    }

    /// <summary>
    /// Divise la pile actuelle en deux parties.
    /// </summary>
    /// <returns>
    /// Une nouvelle instance de _stackItem contenant la moitié des items,
    /// ou null si la pile ne peut pas être divisée.
    /// </returns>
    public StackItem split()
    {
        if (_stack <= 1) 
            return null;

        int splitAmount = _stack / 2;
        Stack -= splitAmount;

        return new StackItem(_resource, splitAmount);
    }
    
    public override String ToString()
    {
        return "\n item : " + _resource.ToString() + "\n stack : " + _stack;
    }
}
