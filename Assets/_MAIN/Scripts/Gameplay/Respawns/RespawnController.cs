using System.Collections;
using System.Collections.Generic;
using MOVEMENT;
using UnityEngine;
using UnityEngine.Diagnostics;

public class RespawnController : MonoBehaviour
{
    private Transform currentCheckpoint;
    [SerializeField] private PlayerController player;

    public void SetCheckpoint(Transform newCheckpoint) => currentCheckpoint = newCheckpoint;


}
