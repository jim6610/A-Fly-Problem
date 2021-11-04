using UnityEngine;

/// Handles the highlighting of objects within a players range and the grabbing and throwing of objects
/// TODO The game logic of higlighting objects and throwing/picking up should be split in different classes
public class SelectionManager : MonoBehaviour
{
    [SerializeField] private string selectableTag = "Destructible";
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private float grabDistance = 5f;
    [SerializeField] private float throwForce = 1f;
    
    [SerializeField] private Transform hand;
    [SerializeField] private Camera fpsCam;
    [SerializeField] private WeaponSwitching weaponManager;
    
    private Transform heldObjectRef;
    private Transform _selection;
    
    private bool isHolding;
    private float throwCooldown = 1f;
    private float throwTimer;
    
    /// Logic Lambdas to make our conditionals less cluttered
    private bool CanThrow => !isHolding && throwTimer > throwCooldown;
    private bool ShouldThrow => isHolding && Input.GetKeyDown(KeyCode.E);
    private bool ShouldPickUp => !isHolding && Input.GetKeyDown(KeyCode.E);

    void Update()
    {
        RemoveSelection();
        ThrowHandler();
        SelectionHandler();
    }
    
    /// Fires a ray at the center of the screen with a given range to see if an object should be highlighted (Grabbable)
    private void SelectionHandler()
    {
        var ray = fpsCam.ScreenPointToRay(Input.mousePosition);
        
        if (CanThrow && Physics.Raycast(ray, out var hit, grabDistance))
        {
            Transform selection = hit.transform;

            if (selection.tag == "Destructible")
            {
                selection.GetComponent<Outline>().enabled = true;
                _selection = selection;
                Pickup(selection);
            }
        }
    }
    
    /// If something is currently select deselect it and reset its material
    private void RemoveSelection()
    {
        if (_selection != null)
        {
            _selection.GetComponent<Outline>().enabled = false;
            _selection = null;
        }
    }
    
    /// Throw whatever object the player is currently holding
    private void ThrowHandler()
    {
        throwTimer += Time.deltaTime;
        
        if (!ShouldThrow) 
            return;
        
        ToggleObject(heldObjectRef, true);
        
        heldObjectRef.GetComponent<Rigidbody>().AddForce(fpsCam.transform.forward * throwForce, ForceMode.Impulse);

        heldObjectRef.parent = null;
        isHolding = false;
        heldObjectRef = null;

        throwTimer = 0;

        weaponManager.EquipPreviousWeapon();
    }
    
    /// "Picks up" a gameobject and places it at the carry location of the player
    /// <param name="selection">The transform of the eligible object</param>
    private void Pickup(Transform selection)
    {
        if (!ShouldPickUp) 
            return;
        
        ToggleObject(selection, false);

        selection.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

        var handTransform = hand.transform;
        
        selection.position = handTransform.position;
        selection.parent = handTransform;

        heldObjectRef = selection;
        isHolding = true;

        weaponManager.PutAwayCurrentWeapon();
    }
    
    /// Toggle between a "frozen" (picked up) state and thrown
    /// When disabled, removes gravity, disables collider and freezes the objects rotation
    /// <param name="objectTransform">Transform to be toggle</param>
    /// <param name="enabled">The new state of the object</param>
    private void ToggleObject(Transform objectTransform, bool enabled)
    {
        objectTransform.GetComponent<Rigidbody>().useGravity = enabled;
        objectTransform.GetComponent<Collider>().enabled = enabled;
        objectTransform.GetComponent<Rigidbody>().freezeRotation = !enabled;
    }
}