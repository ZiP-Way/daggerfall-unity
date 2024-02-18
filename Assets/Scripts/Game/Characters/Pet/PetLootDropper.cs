using System;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Utility;
using UnityEngine;

namespace Game.Characters.Pet
{
    [RequireComponent(typeof(PlayerHealthPercentListener))]
    public class PetLootDropper : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _droppedLootOffset;

        [SerializeField, HideInInspector]
        private CharacterMotor _motor;

        [SerializeField, HideInInspector]
        private PlayerHealthPercentListener _playerHealthPercentListener;

        private void OnEnable() =>
            _playerHealthPercentListener.OnReachedTargetPercent += DropHealingLoot;

        private void OnDisable() =>
            _playerHealthPercentListener.OnReachedTargetPercent -= DropHealingLoot;

        private void DropHealingLoot()
        {
            DaggerfallUnityItem healingPotion = GetHealingPotion();
            DaggerfallLoot loot = GetLootContainer();

            loot.Items.AddItem(healingPotion);

            DaggerfallUI.MessageBox("Pet drop you a healing potion");
            DaggerfallUI.Instance.PlayOneShot(SoundClips.MakePotion);
        }

        private DaggerfallLoot GetLootContainer()
        {
            GameObject playerObject = GameManager.Instance.PlayerObject;
            Vector3 lootPosition = GetLootPosition();

            return GameObjectHelper.CreateDroppedLootContainer(playerObject, DaggerfallUnity.NextUID, at: lootPosition);
        }

        private Vector3 GetLootPosition() =>
            _motor.FindGroundPosition() + _droppedLootOffset;

        private DaggerfallUnityItem GetHealingPotion() =>
            ItemBuilder.CreatePotion(PotionRecipe.classicRecipeKeys[2]);

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_playerHealthPercentListener == null)
                TryGetComponent(out _playerHealthPercentListener);

            if (_motor == null)
                TryGetComponent(out _motor);

        }
#endif
    }
}