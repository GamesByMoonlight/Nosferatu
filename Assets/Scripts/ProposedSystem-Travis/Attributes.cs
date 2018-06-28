using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Teams { neutral, good, bad} //The neutral option is for items that don't lean either way.


/// <summary>
/// Contains attributes that characters and items on the map possess.
/// Items on the map pass attributes via the AttributesObject ScriptableObjects
/// </summary>
[System.Serializable]
public class Attributes
{
    public int attack;
    public int health;
    public Teams team;
    public float projectileRange;
    public float projectileSpeed;
    public float movementSpeed;
    //Add special ability
    //Add magic
}
