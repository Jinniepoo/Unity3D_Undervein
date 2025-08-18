using System;

public enum AttributeType
{
    Agility,
    Intellect,
    Stamina,
    Strength,
    Speed,
    Health,
    Mana,
}

[Serializable]
public class Attribute
{
    public AttributeType type;
    public ModifiableInt value;
}
