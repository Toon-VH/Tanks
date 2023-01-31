// using System;
// using UnityEngine;
// using UnityEngine.EventSystems;
//
// namespace InputControllers
// {
//     public class MobileController : MonoBehaviour, IPointerUpHandler, IDragHandler
//     {
//         [SerializeField] private RectTransform joystickTransform;
//         [SerializeField] private float dragThreshold = 0.6f;
//         [SerializeField] private int dragMovementDistance = 30;
//         [SerializeField] private int dragOffsetDistance = 100;
//
//         public event Action<Vector2> OnMove;
//
//         private void Awake()
//         {
//             joystickTransform = (RectTransform)transform;
//         }
//
//         public void OnDrag(PointerEventData eventData)
//         {
//             Debug.Log("Drag");
//             Vector2 offset;
//             RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickTransform, eventData.position, null,
//                 out offset);
//             Debug.Log(offset);
//         }
//
//         public void OnPointerUp(PointerEventData eventData)
//         {
//             joystickTransform.anchoredPosition = Vector2.zero;
//             OnMove?.Invoke(Vector2.zero);
//         }
//
//         public void OnPointerDown(PointerEventData eventData)
//         {
//             throw new NotImplementedException();
//         }
//     }
// }