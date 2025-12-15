using UnityEngine;

public class Checkpoint : Interactables
{
    [Header("Checkpoint Settings")]
    [SerializeField] private string _checkpointRoomName;

    protected override void Awake()
    {
        base.Awake();
        //_checkpointRoomData = FindFirstObjectByType<RoomData>();
    }

    protected override void OnInteract(GameObject player)
    {
        var saveable = player.GetComponent<PlayerSaveables>();
        var health = player.GetComponent<Health>();

        if (saveable == null || health == null)
            return;

        saveable.lastCheckpointRoomName = _checkpointRoomName;

        health.RestoreToFull();
    }
}
