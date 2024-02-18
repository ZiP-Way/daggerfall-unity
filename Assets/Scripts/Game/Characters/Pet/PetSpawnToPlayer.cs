using DaggerfallWorkshop.Game;
using UnityEngine;

namespace Game.Characters.Pet
{
    public class PetSpawnToPlayer : MonoBehaviour
    {
        [SerializeField]
        private float _maxDistance;

        private void LateUpdate()
        {
            if (IsPetFarFromPlayer())
                SpawnPetToPlayer();
        }

        private bool IsPetFarFromPlayer()
        {
            Vector3 playerPosition = GameManager.Instance.PlayerObject.transform.position;
            return Vector3.Distance(transform.position, playerPosition) > _maxDistance;
        }

        private void SpawnPetToPlayer()
        {
            Vector3 playerPosition = GameManager.Instance.PlayerObject.transform.position;
            transform.position = playerPosition;
        }
    }
}