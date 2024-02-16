// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Allofich
//
// Notes:
//

using Game.Characters;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Enemy motor and AI combat decision-making logic.
    /// </summary>
    public class EnemyMotor : CharacterMotor
    {
        private const float DefaultStopDistance = 1.7f;

        protected override float GetStopDistance()
        {
            float stopDistance = DefaultStopDistance;

            if (!DaggerfallUnity.Settings.EnhancedCombatAI)
            {
                if (Senses.Target == GameManager.Instance.PlayerEntityBehaviour)
                    stopDistance = Attack.MeleeDistance;
                else
                    stopDistance = Attack.ClassicMeleeDistanceVsAI;
            }

            return stopDistance;
        }
    }
}
