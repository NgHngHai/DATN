using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private RoomTransitioner roomTransitioner;
    [SerializeField] private Transform playerTranform;

    private string currentRoomName;
    private string pendingNextRoomName;
    private string pendingDoorLinkID;

    private void Awake()
    {
        roomTransitioner.Initialize(this);
    }

    public void LoadRoomWithTransition(string nextRoomName, Door doorEntered)
    {
        pendingNextRoomName = nextRoomName;
        pendingDoorLinkID = doorEntered.LinkID;
        roomTransitioner.PlayTransitionAnim(doorEntered.enterDirection);
    }

    public void LoadRoomWithNoTransition(string roomName)
    {
        StartCoroutine(LoadRoomRoutine(roomName));
    }

    /// <summary>
    /// Loads a new room additively, saves the current room state, restores the new room state,
    /// switches the active scene, unloads the previous room, and applies room data.
    /// </summary>
    public void StartLoadRoomRoutine()
    {
        StartCoroutine(LoadRoomRoutine(pendingNextRoomName));
    }
    private IEnumerator LoadRoomRoutine(string nextRoomName)
    {
        SaveSystem.Instance.CaptureRegisteredStates();

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(nextRoomName, LoadSceneMode.Additive);
        loadOp.allowSceneActivation = true;
        yield return loadOp;

        yield return null;

        SaveSystem.Instance.RestoreRegisteredStates();

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(nextRoomName));

        if (!string.IsNullOrEmpty(currentRoomName))
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentRoomName);
            yield return unloadOp;
        }

        currentRoomName = nextRoomName;
        pendingNextRoomName = null;
        roomTransitioner.UnfreezeAnim();

        ApplyCurrentRoomData();
    }

    /// <summary>
    /// Sets the camera boundary and teleports the player to the linked door in the new room.
    /// </summary>
    private void ApplyCurrentRoomData()
    {
        RoomData roomData = FindAnyObjectByType<RoomData>();
        if (roomData == null) return;

        foreach (CinemachineCamera cam in CameraManager.cameras)
        {
            CinemachineConfiner2D camConfiner = cam.GetComponent<CinemachineConfiner2D>();

            camConfiner.BoundingShape2D = roomData.CameraBoundary;
        }

        if (string.IsNullOrEmpty(pendingDoorLinkID)) return;

        Vector2 spawnPos = roomData.GetDoorLinkPosition(pendingDoorLinkID);

        playerTranform.position = spawnPos;
        pendingDoorLinkID = null;
    }
}
