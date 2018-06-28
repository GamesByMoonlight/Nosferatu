using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Teams { neutral, good, bad } // The neutral option is for items that don't lean either way.

[CreateAssetMenu (fileName = "NewAttributesObject", menuName = "AttributesObject")]
public class AttributesObject : ScriptableObject
{
    public Attributes attributes;

    // Use when this Attribute set defines starting values
    public void Initialize(Attributes attr)
    {
        attr.Attack = attributes.Attack;
        attr.MaxHealth = attributes.MaxHealth;
        attr.Team = attributes.Team;
        attr.ProjectileRange = attributes.ProjectileRange;
        attr.ProjectileSpeed = attributes.ProjectileSpeed;
        attr.MovementSpeed = attributes.MovementSpeed;
    }

    // Use when this Attribute set defines a change to existing values
    public void Modify(Attributes attr)
    {
        attr.Attack *= attributes.Attack;
        attr.MaxHealth *= attributes.MaxHealth;
        attr.Team = attributes.Team;
        attr.ProjectileRange *= attributes.ProjectileRange;
        attr.ProjectileSpeed *= attributes.ProjectileSpeed;
        attr.MovementSpeed *= attributes.MovementSpeed;
    }

    // Use to remove changes made in Modify()
    public void UnModify(Attributes attr)
    {
        attr.Attack /= attributes.Attack;
        attr.MaxHealth /= attributes.MaxHealth;
        attr.RevertToPreviousTeam();
        attr.ProjectileRange /= attributes.ProjectileRange;
        attr.ProjectileSpeed /= attributes.ProjectileSpeed;
        attr.MovementSpeed /= attributes.MovementSpeed;
    }
}


/// <summary>
/// Contains attributes that characters and items on the map possess.
/// Items on the map pass attributes via the AttributesObject ScriptableObjects
/// </summary>
[System.Serializable]
public class Attributes
{
    private Teams previousTeam;
    private Teams currentTeam;

    public float Attack;
    public float MaxHealth;
    public Teams Team { get { return currentTeam; } set { previousTeam = currentTeam; currentTeam = value; } }
    public float ProjectileRange;
    public float ProjectileSpeed;
    public float MovementSpeed;
    //Add special ability
    //Add magic

    public void RevertToPreviousTeam()
    {
        var old = currentTeam;
        currentTeam = previousTeam;
        previousTeam = old;
    }
}
