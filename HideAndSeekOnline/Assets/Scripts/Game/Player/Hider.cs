using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Game.Player
{
    public class Hider : NetworkBehaviour
    {
        [SerializeField] private EventTrigger rotateBodyButton;

        [SerializeField] private Transform body;
        
        private bool _shouldRotateBody;
        private const float RotationSpeed = 100f;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            SetupRotateBodyButtonTriggers();
        }

        private void FixedUpdate()
        {
            if (_shouldRotateBody) HandleBodyRotation();
        }

        private void HandleBodyRotation()
        {
            body.transform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
        }

        private void SetupRotateBodyButtonTriggers()
        {
            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            pointerDownEntry.callback.AddListener(data =>
            {
                OnRotateBodyButtonDown();
            });
            rotateBodyButton.triggers.Add(pointerDownEntry);
            
            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            pointerUpEntry.callback.AddListener(data =>
            {
                OnRotateBodyButtonUp();
            });
            rotateBodyButton.triggers.Add(pointerUpEntry);
            
            EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            pointerExitEntry.callback.AddListener(data =>
            {
                OnRotateBodyButtonExit();
            });
            rotateBodyButton.triggers.Add(pointerExitEntry);
        }

        private void OnRotateBodyButtonDown() => _shouldRotateBody = true;
        private void OnRotateBodyButtonUp() => _shouldRotateBody = false;
        private void OnRotateBodyButtonExit() => _shouldRotateBody = false;
    }
}
