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
    private Vector3 spinVector;

    protected override void Awake()
    {
        base.Awake();
        material = GetComponent<Renderer>().material;
        normalEmissionColor = material.GetColor("_EmissionColor");
    }

    protected override void Start()
    {
        base.Start();
        transform.localScale = new Vector3(attributes.Attack / 10f, attributes.Attack / 10f, attributes.Attack / 10f);
        spinVector = 
            attributes.Team == Teams.neutral ? new Vector3(1f, 0f, 0f) : 
            attributes.Team == Teams.good ? new Vector3(0f, 1f, 0f) : 
            new Vector3(0f, 0f, 1f);
        // spin speed will simply be attributes.MovementSpeed value

        StartCoroutine(Spin());
    }

    IEnumerator Spin()
    {
        while(true)
        {
            yield return new WaitForFixedUpdate();
            transform.Rotate(spinVector, attributes.ForwardSpeed);
        }
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
