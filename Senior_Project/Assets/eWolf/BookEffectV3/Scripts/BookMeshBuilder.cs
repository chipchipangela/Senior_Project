using BookEffectV3.BookMeshActions;
using BookEffectV3.Definitions;
using eWolf.BookEffectV2.Interfaces;
using eWolf.Common.Helper;
using UnityEngine;

namespace BookEffectV3
{
    // TODO: Need to be able to build the book at any angle.
    public class BookMeshBuilder : MonoBehaviour, IBookControl
    {
        public BookMeshControl BookBuilderControl = new BookMeshControl();
        public BookMeshCreator BookBuilderCreator = new BookMeshCreator();
        public BookMeshPageCreator BookBuilderPageCreator = new BookMeshPageCreator();
        public BuildDetails BuildDetails = new BuildDetails();

        public bool CanTurnPageBackWard
        {
            get
            {
                return IsBookOpen && !BookBuilderControl.IsPageTurningForward && BuildDetails.GetStartingPage != 0;
            }
        }

        public bool CanTurnPageForward
        {
            get
            {
                return BookBuilderControl.IsBookOpen && !BookBuilderControl.IsPageTurningForward && BuildDetails.CanTurnPage();
            }
        }

        public IDetails GetDetails
        {
            get
            {
                return null;
            }
        }

        public bool IsBookOpen
        {
            get
            {
                return BookBuilderControl.IsBookOpen;
            }
        }

        public void BuildMesh()
        {
            BookBuilderCreator.BuildDetails = BuildDetails;
            BookBuilderCreator.BookBuilderControl = BookBuilderControl;
            BookBuilderCreator.BaseObject = gameObject;

            BookBuilderCreator.BuildMeshOpenBook();

            BookBuilderPageCreator.BuildDetails = BuildDetails;
            BookBuilderPageCreator.BookBuilderControl = BookBuilderControl;
            BookBuilderPageCreator.BaseObject = gameObject;

            BookBuilderPageCreator.BuildMeshPages();
        }

        public void CloseBook()
        {
            BookBuilderControl.CloseBook();
        }

        public void CloseEditorBook()
        {
            BookBuilderControl.BuildDetails = BuildDetails;
            BookBuilderControl.ApplyMaterials(gameObject, false);
            BookBuilderControl.EditorModeCloseBook();
        }

        public void OpenBook()
        {
            BookBuilderControl.OpenBook();
        }

        public void OpenBookAtPage(int pageIndex)
        {
            BuildDetails.GetStartingPage = pageIndex;
            OpenBook();
        }

        public void OpenEditorBook()
        {
            BookBuilderControl.BuildDetails = BuildDetails;
            BookBuilderControl.ApplyMaterials(gameObject, false);
            BookBuilderControl.EditorModeOpenBook();
        }

        public void SetSpeed(float speed)
        {
            BookBuilderControl.SetSpeed(speed);
        }

        public void Start()
        {
            BuildMesh();
            BookBuilderControl.BuildDetails = BuildDetails;
            BookBuilderControl.ApplyMaterials(gameObject);
            SetSpeed(BuildDetails.StartSpeed);
        }

        public void TurnPage()
        {
            BookBuilderControl.TurnPage();
        }

        public void TurnPageBack()
        {
            BookBuilderControl.TurnPageBack();
        }

        public void Update()
        {
            SceneHelpers.UpdateSnapshot(); // Allows you to take screen shots by pressing 'c'

            BookBuilderControl.Update();
        }
    }
}