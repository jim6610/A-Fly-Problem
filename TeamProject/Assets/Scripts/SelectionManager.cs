using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private string selectableTag = "Throwable";
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;

    public float grabDistance = 5f;

    private Transform _selection;
    public Transform hand;
    public Camera fpsCam;
    public Transform heldObjectRef;


    public float throwForce = 1f;

    private bool isHolding;
    private float throwCooldown = 1f;
    private float throwTimer;

    // Update is called once per frame
    void Update()
    {
        if (_selection != null)
        {
            var selectionRenderer = _selection.GetComponent<Renderer>();
            selectionRenderer.material = defaultMaterial;
            _selection = null;
        }

        var ray = fpsCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Throw();

        throwTimer += Time.deltaTime;

        if (!isHolding && throwTimer > throwCooldown)
        {
            if (Physics.Raycast(ray, out hit, grabDistance))
            {
                var selection = hit.transform;

                if (selection.CompareTag(selectableTag))
                {
                    var selectionRenderer = selection.GetComponent<Renderer>();

                    if (selectionRenderer != null)
                    {
                        selectionRenderer.material = highlightMaterial;
                    }

                    _selection = selection;


                    if (!isHolding && Input.GetKeyDown("e"))
                    {
                        selection.GetComponent<Rigidbody>().useGravity = false;
                        selection.GetComponent<Rigidbody>().freezeRotation = true;
                        selection.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                        selection.position = hand.transform.position;
                        selection.parent = hand.transform;

                        heldObjectRef = selection;

                        selection.GetComponent<Collider>().enabled = false;

                        isHolding = true;
                    }
                }
            }
        }
    }

    void Throw()
    {
        if (isHolding && Input.GetKeyDown("e"))
        {
            heldObjectRef.GetComponent<Rigidbody>().useGravity = true;
            heldObjectRef.GetComponent<Collider>().enabled = true;
            heldObjectRef.GetComponent<Rigidbody>().freezeRotation = false;
            heldObjectRef.GetComponent<Rigidbody>().AddForce(fpsCam.transform.forward * throwForce, ForceMode.Impulse);

            heldObjectRef.parent = null;
            isHolding = false;
            heldObjectRef = null;

            throwTimer = 0;
        }
    }
}