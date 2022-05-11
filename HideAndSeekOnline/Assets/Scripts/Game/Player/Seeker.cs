using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.Game.Player
{
    public class Seeker : NetworkBehaviour
    {
        [SerializeField] private EventTrigger fireButton;

        private bool _shouldFire;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            SetupFireButtonTriggers();
        }

        private void FixedUpdate()
        {
            if (_shouldFire) HandleFire();
        }

        private void HandleFire()
        {
            fireButton.GetComponent<Image>().color = Color.red;
        }

        private void SetupFireButtonTriggers()
        {
            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            pointerDownEntry.callback.AddListener(data =>
            {
                OnFireButtonDown();
            });
            fireButton.triggers.Add(pointerDownEntry);

            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            pointerUpEntry.callback.AddListener(data =>
            {
                OnFireButtonUp();
            });
            fireButton.triggers.Add(pointerUpEntry);

            EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            pointerExitEntry.callback.AddListener(data =>
            {
                OnFireButtonExit();
            });
            fireButton.triggers.Add(pointerExitEntry);
        }

        private void OnFireButtonDown() => _shouldFire = true;
        private void OnFireButtonUp() => _shouldFire = false;
        private void OnFireButtonExit() => _shouldFire = false;
    }
}
