using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Example item just to give an idea of what I was thinking for these.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class ExampleItem : Item
{
    public Color highlightEmissionColor;

    private Color normalEmissionColor;
    private Material material;

    protected override void Awake()
    {
        base.Awake();
        material = GetComponent<Renderer>().material;
        normalEmissionColor = material.GetColor("_EmissionColor");
    }

    protected override void HighlightObject(bool on)
    {
        base.HighlightObject(on);
        if (on)
            material.SetColor("_EmissionColor", highlightEmissionColor);
        else
            material.SetColor("_EmissionColor", normalEmissionColor);
    }

}
