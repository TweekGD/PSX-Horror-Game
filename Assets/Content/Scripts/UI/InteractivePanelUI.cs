//using UnityEngine;

//public class InteractivePanelUI : MonoBehaviour
//{
//    [SerializeField] private PlayerInteract playerInteract;
//    [SerializeField] private float fadeSpeed = 1f;
//    [SerializeField] private CanvasGroup canvasGroup;
//    public bool InteractivePanelIsActive { get; private set; }

//    private void Update()
//    {
//        ToggleInteractivePanel(playerInteract.OnInteractebleCollider);
//    }
//    public void ToggleInteractivePanel(bool active)
//    {
//        InteractivePanelIsActive = active;

//        canvasGroup.alpha += InteractivePanelIsActive ? Time.deltaTime * fadeSpeed : -Time.deltaTime * fadeSpeed;
//    }
//}
