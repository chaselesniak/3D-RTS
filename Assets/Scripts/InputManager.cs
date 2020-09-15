using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float panSpeed;
    public float rotateSpeed;
    public float rotateAmount;
    private Quaternion rotation;

    private float pandDetect = 15f;
    private float minHeight = 10f;
    private float maxHeight = 100f;
    public GameObject selectedObject;
    private ObjectInfo selectedInfo;
    // Start is called before the first frame update
    void Start()
    {
        rotation = Camera.main.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
        RotateCamera();
        if(Input.GetMouseButtonDown(0))
        {
            LeftClick();
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Camera.main.transform.rotation = rotation;
        }
    }


    public void LeftClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit,100))
        {
            if(hit.collider.tag == "Ground")
            {
                if (selectedInfo != null && selectedInfo.isSelected)
                {
                    selectedInfo.isSelected = false;
                    selectedObject = null;
                }
               
               Debug.Log("Deselected");
            }
            else if(hit.collider.tag == "Selectable")
            {
                selectedObject = hit.collider.gameObject;
                selectedInfo = selectedObject.GetComponent<ObjectInfo>();
                selectedInfo.isSelected = true;
                Debug.Log("selected");

            }
        }

    }


    void MoveCamera()
    {
        float moveX = Camera.main.transform.position.x;
        float moveZ = Camera.main.transform.position.z;
        float moveY = Camera.main.transform.position.y;
        float xPos = Input.mousePosition.x;
        float yPos = Input.mousePosition.y;

        if(Input.GetKey(KeyCode.A)|| xPos > 0 && xPos < pandDetect)
        {
            moveX -= panSpeed;
            
        }
        else if (Input.GetKey(KeyCode.D) || xPos < Screen.width && xPos > Screen.width-pandDetect)
        {
            moveX += panSpeed;

        }
         if (Input.GetKey(KeyCode.W) || yPos < Screen.height && yPos > Screen.height - pandDetect)
        {
            moveZ += panSpeed;

        }
        else if (Input.GetKey(KeyCode.S) || yPos > 0 && yPos < pandDetect)
        {
            moveZ -= panSpeed;

        }
        moveY -= Input.GetAxis("Mouse ScrollWheel")* (panSpeed *20);
        moveY = Mathf.Clamp(moveY, minHeight, maxHeight);

        Vector3 newPos = new Vector3(moveX, moveY, moveZ);
        Camera.main.transform.position = newPos;
    }


    void RotateCamera()
    {
        Vector3 origin = Camera.main.transform.eulerAngles;
        Vector3 destination = origin;
        
        if(Input.GetMouseButton(2))
        {
            destination.x -= Input.GetAxis("Mouse Y") * rotateAmount;
            destination.y += Input.GetAxis("Mouse X") * rotateAmount;
        }
        if(destination != origin)
        {
            Camera.main.transform.eulerAngles = Vector3.MoveTowards(origin, destination, Time.deltaTime * rotateSpeed);
        }
    }
}
