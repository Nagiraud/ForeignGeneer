using System.Collections.Generic;
using Godot;

namespace ForeignGeneer.Assets.Scripts.manager;

public class EnergyManager : IObservable
{
	private static EnergyManager instance { get; set; }
	private List<IObserver> _observers = new List<IObserver>();
	private EnergyManager()
	{
		instance = this;
	}
	public static EnergyManager getInstance()
	{
		return instance ?? (instance = new EnergyManager());
	}
	private float _globalElectricity = 100;
	
	public float getGlobalElectricity()
	{
		return _globalElectricity;
	}

	public void addGlobalElectricity(float value)
	{
		_globalElectricity += value;
		notify(InterfaceType.Energy);
	}
	public void removeGlobalElectricity(float value)
	{
		_globalElectricity -= value;
		notify(InterfaceType.Energy);
	}

	public bool hasEnergy(float value)
	{
		return _globalElectricity - value >= 0;
	}

	public bool isDown()
	{
		return _globalElectricity <= 0;
	}

	public void attach(IObserver observer)
	{
		if (!_observers.Contains(observer))
		{
			_observers.Add(observer);
		}
	}

	public void detach(IObserver observer)
	{
		if (_observers.Contains(observer))
		{
			_observers.Remove(observer);
		}
	}

	public void notify(InterfaceType? interfaceType = null)
	{
		foreach (var observer in _observers)
		{
			observer.update(interfaceType);
		}
	}
}
