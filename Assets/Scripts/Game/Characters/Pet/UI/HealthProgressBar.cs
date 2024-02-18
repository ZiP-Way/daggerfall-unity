using System;
using DaggerfallWorkshop.Game.Entity;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Characters.Pet.UI
{
    public class HealthProgressBar : MonoBehaviour
    {
        [SerializeField]
        private Image _indicator;

        [SerializeField]
        private DaggerfallEntityBehaviour _entityBehaviour;

        private void OnEnable()
        {
            if (_entityBehaviour.Entity == null)
                _entityBehaviour.OnEntityAssigned += EntityInitialized;
            else
                _entityBehaviour.Entity.OnHealthChanged += UpdateProgressBar;
        }

        private void Start() =>
            UpdateProgressBar();

        private void OnDisable()
        {
            _entityBehaviour.OnEntityAssigned -= EntityInitialized;
            _entityBehaviour.Entity.OnHealthChanged -= UpdateProgressBar;
        }

        private void EntityInitialized()
        {
            _entityBehaviour.OnEntityAssigned -= EntityInitialized;
            _entityBehaviour.Entity.OnHealthChanged += UpdateProgressBar;
        }

        private void UpdateProgressBar() =>
            _indicator.fillAmount = _entityBehaviour.Entity.CurrentHealthPercent;
    }
}