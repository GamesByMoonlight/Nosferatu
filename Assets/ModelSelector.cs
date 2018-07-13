using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelSelector : MonoBehaviour {

    public GameObject[] models;

	public void ChooseModel(PlayerClass playerClass)
    {
        Debug.Log("Model chosen is " + playerClass);

        GameObject myModel = Instantiate(models[(int)playerClass], transform, false);

        myModel.transform.parent = transform;


        // Minor tweaks to player models and collider positions so that things look/feel correct.  Otherwise the models feet go through the floor for some reason
        switch (playerClass)
        {
            case PlayerClass.Vampire:
                CapsuleCollider CC = myModel.transform.parent.GetComponentInParent<CapsuleCollider>();
                CC.height = 1.6f;
                myModel.transform.localPosition = new Vector3(0f, 0.33f, 0f);
                return;
            case PlayerClass.Wizard:
                myModel.transform.localPosition = new Vector3(0f, 0.24f, 0f);
                return;
            case PlayerClass.Archer:
                myModel.transform.localPosition = new Vector3(0f, 0.25f, 0f);
                return;
        }
    }

}
