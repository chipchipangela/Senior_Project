using UnityEngine;

namespace eWolf.BookEffectV2.Interfaces
{
    public interface IBookDetails : IBookMaterials, IBookGameObjects
    {
        Animator BookAnimator { get; set; }
        bool IsBookOpen { get; set; }
        bool IsPageTurnedBackward { get; set; }
        bool IsPageTurningForward { get; set; }
        Animator PageAnimator { get; set; }

        void ApplyMaterials(IDetails details);

        void CloseBook();

        void HidePage();

        void OpenBook(IDetails details);

        void PopulateMaterials(IDetails details);

        void SetSpeed(float speed);

        void ShowPage();

        void TurnPage(IDetails details);

        void TurnPageBack(IDetails details);

        void Update(IDetails details);
    }
}