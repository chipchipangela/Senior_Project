using System;
using UnityEngine;

namespace BookEffectV3.Definitions
{
    [Serializable]
    public class BookCoverUVSize
    {
        [Range(0.2f, 0.5f)]
        public float UVCoverWidth = 0.46f;

        /// <summary>
        /// Set the boarder UV size X
        /// </summary>
        [Range(0, 0.1f)]
        public float UVEdgeBorderX = 0.0191f;

        /// <summary>
        /// Set the boarder UV size Y
        /// </summar>
        [Range(0, 0.1f)]
        public float UVEdgeBorderY = 0.025f;
    }
}