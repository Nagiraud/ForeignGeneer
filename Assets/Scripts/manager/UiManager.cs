using System;
using Godot;
using System.Collections.Generic;
using ForeignGeneer.Assets.Scripts;
using ForeignGeneer.Assets.Scripts.Interface;

public partial class UiManager : Node
{
	public static UiManager instance { get; private set; }

	// Tableaux exportables pour les clés et les PackedScene
	[Export] private Godot.Collections.Array<string> _uiKeys = new Godot.Collections.Array<string>();
	[Export] private Godot.Collections.Array<PackedScene> _uiScenes = new Godot.Collections.Array<PackedScene>();

	// Dictionnaire pour stocker les UI ouvertes (nom de la scène -> instance)
	public Dictionary<string, Control> _openUis = new Dictionary<string, Control>();

	// Dictionnaire pour stocker les scènes UI chargées (nom de la scène -> PackedScene)
	private Dictionary<string, PackedScene> _loadedUiScenes = new Dictionary<string, PackedScene>();

	public event Action<bool> onUiStateChanged;

	public override void _Ready()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			instance = this;
		}

		// Convertit les tableaux exportés en dictionnaire
		for (int i = 0; i < _uiKeys.Count; i++)
		{
			if (i < _uiScenes.Count)
			{
				_loadedUiScenes[_uiKeys[i]] = _uiScenes[i];
			}
		}
	}

	/// <summary>
	/// Ouvre une UI par son nom.
	/// </summary>
	public void openUi(string uiName, Object data = null)
	{
		if (_openUis.ContainsKey(uiName))
		{
			return;
		}
		if (_loadedUiScenes.TryGetValue(uiName, out var uiScene))
		{
			var uiInstance = uiScene.Instantiate<Control>();
			AddChild(uiInstance, true);

			BaseUi baseUi = uiInstance as BaseUi;
			baseUi?.initialize(data);

			_openUis[uiName] = uiInstance;
			Input.MouseMode = Input.MouseModeEnum.Visible;
			onUiStateChanged?.Invoke(true);

		}
	}

	/// <summary>
	/// Ferme une UI spécifique ou toutes les UI si aucun paramètre n'est fourni.
	/// </summary>
	public void closeUi(string uiName = null)
	{
		if (string.IsNullOrEmpty(uiName))
		{
			foreach (var uiEntry in _openUis)
			{
				DetachIfObserver(uiEntry.Value);
				uiEntry.Value.QueueFree();
			}
			_openUis.Clear();
			Input.MouseMode = Input.MouseModeEnum.Captured;
			onUiStateChanged?.Invoke(false);
		}
		else
		{
			if (_openUis.ContainsKey(uiName))
			{
				DetachIfObserver(_openUis[uiName]);

				_openUis[uiName].QueueFree();
				_openUis.Remove(uiName);

				if (_openUis.Count == 0)
				{
					onUiStateChanged?.Invoke(false);
				}
			}
		}
	}

	/// <summary>
	/// Vérifie si l'objet implémente IObserver et appelle detach() si c'est le cas.
	/// </summary>
	private void DetachIfObserver(Node uiNode)
	{
		if (uiNode is IObserver observer)
		{
			observer.detach();
		}
	}


	/// <summary>
	/// Vérifie si une UI est ouverte.
	/// </summary>
	public bool isUiOpen(string uiName)
	{
		return _openUis.ContainsKey(uiName);
	}

	/// <summary>
	/// Vérifie si au moins une UI est ouverte.
	/// </summary>
	public bool isAnyUiOpen()
	{
		return _openUis.Count > 0;
	}

	/// <summary>
	/// Récupère une UI ouverte par son nom.
	/// </summary>
	public Control getUi(string uiName)
	{
		return _openUis.GetValueOrDefault(uiName);
	}

	/// <summary>
	/// Actualise l'interface utilisateur actuellement ouverte.
	/// </summary>
	public void refreshCurrentUi(Node data = null)
	{
		// Si aucune UI n'est ouverte, on ne fait rien
		if (_openUis.Count == 0)
		{
			return;
		}

		// Rafraîchit toutes les UI ouvertes
		foreach (var uiEntry in _openUis)
		{
			var baseUi = uiEntry.Value as BaseUi;
			baseUi?.update();
		}
	}
	/// <summary>
	/// Actualise l'interface utilisateur donnée en paramètre
	/// </summary>
	public void refreshUi(string name, Node data = null)
	{
		if (_openUis.Count == 0 || _openUis.ContainsKey(name))
		{
			return;
		}
		BaseUi ui = _openUis[name] as BaseUi;
		ui?.update();
	}
}
