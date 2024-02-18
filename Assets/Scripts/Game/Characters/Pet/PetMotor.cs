
using UnityEngine;

namespace Game.Characters.Pet
{
    public class PetMotor : CharacterMotor
    {
        [SerializeField]
        private float _stopDistance;

        protected override float GetStopDistance() =>
            _stopDistance;
    }
}