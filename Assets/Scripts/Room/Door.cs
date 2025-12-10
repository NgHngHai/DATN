using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private string linkID = "1-2-L";
    [SerializeField] private string nextRoomName;

    private RoomManager roomManager;

    private void Awake()
    {
        roomManager = FindAnyObjectByType<RoomManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            // Change player's current room & linked door ID
            var playerSaveables = collision.GetComponent<PlayerSaveables>();
            playerSaveables.playerRoomID = nextRoomName;
            playerSaveables.playerLinkDoorID = linkID;

            // Trigger room switch overlay

            // Load next room
            roomManager.LoadRoomScene(nextRoomName, linkID);
        }
    }

    public string LinkID => linkID;
}

