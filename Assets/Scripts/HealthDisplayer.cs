using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UnitController))]
public class HealthDisplayer : MonoBehaviour
{
    private UnitController unitController;
    private TextMesh textMesh;
    private int lastHealth = -1;

    // Use this for initialization
	void Start ()
	{
	    unitController = GetComponent<UnitController>();
	    textMesh = GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (lastHealth != unitController.unitData.healthPoints)
	    {
	        lastHealth = unitController.unitData.healthPoints;

	        string value;
	        if (lastHealth > 0)
	        {
	            value = Convert.ToString(unitController.unitData.healthPoints);
	        }
	        else
	        {
	            value = "";
	        }

	        textMesh.text = value;
	    }

	}
}
