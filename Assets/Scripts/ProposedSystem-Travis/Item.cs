using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ItemState { none, placing, idle, activated, exhausted, destroyed} //I just made these up and haven't used them yet. Just an example.

/// <summary>
/// Parent class that all items can inherit from. Treasure, traps, minion spawners, etc.
/// </summary>
public class Item : MonoBehaviour
{
    [SerializeField] private AttributesObject attributesObject;
    protected Attributes attributes = new Attributes(); // { get {return attributesObject.attributes; } }

    private ItemState state;

    protected virtual void Awake()
    {
        attributesObject.Initialize(attributes);
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        
    }

    protected virtual void OnMouseDown()
    {
        
    }

    protected virtual void OnMouseUp()
    {
        
    }

    protected virtual void OnMouseEnter()
    {
        HighlightObject(true);
    }

    protected virtual void OnMouseExit()
    {
        HighlightObject(false);
    }

    protected virtual void HighlightObject(bool on)
    {

    }

}
