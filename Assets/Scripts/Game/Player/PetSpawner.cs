using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Utility;
using UnityEngine;

namespace Game.Player
{
    public class PetSpawner : MonoBehaviour
    {
        [SerializeField]
        private string _name = "Johny";

        [SerializeField]
        private MobileTypes _type;

        private void OnEnable() =>
            PlayerEnterExit.OnMovePlayerToDungeonStart += SpawnPet;

        private void OnDisable() =>
            PlayerEnterExit.OnMovePlayerToDungeonStart -= SpawnPet;

        private void SpawnPet() =>
            GameObjectHelper.CreatePet(_name, _type, transform.position);
    }
}