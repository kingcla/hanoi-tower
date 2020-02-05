using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Support script to handle repetion of scrolling sprites 
/// </summary>
public class RepeatingBG : MonoBehaviour
{
    /// <summary>
    /// Scrolling speed
    /// </summary>
    [Tooltip("Scrolling speed")]
    public float scrollSpeed = 1.2f;

    // This stores a reference to the collider attached to the Background.
    private BoxCollider2D backgroundCollider;
    // A float to store the y-axis length of the collider2D attached to the Background GameObject.
    private float backgroundHorizontalLength;         
    
    // Components holders
    private Rigidbody2D _rigidBody;

    // Use this for initialization
    void Start()
    {
        //Get and store a reference to the Rigidbody2D attached to this GameObject.
        _rigidBody = GetComponent<Rigidbody2D>();

        //Start the object moving.
        _rigidBody.velocity = new Vector2(scrollSpeed, 0);
    }

    // Awake is called before Start.
    private void Awake()
    {
        //Get and store a reference to the collider2D attached to Background.
        backgroundCollider = GetComponent<BoxCollider2D>();

        //Store the size of the collider along the x axis (its length in units).
        backgroundHorizontalLength = backgroundCollider.size.x;
    }

    // Update runs once per frame
    private void Update()
    {
        
        //Check if the difference along the x axis between the main Camera and the position of the object this is attached to is greater than groundHorizontalLength.
        if (transform.position.x < -backgroundHorizontalLength)
        {
            //If true, this means this object is no longer visible and we can safely move it forward to be re-used.
            RepositionBackground();
        }
    }

    // Moves the object this script is attached to right in order to create our looping background effect.
    private void RepositionBackground()
    {
        //This is how far to the top we will move our background object, in this case, twice its length. This will position it directly to the right of the currently visible background object.
        Vector2 groundOffSet = new Vector2(backgroundHorizontalLength * 2f, 0);

        //Move this object from it's position offscreen to the new position off-camera in front of the camera.
        transform.position = (Vector2)transform.position + groundOffSet;
    }
}
