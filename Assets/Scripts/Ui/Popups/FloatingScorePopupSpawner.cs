using UnityEngine;
using DarkTonic.PoolBoss;

namespace FunClicker.UI
{
    public class FloatingScorePopupSpawner : MonoBehaviour
    {
        [Header("PoolBoss")]
        [SerializeField] private Transform popupPrefab;

        [Header("UI")]
        [SerializeField] private RectTransform popupContainer;

        public void SpawnAtScreenPosition(Vector2 screenPosition, Camera uiCamera, string textValue)
        {
            if (popupPrefab == null)
            {
                Debug.LogWarning("Popup Prefab chưa được gán.");
                return;
            }

            if (popupContainer == null)
            {
                Debug.LogWarning("Popup Container chưa được gán.");
                return;
            }

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    popupContainer,
                    screenPosition,
                    uiCamera,
                    out Vector2 localPoint))
            {
                return;
            }

            Transform spawned = PoolBoss.Spawn(
                popupPrefab,
                popupContainer.position,
                Quaternion.identity,
                popupContainer
            );

            if (spawned == null)
            {
                Debug.LogWarning("PoolBoss spawn trả về null.");
                return;
            }

            RectTransform popupRect = spawned as RectTransform;
            if (popupRect == null)
            {
                Debug.LogWarning("Popup spawn ra không phải RectTransform.");
                spawned.Despawn();
                return;
            }

            // Ép anchor/pivot về center để anchoredPosition dùng đúng localPoint
            popupRect.anchorMin = new Vector2(0.5f, 0.5f);
            popupRect.anchorMax = new Vector2(0.5f, 0.5f);
            popupRect.pivot = new Vector2(0.5f, 0.5f);

            popupRect.anchoredPosition = localPoint;
            popupRect.localRotation = Quaternion.identity;
            popupRect.localScale = Vector3.one;

            var popup = spawned.GetComponent<FloatingScorePopup>();
            if (popup == null)
            {
                Debug.LogWarning("Prefab popup chưa có FloatingScorePopup.");
                spawned.Despawn();
                return;
            }

            popup.Play(textValue, localPoint);
        }
    }
}