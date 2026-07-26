using UnityEngine;


public class GearController : MonoBehaviour
{
    public static GearController currentGear;
    public GearType gearType;
    public float gearForce = 1;
    public Vector3 rotationSpeed = new Vector3(0, 0, 10);

    [Header("Gravity")]

    public GravityDirectionType gravityType;

    [Range(0,360)]

    public float customAngle;

    void OnTriggerEnter2D(
        Collider2D other
    )
    {
        if(!other.CompareTag("Player"))
            return;

        currentGear = this;

        PlayerController.canGear = true;
    }

    void OnTriggerExit2D(
        Collider2D other
    )
    {
        if(!other.CompareTag("Player"))
            return;

        if(currentGear == this)
        {
            currentGear = null;
        }

        PlayerController.canGear = false;
    }
    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }

    public Vector2 EndGravityVector
    {
        get
        {
            switch(gravityType)
            {
                case GravityDirectionType.Down:
    
                    return Vector2.down;
    
                case GravityDirectionType.Up:
    
                    return Vector2.up;
    
                case GravityDirectionType.Left:
    
                    return Vector2.left;
    
                case GravityDirectionType.Right:
    
                    return Vector2.right;
    
                case GravityDirectionType.DownRight:
    
                    return new Vector2(1,-1).normalized;
    
                case GravityDirectionType.DownLeft:
    
                    return new Vector2(-1,-1).normalized;
    
                case GravityDirectionType.UpRight:
    
                    return new Vector2(1,1).normalized;
    
                case GravityDirectionType.UpLeft:
    
                    return new Vector2(-1,1).normalized;
    
                case GravityDirectionType.Custom:
    
                    float radians =
                        customAngle * Mathf.Deg2Rad;
    
                    return new Vector2(
                    
                        Mathf.Cos(radians),
    
                        Mathf.Sin(radians)
    
                    ).normalized;
            }
    
            return Vector2.down;
        }
    }
}
