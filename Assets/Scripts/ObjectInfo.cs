using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObjectInfo : MonoBehaviour
{
    public TaskList task;

    public ResourceManager RM;

    GameObject targetNode;

    public NodeManager.ResourceTypes heldResourceType;

    public bool isSelected = false;

    public string objectName;

    private NavMeshAgent agent;

    public bool isGathering;

    public int heldResource;

    public int maxHeldResource;

    public GameObject[] drops =  new GameObject[20];
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GatherTick());
        agent = GetComponent<NavMeshAgent>();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && isSelected)
        {
            RightClick();
        }
        if (targetNode == null)
        {
            if(heldResource != 0)
            {
                drops = GameObject.FindGameObjectsWithTag("Drops");
                agent.destination = GetClosestDropOff(drops).transform.position;
                drops = null;
                task = TaskList.Delivering;
            }
            else
            {
                task = TaskList.Idle;
                
            }
        }
        if (heldResource >= maxHeldResource && isGathering)
        {
            //drop off point here
            drops = GameObject.FindGameObjectsWithTag("Drops");
            agent.destination = GetClosestDropOff(drops).transform.position;
            drops = null;
            task = TaskList.Delivering;
        }
        
    }


    GameObject GetClosestDropOff(GameObject[] dropOffs)
    {
        GameObject closestDrop = null;
        float closestDistance = Mathf.Infinity;
        Vector3 position = transform.position;
        
        foreach(GameObject targetDrop in dropOffs)
        {
            Vector3 direction = targetDrop.transform.position - position;
            float distance = direction.sqrMagnitude;

            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestDrop = targetDrop;

            }

        }

        return closestDrop;

    }


    public void RightClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                agent.destination = hit.point;
                task = TaskList.Moving;
            }
            else if (hit.collider.CompareTag("Resource"))
            {
                agent.destination = hit.collider.gameObject.transform.position;
                Debug.Log("Harvesting");
                task = TaskList.Gathering;
                targetNode = hit.collider.gameObject;
                isSelected = false;
            }
        }
    }


    public void OnTriggerEnter(Collider other)
    {
        GameObject hitObject = other.gameObject;
        if (hitObject.CompareTag("Resource") && task == TaskList.Gathering)
        {
            isGathering = true;
            hitObject.GetComponent<NodeManager>().gatherers++;
            heldResourceType = hitObject.GetComponent<NodeManager>().resourceType;
            
        }else if(hitObject.CompareTag("Drops") && task == TaskList.Delivering)
        {
            if(RM.Gold >= RM.maxGold)
            {
                task = TaskList.Idle;

            }
            else
            {
                RM.Gold += heldResource;
                heldResource = 0;
                task = TaskList.Gathering;
                agent.destination = targetNode.transform.position;

            }
        }
    }


    public void OnTriggerExit(Collider other)
    {
        GameObject hitObject = other.gameObject;

        if (hitObject.CompareTag( "Resource"))
        {
            hitObject.GetComponent<NodeManager>().gatherers--;
            isGathering = false;
        }
    }


    IEnumerator GatherTick()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (isGathering)
            {
                heldResource++;
            }
            
        }

    }
}