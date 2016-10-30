using UnityEngine;

public class AttackPatternRenderer : MonoBehaviour
{

    private Position lastPosition = new Position(-1, -1);

    public void SetPattern(Position tileDataPosition, Transform transform1, AttackPatternDefinition definitionAttackPattern)
    {
        if (!tileDataPosition.Equals(lastPosition))
        {
            lastPosition = tileDataPosition;

            if (transform.childCount > 0)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Destroy(transform.GetChild(i));
                }
                transform.DetachChildren();
            }


            int xOffset = definitionAttackPattern.Width / 2;
            int yOffset = definitionAttackPattern.Height / 2;

            for (int x=0; x < definitionAttackPattern.Width; x++)
            {
                for (int y = 0; y < definitionAttackPattern.Height; y++)
                {
                    var attackStrength = definitionAttackPattern.GetData((uint) x, (uint) y);
                    var divisor = 0.6f / definitionAttackPattern.GetMax();
                    if (attackStrength > 0)
                    {
                        //create a square and color it according to attack strength
                        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                        quad.transform.position = new Vector3(x - xOffset, y - yOffset);
                        var material = quad.GetComponent<Renderer>().material;
                        material.color = new Color(1f, 0f, 0f, attackStrength * divisor);
                        material.shader = Shader.Find("Transparent/Diffuse");
                        quad.transform.parent = gameObject.transform;
                    }
                }
            }

            transform.position = new Vector3(transform1.position.x, transform1.position.y, transform.position.z);
        }
    }

    public void HidePattern(Position tileDataPosition, AttackPatternDefinition definitionAttackPattern)
    {
        if (tileDataPosition.Equals(lastPosition))
        {
            lastPosition = new Position(-1, -1);
            if (transform.childCount > 0)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Destroy(transform.GetChild(i));
                }
                transform.DetachChildren();
            }
        }
    }
}
