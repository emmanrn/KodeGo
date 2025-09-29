using UnityEngine;

namespace PLAYER
{
    public class PlayerInteraction : MonoBehaviour
    {
        private InteractionDetector interaction;

        private void Awake() =>
            interaction = GetComponentInChildren<InteractionDetector>();

        public void OnInteract() =>
            interaction.interactableRange?.Interact();
    }
}
