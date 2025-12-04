using System.Collections.Generic;
using UnityEngine;

namespace TestPersistentScene
{
    public class RoomData : MonoBehaviour
    {
        public Collider2D CameraBoundary;
        [SerializeField] private List<Door> doorList = new List<Door>();

        private Dictionary<string, Door> doorDict;

        private void Awake()
        {
            doorDict = new Dictionary<string, Door>();

            foreach (Door door in doorList)
            {
                if (!doorDict.ContainsKey(door.LinkID))
                    doorDict.Add(door.LinkID, door);
            }
        }

        public Vector2 GetDoorLinkPosition(string linkID)
        {
            if (doorDict.TryGetValue(linkID, out Door door))
            {
                return door.transform.position;
            }

            return Vector2.zero;
        }
    }
}
