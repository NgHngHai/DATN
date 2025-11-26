using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public string currentRoom;

    // References
    private RoomManager _roomManager;
    private Collider2D _collider2D;
    [SerializeField] private PlayerController _playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _roomManager = GameObject.FindGameObjectWithTag("Room Manager").GetComponent<RoomManager>();
        _playerController = _roomManager.playerController;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            if (_playerController.currentRoomID == currentRoom) 
                return; // Already in the target room
            
            _roomManager.SwitchCurrentRoom(currentRoom);
            _playerController.currentRoomID = currentRoom;
        }
    }
}
