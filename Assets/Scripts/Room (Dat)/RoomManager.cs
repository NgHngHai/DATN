using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TestPersistentScene
{
    public class RoomManager : MonoBehaviour
    {
        [SerializeField] private Transform playerTranform;
        [SerializeField] private CinemachineConfiner2D cinemachineConfiner;

        private string currentRoomName;
        private string pendingDoorLinkID;

        private void Start()
        {
            SaveSystem.Instance.CreateNewGameData();
        }

        public void LoadRoomScene(string nextRoomName, string doorLinkID = null)
        {
            pendingDoorLinkID = doorLinkID;
            StartCoroutine(LoadRoomRoutine(nextRoomName));
        }

        private IEnumerator LoadRoomRoutine(string nextRoomName)
        {
            SaveSystem.Instance.CaptureRegisteredStates();

            AsyncOperation loadOp = SceneManager.LoadSceneAsync(nextRoomName, LoadSceneMode.Additive);
            loadOp.allowSceneActivation = true;
            yield return loadOp;

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(nextRoomName));

            if (!string.IsNullOrEmpty(currentRoomName))
            {
                AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentRoomName);
                yield return unloadOp;
            }

            currentRoomName = nextRoomName;

            SaveSystem.Instance.RestoreRegisteredStates();

            ApplyCurrentRoomData();
        }

        private void ApplyCurrentRoomData()
        {
            RoomData roomData = FindAnyObjectByType<RoomData>();
            if (roomData == null) return;

            cinemachineConfiner.BoundingShape2D = roomData.CameraBoundary;

            if (string.IsNullOrEmpty(pendingDoorLinkID)) return;

            Vector2 spawnPos = roomData.GetDoorLinkPosition(pendingDoorLinkID);

            playerTranform.position = spawnPos;
            pendingDoorLinkID = null;
        }
    }
}
