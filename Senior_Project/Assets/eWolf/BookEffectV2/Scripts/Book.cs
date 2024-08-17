using eWolf.BookEffectV2.Interfaces;
using eWolf.Common.Helper;
using UnityEngine;

// Book Effect by Electric Wolf
// Version logs
// v2.1: Update code structure, created new interfaces and classes.
// v3.0: Now building the book at runtime.
namespace eWolf.BookEffectV2
{
    public class Book : MonoBehaviour, IBookControl
    {
        public Details Details = new Details();
        public int StartSpeed = 2;
        private IBookDetails _bookDetails;

        public bool CanTurnPageBackWard
        {
            get
            {
                return IsBookOpen && !_bookDetails.IsPageTurningForward && Details.GetStartingPage != 0;
            }
        }

        public bool CanTurnPageForward
        {
            get
            {
                return _bookDetails.IsBookOpen && !_bookDetails.IsPageTurningForward && Details.CanTurnPage();
            }
        }

        public IDetails GetDetails
        {
            get
            {
                return Details;
            }
        }

        public bool IsBookOpen
        {
            get
            {
                return _bookDetails.IsBookOpen;
            }
        }

        public void CloseBook()
        {
            _bookDetails.CloseBook();
        }

        public void OpenBook()
        {
            _bookDetails.OpenBook(Details);
        }

        public void OpenBookAtPage(int pageIndex)
        {
            Details.GetStartingPage = pageIndex;
            OpenBook();
        }

        public void SetSpeed(float speed)
        {
            _bookDetails.SetSpeed(speed);
        }

        public void Start()
        {
            _bookDetails = new BookDetails(gameObject, Details);
            SetSpeed(StartSpeed);
            _bookDetails.ApplyMaterials(Details);
        }

        public void TurnPage()
        {
            _bookDetails.TurnPage(Details);
        }

        public void TurnPageBack()
        {
            _bookDetails.TurnPageBack(Details);
        }

        public void Update()
        {
            SceneHelpers.UpdateSnapshot(); // Allows you to take screen shots by pressing 'c'

            _bookDetails.Update(Details);
        }
    }
}