using System;
using System.Collections.Generic;
using UnityEngine;

namespace BookEffectV3.Definitions
{
    [Serializable]
    public class BookMeshMaterials
    {
        public BookCoverUVSize BookCoverUVSize = new BookCoverUVSize();
        public Material CoverInner;
        public Material CoverOuter;
        public Material InsideBackPage;
        public Material InsideFrontPage;
        public Material Pages;

        public List<Material> GetAll()
        {
            return new List<Material>()
            {
                CoverOuter,
                CoverInner,
                Pages,
                InsideFrontPage,
                InsideBackPage,
            };
        }
    }
}