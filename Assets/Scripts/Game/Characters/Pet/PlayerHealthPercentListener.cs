using System;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using UnityEngine;

namespace Game.Characters.Pet
{
    public class PlayerHealthPercentListener : MonoBehaviour
    {
        [SerializeField, Range(0, 1)]
        private float _targetPercent = 0.25f;

        [SerializeField]
        private bool _isOnlyOnceInvoke;

        #region Actions

        public event Action OnReachedTargetPercent;
        private bool _isAlreadyInvoked;

        #endregion

        #region Fields

        private DaggerfallEntity _playerEntity;

        #endregion

        private void Awake() =>
            _playerEntity = GameManager.Instance.PlayerEntity;

        private void OnEnable() =>
            _playerEntity.OnHealthChanged += HealthChanged;

        private void OnDisable() =>
            _playerEntity.OnHealthChanged -= HealthChanged;

        private void HealthChanged()
        {
            if(_isAlreadyInvoked && _isOnlyOnceInvoke)
                return;

            if (_playerEntity.CurrentHealthPercent <= _targetPercent)
            {
                _isAlreadyInvoked = true;
                OnReachedTargetPercent?.Invoke();
            }
        }
    }
}