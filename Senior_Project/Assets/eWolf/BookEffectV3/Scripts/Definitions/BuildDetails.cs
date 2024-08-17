using System;
using System.Collections.Generic;
using UnityEngine;

namespace BookEffectV3.Definitions
{
    [Serializable]
    public class BuildDetails
    {
        public BookDefinition BookDefinition = new BookDefinition();
        public BookPagesDefinition BookPagesDefinition = new BookPagesDefinition();
        public BookMeshMaterials Materials = new BookMeshMaterials();

        public List<Texture> Pages = new List<Texture>();
        public int StartingPage;
        public float StartSpeed = 2;

        public int GetStartingPage
        {
            get
            {
                return StartingPage;
            }
            set
            {
                StartingPage = value;
            }
        }

        public Texture this[int index]
        {
            get
            {
                if (Pages.Count == 0)
                    return null;

                return Pages[index];
            }
        }

        public bool CanTurnPage()
        {
            return StartingPage < Pages.Count - 2;
        }

        public Texture LeftPage()
        {
            if (Pages.Count == 0)
                return null;
            return Pages[StartingPage];
        }

        public Texture NextLeftPage()
        {
            if (Pages.Count == 0)
                return null;
            return Pages[StartingPage + 2];
        }

        public Texture NextRightPage()
        {
            if (Pages.Count == 0)
                return null;

            return Pages[StartingPage + 3];
        }

        public Texture RightPage()
        {
            if (Pages.Count == 0)
                return null;

            return Pages[StartingPage + 1];
        }

        public void TurnPage()
        {
            StartingPage += 2;
        }

        public void TurnPageBack()
        {
            StartingPage -= 2;
        }
    }
}