using BookEffectV3;
using BookEffectV3.Definitions;
using eWolf.BookEffectV2.Interfaces;
using UnityEngine;

namespace eWolf.BookEffect
{
    public class KeyPress : MonoBehaviour
    {
        public GameObject Book;
        private IBookControl _bookControl;
        private BookMeshBuilder _bookControlFull;
        public Texture[] SetA;
        public Texture[] SetB;
        public Texture[] SetC;
        private bool AllReadyAddA = false;
        private bool AllReadyAddB = false;
        private bool AllReadyAddC = false;

        private void Start()
        {
            _bookControl = Book.GetComponent<IBookControl>();
            _bookControlFull = (BookMeshBuilder)_bookControl;
        }

        private void Update()
        {
            if (Input.GetKeyDown("o"))
            {
                _bookControl.OpenBook();
            }

            if (Input.GetKeyDown("a"))
            {
                if (_bookControl.CanTurnPageForward && !Book.GetComponentInChildren<Animation>().isPlaying)
                {
                    _bookControl.TurnPage();
                }
            }
            if (Input.GetKeyDown("d"))
            {
                if (_bookControl.CanTurnPageBackWard && !Book.GetComponentInChildren<Animation>().isPlaying)
                {
                    _bookControl.TurnPageBack();
                }
            }

            if (Input.GetKeyDown("1") && AllReadyAddA == false)
            {
                AllReadyAddA = true;
                
                _bookControlFull.BuildDetails.Pages.InsertRange(2, SetA);
                Debug.Log("Added Set A");
            }

            if (Input.GetKeyDown("2") && AllReadyAddB == false)
            {
                AllReadyAddB = true;
                _bookControlFull.BuildDetails.Pages.InsertRange(2, SetB);
                Debug.Log("Added Set B");
            }

        }
    }
}