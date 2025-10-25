using UnityEngine;

public struct AirStepData
{
    public Vector2 airStepDirection;
    public float powerFactor;

    public AirStepData(Vector2 airStepDirection, float powerFactor) : this()
    {
        this.airStepDirection = airStepDirection;
        this.powerFactor = powerFactor;
    }
}