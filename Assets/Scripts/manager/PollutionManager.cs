using Godot;

namespace ForeignGeneer.Assets.Scripts.manager;

public class PollutionManager : IObservable
{
	private static PollutionManager instance { get; set; }

	private float _pollution = 0;
	
	private PollutionManager()
	{
	}

	public static PollutionManager getInstance()
	{
		return instance ?? (instance = new PollutionManager());
	}
	public void addPolution(float pollution)
	{
		_pollution += pollution;
	}

	public void removePolution(float pollution)
	{
		_pollution -= pollution;
	}

	public float getPollution()
	{
		return _pollution;
	}

	public bool hasPollution(float pollution)
	{
		return _pollution >= pollution;
	}

	public void attach(IObserver observer)
	{
		
	}

	public void detach(IObserver observer)
	{
		
	}

	public void notify(InterfaceType? interfaceType = null)
	{
		
	}
}
