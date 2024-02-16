using UnityEngine;

namespace Game.Characters.Pet.UI
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasLookAtCamera : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private Canvas _canvas;

        #region Fields

        private Camera _camera;

        #endregion

        private void Awake()
        {
            _camera = Camera.main;
            _canvas.worldCamera = _camera;
        }

        private void LateUpdate() =>
            transform.LookAt(_camera.transform);

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_canvas == null)
                TryGetComponent(out _canvas);
        }
#endif
    }
}