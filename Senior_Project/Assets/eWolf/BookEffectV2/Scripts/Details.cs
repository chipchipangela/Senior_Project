using eWolf.BookEffectV2.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace eWolf.BookEffectV2
{
    [Serializable]
    public class Details : IDetails
    {
        public Material Material_Cover;
        public Material Material_Sides;
        public List<Texture> Pages = new List<Texture>();
        public int StartingPage;

        public Material GetBookCover
        {
            get
            {
                return Material_Cover;
            }
        }

        public Material GetBookSides
        {
            get
            {
                return Material_Sides;
            }
        }

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