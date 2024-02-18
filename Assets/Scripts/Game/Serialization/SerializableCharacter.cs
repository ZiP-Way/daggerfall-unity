// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:
//
// Notes:
//

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using FullSerializer;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.MagicAndEffects;

namespace DaggerfallWorkshop.Game.Serialization
{
    public class SerializableCharacter : MonoBehaviour, ISerializableGameObject
    {
        #region Fields

        DaggerfallCharacter character;

        #endregion

        #region Unity

        void Awake()
        {
            character = GetComponent<DaggerfallCharacter>();
            if (!character)
                throw new Exception("DaggerfallCharacter not found.");
        }

        void Start()
        {
            if (LoadID != 0)
            {
                // In RDB layouts the LoadID is generated from RDB record position
                // This is used to map save data back to an character injected by layout builders
                // But this can result in collisions when an RDB block is used more than once per layout
                // This hack fix will resolve collision by incrementing LoadID
                // This only works because RDB resources are always laid out in the same order
                // So subsequent layouts and collisions will resolve in same way
                // This bug can happen for serializable enemies, doors, and action objects added by layout
                // Does not affect dynamic objects like quest enemies and loot piles
                // Only fixing for enemies now - will look for a better solution in the future
                if (character && GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon)
                {
                    while (SaveLoadManager.StateManager.ContainsEnemy(character.LoadID))
                        character.LoadID++;
                }

                SaveLoadManager.RegisterSerializableGameObject(this);
            }
        }

        void OnDestroy()
        {
            if (LoadID != 0)
                SaveLoadManager.DeregisterSerializableGameObject(this);
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up, "LoadID=" + LoadID);
        }
#endif

#endregion

        #region ISerializableGameObject

        public ulong LoadID { get { return GetLoadID(); } }
        public bool ShouldSave { get { return HasChanged(); } }

        public object GetSaveData()
        {
            if (!character)
                return null;

            // Get entity behaviour
            DaggerfallEntityBehaviour entityBehaviour = character.GetComponent<DaggerfallEntityBehaviour>();
            if (!entityBehaviour)
                return null;

            // Create save data
            EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
            EnemyMotor motor = character.GetComponent<EnemyMotor>();
            EnemySenses senses = character.GetComponent<EnemySenses>();
            var mobileEnemy = character.GetComponentInChildren<MobileUnit>();
            EnemyData_v1 data = new EnemyData_v1();
            data.loadID = LoadID;
            data.gameObjectName = entityBehaviour.gameObject.name;
            data.currentPosition = character.transform.position;
            data.localPosition = character.transform.localPosition;
            data.currentRotation = character.transform.rotation;
            data.worldContext = entity.WorldContext;
            data.worldCompensation = GameManager.Instance.StreamingWorld.WorldCompensation;
            data.entityType = entity.EntityType;
            data.careerName = entity.Career.Name;
            data.careerIndex = entity.CareerIndex;
            data.startingHealth = entity.MaxHealth;
            data.currentHealth = entity.CurrentHealth;
            data.currentFatigue = entity.CurrentFatigue;
            data.currentMagicka = entity.CurrentMagicka;
            data.isHostile = motor.IsHostile;
            data.hasEncounteredPlayer = senses.HasEncounteredPlayer;
            data.isDead = (entity.CurrentHealth <= 0) ? true : false;
            data.questSpawn = character.QuestSpawn;
            data.mobileGender = mobileEnemy.Enemy.Gender;
            data.items = entity.Items.SerializeItems();
            data.equipTable = entity.ItemEquipTable.SerializeEquipTable();
            data.instancedEffectBundles = GetComponent<EntityEffectManager>().GetInstancedBundlesSaveData();
            data.alliedToPlayer = mobileEnemy.Enemy.Team == MobileTeams.PlayerAlly;
            data.questFoeSpellQueueIndex = entity.QuestFoeSpellQueueIndex;
            data.questFoeItemQueueIndex = entity.QuestFoeItemQueueIndex;
            data.wabbajackActive = entity.WabbajackActive;
            data.team = (int)entity.Team + 1;
            data.specialTransformationCompleted = mobileEnemy.SpecialTransformationCompleted;

            // Add quest resource data if present
            QuestResourceBehaviour questResourceBehaviour = GetComponent<QuestResourceBehaviour>();
            if (questResourceBehaviour)
            {
                data.questResource = questResourceBehaviour.GetSaveData();
            }

            return data;
        }

        public void RestoreSaveData(object dataIn)
        {
            if (!character)
                return;

            EnemyData_v1 data = (EnemyData_v1)dataIn;
            if (data.loadID != LoadID)
                return;

            DaggerfallEntityBehaviour entityBehaviour = character.GetComponent<DaggerfallEntityBehaviour>();
            EnemySenses senses = character.GetComponent<EnemySenses>();
            EnemyMotor motor = character.GetComponent<EnemyMotor>();
            EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
            MobileUnit mobileEnemy = character.GetComponentInChildren<MobileUnit>();

            // Restore enemy career or class if different
            if (entity == null || entity.EntityType != data.entityType || entity.CareerIndex != data.careerIndex)
            {
                SetupDemoEnemy setupEnemy = character.GetComponent<SetupDemoEnemy>();
                setupEnemy.ApplyEnemySettings(data.entityType, data.careerIndex, data.mobileGender, data.isHostile, alliedToPlayer: data.alliedToPlayer);
                setupEnemy.AlignToGround();

                if (entity == null)
                    entity = entityBehaviour.Entity as EnemyEntity;
            }

            // Quiesce entity during state restore
            entity.Quiesce = true;

            // Restore enemy data
            entityBehaviour.gameObject.name = data.gameObjectName;
            character.transform.rotation = data.currentRotation;
            entity.QuestFoeSpellQueueIndex = data.questFoeSpellQueueIndex;
            entity.QuestFoeItemQueueIndex = data.questFoeItemQueueIndex;
            entity.WabbajackActive = data.wabbajackActive;
            entity.Items.DeserializeItems(data.items);
            entity.ItemEquipTable.DeserializeEquipTable(data.equipTable, entity.Items);
            entity.MaxHealth = data.startingHealth;
            entity.SetHealth(data.currentHealth, true);
            entity.SetFatigue(data.currentFatigue, true);
            entity.SetMagicka(data.currentMagicka, true);
            int team = data.team;
            if (team > 0)   // Added 1 to made backwards compatible. 0 = no team saved
                entity.Team = (MobileTeams)(team - 1);
            motor.IsHostile = data.isHostile;
            senses.HasEncounteredPlayer = data.hasEncounteredPlayer;

            // Restore enemy position and migrate to floating y support for exteriors
            // Interiors seem to be working fine at this stage with any additional support
            // Dungeons are not involved with floating y and don't need any changes
            WorldContext enemyContext = GetEnemyWorldContext(character);
            if (enemyContext == WorldContext.Exterior)
            {
                RestoreExteriorPositionHandler(character, data, enemyContext);
            }
            else
            {
                // Everything else
                character.transform.position = data.currentPosition;
            }

            // Disable dead enemies
            if (data.isDead)
            {
                entityBehaviour.gameObject.SetActive(false);
            }

            // Restore quest resource link
            character.QuestSpawn = data.questSpawn;
            if (character.QuestSpawn)
            {
                // Add QuestResourceBehaviour to GameObject
                QuestResourceBehaviour questResourceBehaviour = entityBehaviour.gameObject.AddComponent<QuestResourceBehaviour>();
                questResourceBehaviour.RestoreSaveData(data.questResource);

                // Destroy QuestResourceBehaviour if no actual quest properties are restored from save
                if (questResourceBehaviour.QuestUID == 0 || questResourceBehaviour.TargetSymbol == null)
                {
                    character.QuestSpawn = false;
                    Destroy(questResourceBehaviour);
                }
            }

            // Restore instanced effect bundles
            GetComponent<EntityEffectManager>().RestoreInstancedBundleSaveData(data.instancedEffectBundles);

            // Restore special transformation state if completed
            if (data.specialTransformationCompleted && mobileEnemy)
            {
                mobileEnemy.SetSpecialTransformationCompleted();
            }

            // Resume entity
            entity.Quiesce = false;
        }

        #endregion

        #region Private Methods

        void RestoreExteriorPositionHandler(DaggerfallCharacter character, EnemyData_v1 data, WorldContext enemyContext)
        {
            // If enemy context matches serialized world context then enemy was saved after floating y change
            // Need to get relative difference between current and serialized world compensation to get actual y position
            if (enemyContext == data.worldContext)
            {
                float diffY = GameManager.Instance.StreamingWorld.WorldCompensation.y - data.worldCompensation.y;
                character.transform.position = data.currentPosition + new Vector3(0, diffY, 0);
                return;
            }

            // Otherwise we migrate a legacy exterior position by adjusting for world compensation
            character.transform.position = data.currentPosition + GameManager.Instance.StreamingWorld.WorldCompensation;
        }

        WorldContext GetEnemyWorldContext(DaggerfallCharacter character)
        {
            // Must be a parented enemy
            if (!character || !character.transform.parent)
                return WorldContext.Nothing;

            // Interior
            if (character.transform.parent.GetComponentInParent<DaggerfallInterior>())
                return WorldContext.Interior;

            // Dungeon
            if (character.transform.parent.GetComponentInParent<DaggerfallDungeon>())
                return WorldContext.Dungeon;

            // Exterior (loose world object)
            return WorldContext.Exterior;
        }

        bool HasChanged()
        {
            if (!character)
                return false;

            // Always serialize enemy
            return true;

            //// Always save enemy if a quest spawn
            //if (enemy.QuestSpawn)
            //    return true;

            //// Get references
            //DaggerfallEntityBehaviour entityBehaviour = enemy.GetComponent<DaggerfallEntityBehaviour>();
            //EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
            //EnemySenses senses = enemy.GetComponent<EnemySenses>();

            //// Save enemy if it has ever encountered player or if any vital signs have dropped
            //// Enemy should otherwise still be in starting state
            //bool save = false;
            //if (senses.HasEncounteredPlayer ||
            //    entity.CurrentHealth < entity.MaxHealth ||
            //    entity.CurrentFatigue < entity.MaxFatigue ||
            //    entity.CurrentMagicka < entity.MaxMagicka)
            //{
            //    save = true;
            //}

            //return save;
        }

        ulong GetLoadID()
        {
            if (!character)
                return 0;

            return character.LoadID;
        }

        #endregion
    }
}