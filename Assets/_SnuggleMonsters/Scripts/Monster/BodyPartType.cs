// <copyright file="BodyPartType.cs" company="SnuggleMonsters">
// Copyright (c) SnuggleMonsters. All rights reserved.
// </copyright>

namespace SnuggleMonsters
{
    /// <summary>
    /// Defines the types of body parts a monster can have.
    /// Used by MonsterPartSO and MonsterRuntimeModel to identify and
    /// categorize each selectable part slot on the monster.
    /// </summary>
    public enum BodyPartType
    {
        /// <summary>Main body shape/colour of the monster.</summary>
        Body = 0,

        /// <summary>Eye shape and style.</summary>
        Eyes = 1,

        /// <summary>Horns on the head.</summary>
        Horns = 2,

        /// <summary>Wings on the back.</summary>
        Wings = 3,

        /// <summary>Tail at the rear.</summary>
        Tail = 4,

        /// <summary>Body pattern/texture overlay.</summary>
        Pattern = 5,
    }
}