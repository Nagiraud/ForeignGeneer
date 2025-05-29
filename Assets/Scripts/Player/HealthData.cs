using Godot;
using System;

public class HealthData
{
    public float Health { get; private set; } = 100f;
    public float Hunger { get; private set; } = 100f;
    public float Pollution { get; private set; } = 20f;

    public const float MAXHEALTH = 100f;
    public const float MINHEALTH = 0f;
    public const float MAXHUNGER = 100f;
    public const float MINHUNGER = 0f;

    public event Action HealthChanged;
    public event Action HungerChanged;
    public event Action PollutionChanged;


    public void modifyHealth(float healthPoint)
    {
        Health = Mathf.Clamp(Health + healthPoint, MINHEALTH, MAXHEALTH);
        GD.Print(healthPoint, Health, "this is hungermodify");
        HealthChanged?.Invoke();
    }

    public void modifyHunger(float hungerPoint)
    {
        Hunger = Mathf.Clamp(Hunger + hungerPoint, MINHUNGER, MAXHUNGER);
        GD.Print(hungerPoint, Hunger, "this is hungermodify");
        HungerChanged?.Invoke();
    }

    public void modifyPollution(float pollutionPoint)
    {
        Pollution += pollutionPoint;
        PollutionChanged?.Invoke();
    }
}