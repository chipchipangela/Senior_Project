using UnityEngine;

namespace eWolf.BookEffectV2.Interfaces
{
    public interface IDetails
    {
        Material GetBookCover { get; }
        Material GetBookSides { get; }

        int GetStartingPage { get; set; }

        Texture this[int index] { get; }

        bool CanTurnPage();

        Texture LeftPage();

        Texture NextLeftPage();

        Texture NextRightPage();

        Texture RightPage();

        void TurnPage();

        void TurnPageBack();
    }
}