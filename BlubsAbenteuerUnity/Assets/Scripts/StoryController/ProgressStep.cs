using System.Collections.Generic;
using UnityEngine;

// base class for progress steps
public class ProgressStep : ScriptableObject
{
    private bool completed;
    public bool Completed
    {
        get
        {
            return completed;
        }

        set
        {
            completed = value;
        }
    }

    [Header("Preconditions")]
    [SerializeField] private List<ProgressStep> proconditions;
    public List<ProgressStep> Preconditions
    {
        get
        {
            return proconditions;
        }
    }

    [Header("Room output")]
    [SerializeField] private bool hasRoomOutput;
    [SerializeField] private OutputContainer roomOutput;
    public bool HasRoomOutput(out OutputContainer output)
    {
        output = roomOutput;
        return hasRoomOutput;
    }
}