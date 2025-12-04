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
            roomManager.LoadRoomScene(nextRoomName, linkID);
        }
    }

    public string LinkID => linkID;
}

