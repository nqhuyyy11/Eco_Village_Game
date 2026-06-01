using UnityEngine;
using UnityEngine.Events;

namespace EcoVillage.Core.Interaction
{
    [RequireComponent(typeof(Collider2D))]
    public class TriggerZone : MonoBehaviour
    {
        [Header("Filter Settings")]
        [Tooltip("If checked, only objects with the 'Player' tag will trigger this zone.")]
        [SerializeField] private bool onlyTriggerByPlayer = true;
        [Tooltip("The tag required to trigger if onlyTriggerByPlayer is false.")]
        [SerializeField] private string targetTag = "Player";

        [Header("Trigger 2D Events")]
        [Tooltip("Fired when a matching 2D object enters the trigger.")]
        public UnityEvent<Collider2D> onTriggerEnter;
        
        [Tooltip("Fired when a matching 2D object stays inside the trigger.")]
        public UnityEvent<Collider2D> onTriggerStay;

        [Tooltip("Fired when a matching 2D object exits the trigger.")]
        public UnityEvent<Collider2D> onTriggerExit;

        private void Reset()
        {
            // Ensure the 2D collider is set as trigger automatically
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsValidTriggerTarget(other))
            {
                onTriggerEnter?.Invoke(other);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (IsValidTriggerTarget(other))
            {
                onTriggerStay?.Invoke(other);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (IsValidTriggerTarget(other))
            {
                onTriggerExit?.Invoke(other);
            }
        }

        private bool IsValidTriggerTarget(Collider2D other)
        {
            if (onlyTriggerByPlayer)
            {
                return other.CompareTag("Player");
            }
            
            return other.CompareTag(targetTag);
        }
    }
}
