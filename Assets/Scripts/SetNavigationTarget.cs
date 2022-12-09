using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;

public class SetNavigationTarget : MonoBehaviour
{

    [SerializeField]
    private List<RoomTargets> RoomObjects = new List<RoomTargets>();

    [SerializeField]
    private Slider NavigationLineSliderYaxis;
   

    [SerializeField]
    private TMP_Dropdown RoomDropdown;


    [SerializeField]
    private Camera topDownCamera;
    

    private Vector3 Roomtargetposition = Vector3.zero;
    private NavMeshPath path;
    private LineRenderer line;

    private bool lineToggle = false;


    private void Start()
    {
        path = new NavMeshPath();
        line = transform.GetComponent<LineRenderer>();
    }

    //

    private void Update()
    {
        if (Roomtargetposition != Vector3.zero)
        {
            NavMesh.CalculatePath(transform.position, Roomtargetposition, NavMesh.AllAreas, path);
            line.positionCount = path.corners.Length;
            line.SetPositions(path.corners);
            line.enabled = true;

            //
            Vector3[] linepositionY = CustomizelineY();
            line.SetPositions(linepositionY);

            

        }

    }


    public void RoomTargetSet(int dropdownvalue)
    {
        Roomtargetposition = Vector3.zero;
        string SelectedRoom = RoomDropdown.options[dropdownvalue].text;
        RoomTargets CurrentRoom = RoomObjects.Find(x => x.roomname.Equals(SelectedRoom));
        if (CurrentRoom != null)
        {
            Roomtargetposition = CurrentRoom.roomobject.transform.position;
        }

    }

    private Vector3[] CustomizelineY()
    {
       
        Vector3[] calculatedLine = new Vector3[path.corners.Length];
        for (int i = 0; i < path.corners.Length; i++)
        {
            calculatedLine[i] = path.corners[i] + new Vector3(0, NavigationLineSliderYaxis.value, 0);


        }
        return calculatedLine;

    }

    





}