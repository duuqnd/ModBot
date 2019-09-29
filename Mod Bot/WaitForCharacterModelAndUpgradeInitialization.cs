﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModLibrary.YieldInstructions
{
    /// <summary>
    /// Waits until both the upgrades and character model has been initialized on the tracking <see cref="FirstPersonMover"/>
    /// </summary>
    public class WaitForCharacterModelAndUpgradeInitialization : CustomYieldInstruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaitForCharacterModelAndUpgradeInitialization"/> instruction
        /// </summary>
        /// <param name="_firstPersonMover">The <see cref="FirstPersonMover"/> to check</param>
        public WaitForCharacterModelAndUpgradeInitialization(FirstPersonMover _firstPersonMover)
        {
            firstPersonMover = _firstPersonMover;
        }

        /// <summary>
        /// Returns if the <see cref="Coroutine"/> should keep waiting
        /// </summary>
        public override bool keepWaiting
        {
            get
            {
                return Accessor.GetPrivateField<FirstPersonMover, bool>("_hasInitializedCharacter", firstPersonMover);
            }
        }

        FirstPersonMover firstPersonMover;
    }
}