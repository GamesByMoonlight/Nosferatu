using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Teams { neutral, good, bad } // The neutral option is for items that don't lean either way.
public enum PlayerClass { Vampire, Heavy, Light }  // Refactor to match player classes once we decide on player types

[CreateAssetMenu (fileName = "NewAttributesObject", menuName = "AttributesObject")]
public class AttributesObject : ScriptableObject
{
    public Attributes attributes;

    // Use when this Attribute set defines starting values
    public void Initialize(Attributes attr)
    {
        attr.Team = attributes.Team;
        attr.Name = attributes.Name;
        attr.MaxHealth = attributes.MaxHealth;
        attr.CurrentHealth = attributes.CurrentHealth;
        attr.Attack = attributes.Attack;
        attr.ProjectileSpeed = attributes.ProjectileSpeed;
        attr.FireRate = attributes.FireRate;
        attr.ProjectileRange = attributes.ProjectileRange;
        attr.ForwardSpeed = attributes.ForwardSpeed;
        attr.BackwardSpeed = attributes.BackwardSpeed;
        attr.StrafeSpeed = attributes.StrafeSpeed;
}

    // Use when this Attribute set defines a change to existing values
    public void Modify(Attributes attr)
    {
        attr.Team = attributes.Team;
        attr.MaxHealth *= attributes.MaxHealth;
        attr.CurrentHealth *= attributes.CurrentHealth;
        attr.Attack *= attributes.Attack;
        attr.ProjectileSpeed *= attributes.ProjectileSpeed;
        attr.FireRate *= attributes.FireRate;
        attr.ProjectileRange *= attributes.ProjectileRange;
        attr.ForwardSpeed *= attributes.ForwardSpeed;
        attr.BackwardSpeed *= attributes.BackwardSpeed;
        attr.StrafeSpeed *= attributes.StrafeSpeed;
    }

    // Use to remove changes made in Modify()
    public void UnModify(Attributes attr)
    {
        attr.RevertToPreviousTeam();
        attr.MaxHealth /= attributes.MaxHealth;
        attr.CurrentHealth /= attributes.CurrentHealth;
        attr.Attack /= attributes.Attack;
        attr.ProjectileSpeed /= attributes.ProjectileSpeed;
        attr.FireRate /= attributes.FireRate;
        attr.ProjectileRange /= attributes.ProjectileRange;
        attr.ForwardSpeed /= attributes.ForwardSpeed;
        attr.BackwardSpeed /= attributes.BackwardSpeed;
        attr.StrafeSpeed /= attributes.StrafeSpeed;
    }
}


/// <summary>
/// Contains attributes that characters and items on the map possess.
/// Items on the map pass attributes via the AttributesObject ScriptableObjects
/// </summary>
[System.Serializable]
public class Attributes
{
    public string Name;

    [SerializeField]
    private Teams currentTeam;
    private Teams previousTeam;
    public Teams Team { get { return currentTeam; } set { previousTeam = currentTeam; currentTeam = value; } }
    
    public float MaxHealth;
    public float CurrentHealth;
    public float Attack;
    public float ProjectileSpeed;
    public float FireRate;
    public float ProjectileRange;
    public float ForwardSpeed;
    public float BackwardSpeed;
    public float StrafeSpeed;
    //Add special ability
    //Add magic

    public void RevertToPreviousTeam()
    {
        var old = currentTeam;
        currentTeam = previousTeam;
        previousTeam = old;
    }
}
