using UnityEngine;
using System.Collections;
using System;

public class DiskController : MonoBehaviour
{
    //Initialize Variables

    /// <summary>
    /// Speed time to move back on the top of a stick (lower = faster)
    /// </summary>
    [Tooltip("Specify the speed of movement for the disk (lower = faster)")]
    public float moveTime = 0.03f;

    [Tooltip("Specify the size of the disk")]
    public ushort Size = 0;

    // Keep a reference of the sick where the disk is currently piled
    public StickController currentStick;

    //Used to make movement more efficient.
    float _inverseMoveTime;          

    // set true when start dragging a disk
    bool _isMouseDragging = false;
    // Used to set the initial position when dragging a disk
    Vector3 _offsetValue;

    // Store the initial position when sstart dragging a disk
    Vector2 _initialPosition;

    // Various component of this gameObject
    Rigidbody2D _rigidBody;
    Collider2D _collider;
    SpriteRenderer _renderer;

    // Sorting layer names used for showing dragged disk on top of other
    string _layerName;    
    string _draggingLayerName = "DraggedDisk";

    // Hold the reference of the stick currently overed by the disk
    StickController _overedStick;
    
    // Fields for the shaking animation
    Vector3 shakeOriginPosition;
    Quaternion shakeOriginRotation;
    float temp_shake_intensity = 0;
    float shakeDecay = 0.01f;
    float shakeIntensity = 0.2f;

    /// <summary>
    /// Raised when a disk was successfully dropped
    /// </summary>
    public event EventHandler DroppedSuccess;

    /// <summary>
    /// Raised when a disk was dopped but without success
    /// </summary>
    public event EventHandler DroppedFail;

    protected virtual void OnDropped(bool success)
    {
        EventHandler handler = null;
        if (success)
        {
            handler = DroppedSuccess;
        }
        else
        {
            handler = DroppedFail;
        }

        if (handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }

    void Start()
    {
        // Get Disk components
        _rigidBody = gameObject.GetComponent<Rigidbody2D>();
        _renderer = gameObject.GetComponent<SpriteRenderer>();
        _collider = gameObject.GetComponent<Collider2D>();

        // Save the layer name for this disk
        _layerName = _renderer.sortingLayerName;

        // Don't raycast on the trigger zones of the sticks
        Physics2D.queriesHitTriggers = false; 

        // We disable rotation so that the disk will not do any weird roation wile dragged or dropped
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;

        //By storing the reciprocal of the move time we can use it by multiplying instead of dividing, this is more efficient.
        if (moveTime <= 0) moveTime = 1f;
        _inverseMoveTime = 1f / moveTime;
    }

    void Update()
    {
        // Shaking animations using rotations and randomized position
        if (temp_shake_intensity > 0)
        {
            transform.position = shakeOriginPosition + UnityEngine.Random.insideUnitSphere * temp_shake_intensity;
            transform.rotation = new Quaternion(
                shakeOriginRotation.x + UnityEngine.Random.Range(-temp_shake_intensity, temp_shake_intensity) * .2f,
                shakeOriginRotation.y + UnityEngine.Random.Range(-temp_shake_intensity, temp_shake_intensity) * .2f,
                shakeOriginRotation.z + UnityEngine.Random.Range(-temp_shake_intensity, temp_shake_intensity) * .2f,
                shakeOriginRotation.w + UnityEngine.Random.Range(-temp_shake_intensity, temp_shake_intensity) * .2f);
            temp_shake_intensity -= shakeDecay;
        }
    }


    #region Drag&Drop

    //Mouse Button Press Down -> Start dragging
    void OnMouseDown()
    {
        // Can only move the first disk on the stick and only if it is resting
        if (CanDrag())
        {
            // Start dragging
            _isMouseDragging = true;

            //Converting world position to screen position.
            _offsetValue = transform.position - Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));

            // Set collider as trigger to not interfer with other disk while dragging
            _collider.isTrigger = true;

            // Set the sort layer to the last so it will be in front of any other disks while dragging
            _renderer.sortingLayerName = _draggingLayerName;

            // Save intial position in case the disk should be moved back
            _initialPosition = transform.position;
        }
    }

    //Is mouse Moving -> Continue dragging
    void OnMouseDrag()
    {
        if (_isMouseDragging)
        {
            //tracking mouse position.
            Vector2 currentScreenSpace = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            //converting screen position to world position with offset changes.
            Vector2 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenSpace) + _offsetValue;

            //It will update target gameobject's current postion.
            transform.position = currentPosition;
        }
    }

    // Mouse Button Up -> Stop dragging and drop the disk
    void OnMouseUp()
    {
        if (_isMouseDragging)
        {
            Vector2 end = Vector2.zero;

            // Restore sorting layer
            _renderer.sortingLayerName = _layerName;

            // Restore a no-trigger collider, we need this to trigger the Enter/Exit of a stick + falling animation
            _collider.isTrigger = false;

            // Stop dragging
            _isMouseDragging = false;

            // Restore opacity on the stick disks when releasing the mouse
            if (_overedStick != null)
            {
                _overedStick.SetDisksTransparent(false, this);
            }

            // The disk just got dropped on a stick
            if (_overedStick != null && _overedStick.CanAddDisk(this))
            {
                if (IsCurrentStick(_overedStick))
                {
                    // Dropping the disk on the same stick, animate the movement towards the initial position
                    end = _initialPosition;

                    OnDropped(false);
                }
                else
                {
                    // Aligh stick to the center of the stick and disable further dragging
                    end = GetEndMovementPosition(_overedStick.GetComponent<Collider2D>());

                    // Remove the disk from the previous stick
                    currentStick.RemoveTopDisk();

                    // Add the disk to the new stick
                    _overedStick.AddDisk(this);

                    // Store the stick on which the disk has been dropped as backup
                    //currentStick = _stickElement;

                    // Raise event for dropping
                    OnDropped(true);
                }
            }
            else
            {
                // Disk was dropped outside a stick or on a smaller disk
                end = _initialPosition;

                OnDropped(false);
            }

            // Start movement of the disk on the top of the designed stick
            StartCoroutine(DropDisk(end));
        }
    }

    private IEnumerator DropDisk(Vector3 end)
    {
        // Disable the disk collider so that when moving towards the stick it won't trigger or collide with something else
        _collider.enabled = false;

        //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
        //Square magnitude is used instead of magnitude because it's computationally cheaper.
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        //While that distance is greater than a very small amount (Epsilon, almost zero):
        while (sqrRemainingDistance > 0.01)
        {
            //Find a new position proportionally closer to the end, based on the moveTime
            Vector3 newPostion = Vector3.MoveTowards(_rigidBody.position, end, _inverseMoveTime * Time.deltaTime);

            //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
            _rigidBody.MovePosition(newPostion);

            //Recalculate the remaining distance after moving.
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            if (_isMouseDragging) break;

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }

        // Re-enable collider
        _collider.enabled = true;

        // Give a disk a little shake on the top of the stick :)
        Shake();

        // ------- Here Physics engine will do the rest, will make the disk fall in the stick -------
    }
    #endregion

    #region StickTrigger
    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsAStick(other.gameObject))
        {
            _overedStick = other.GetComponent<StickController>();

            // Trigger a laywer of transparency on the disks of the hovered stick (and it is not the stick the disk is on)
            if (_isMouseDragging && !IsCurrentStick(_overedStick))
            {
                other.GetComponent<StickController>().SetDisksTransparent(true, this);
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (IsAStick(other.gameObject))
        {
            _overedStick = other.GetComponent<StickController>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (IsAStick(other.gameObject))
        {
            // Restore opacity on the disks of the hovered stick (and it is not the stick the disk is on)
            if (_isMouseDragging && !IsCurrentStick(other.GetComponent<StickController>()))
            {
                other.GetComponent<StickController>().SetDisksTransparent(false, this);
            }

            _overedStick = null;
        }
    } 
    #endregion

    #region Helpers
    private bool IsAStick(GameObject obj)
    {
        // Check if a game object is Stick type
        return obj.GetComponent<StickController>() != null;
    }

    private bool IsCurrentStick(StickController other)
    {
        // Check if a stick is the same has the one on which this disk is piled
        return currentStick.name == other.name;
    }

    private bool CanDrag()
    {
        /* Can drag disk if 
         * it's on the top of the stick
         * it's not moving
         * game is not over
         * game is not paused
        */
        return currentStick.GetTopDisk().name == this.name &&
                //_rigidBody.IsSleeping() &&
                !GameManager.instance.gameOver &&
                !GameManager.instance.gamePaused;
    }

    private void Shake()
    {
        // Set values for starting the shaking in the Update method
        shakeOriginPosition = transform.position;
        shakeOriginRotation = transform.rotation;
        temp_shake_intensity = shakeIntensity;
    }

    private Vector2 GetEndMovementPosition(Collider2D coll)
    {
        return coll.bounds.center + new Vector3(0, coll.bounds.extents.y);
    }
    #endregion
}

