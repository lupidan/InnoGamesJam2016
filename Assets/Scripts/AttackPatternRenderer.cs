using UnityEngine;
using System.Collections;

public class AttackPatternRenderer : MonoBehaviour
{

    public AttackPatternDefinition attackPattern;

	// Use this for initialization
	void Start ()
	{
	    int xOffset = attackPattern.Width / 2;
	    int yOffset = attackPattern.Height / 2;

	    for (int x=0; x < attackPattern.Width; x++)
	    {
	        for (int y = 0; y < attackPattern.Height; y++)
	        {
	            var attackStrength = attackPattern.GetData((uint) x, (uint) y);
	            if (attackStrength > 0)
	            {
	                //create a square and color it according to attack strength
	                GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
	                quad.transform.position = new Vector3(x - xOffset, y - yOffset);
	                var material = quad.GetComponent<Renderer>().material;
	                material.color = new Color(255.0f, 0.0f, 0.0f, 0.5f*attackStrength/10.0f);
	                material.shader = Shader.Find("Transparent/Diffuse");
	                quad.transform.parent = gameObject.transform;
	            }
	        }
	    }
	}

	// Update is called once per frame
	void Update () {

	}
}
