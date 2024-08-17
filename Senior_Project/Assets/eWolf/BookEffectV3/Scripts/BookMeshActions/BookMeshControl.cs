using BookEffectV3.Definitions;
using eWolf.Common.Helper;
using UnityEngine;

namespace BookEffectV3.BookMeshActions
{
    public class BookMeshControl
    {
        public BuildDetails BuildDetails { get; set; }
        public bool IsBookOpen { get; set; }
        public bool IsPageTurnedBackward { get; set; }
        public bool IsPageTurningForward { get; set; }
        public GameObject PageMesh { get; set; }
        private Animator _bookAnimator { get; set; }
        private Material _bookCoverInsideBackPage { get; set; }
        private Material _bookCoverInsideFrontPage { get; set; }
        private GameObject _bookCoverRoot { get; set; }
        private Animation _coverAnimation { get; set; }
        private Animator _pageAnimator { get; set; }
        private Material _pageTextureBack { get; set; }
        private Material _pageTextureFront { get; set; }

        public void ApplyMaterials(GameObject baseObject, bool setMainTexture = true)
        {
            _bookCoverRoot = ObjectHelper.FindChildObject(baseObject, "BoneCover0");
            _bookAnimator = _bookCoverRoot.GetComponent<Animator>();
            _coverAnimation = _bookCoverRoot.GetComponent<Animation>();

            PageMesh = ObjectHelper.FindChildObject(baseObject, "Pages");
            _pageAnimator = PageMesh.GetComponent<Animator>();

            HidePage();
            _pageAnimator.applyRootMotion = false;
            _pageAnimator.applyRootMotion = true;

            if (!setMainTexture)
                return;

            foreach (Material material in baseObject.GetComponent<Renderer>().materials)
            {
                if (material.name == BuildDetails.Materials.InsideFrontPage.name + " (Instance)")
                {
                    _bookCoverInsideFrontPage = material;
                }
                if (material.name == BuildDetails.Materials.InsideBackPage.name + " (Instance)")
                {
                    _bookCoverInsideBackPage = material;
                }
            }

            foreach (Material material in PageMesh.GetComponent<Renderer>().materials)
            {
                if (material.name == BuildDetails.Materials.InsideFrontPage.name + " (Instance)")
                {
                    _pageTextureFront = material;
                }

                if (material.name == BuildDetails.Materials.InsideBackPage.name + " (Instance)")
                {
                    _pageTextureBack = material;
                }
            }

            _pageTextureFront.mainTexture = BuildDetails.RightPage();
            _pageTextureBack.mainTexture = BuildDetails.NextLeftPage();

            _bookCoverInsideFrontPage.mainTexture = BuildDetails.LeftPage();
            _bookCoverInsideBackPage.mainTexture = BuildDetails.RightPage();
        }

        public void CloseBook()
        {
            HidePage();
            IsPageTurningForward = false;
            IsPageTurnedBackward = false;

            IsBookOpen = false;
            _bookAnimator.SetBool("Open", false);
        }

        public void EditorModeCloseBook()
        {
            HidePage();
            IsPageTurningForward = false;
            IsPageTurnedBackward = false;

            IsBookOpen = false;
            _bookAnimator.SetBool("Open", false);

            _coverAnimation.Play("CloseBookEditor");
            _coverAnimation.Sample();
        }

        public void EditorModeOpenBook()
        {
            HidePage();
            IsPageTurningForward = false;
            IsPageTurnedBackward = false;

            IsBookOpen = true;
            _bookAnimator.SetBool("Open", true);

            _coverAnimation.Play("OpenBookEditor");
            _coverAnimation.Sample();
        }

        public void HidePage()
        {
            PageMesh.SetActive(false);
        }

        public void OpenBook()
        {
            _bookCoverInsideFrontPage.mainTexture = BuildDetails.LeftPage();
            _bookCoverInsideBackPage.mainTexture = BuildDetails.RightPage();
            IsBookOpen = true;
            _bookAnimator.SetBool("Open", true);
        }

        public void SetSpeed(float speed)
        {
            _bookAnimator.speed = speed;
            _pageAnimator.speed = speed;
        }

        public void ShowPage()
        {
            PageMesh.SetActive(true);
        }

        public void TurnPage()
        {
            ShowPage();
            IsPageTurningForward = true;
            StartPageTurningForward();

            _pageTextureBack.mainTexture = BuildDetails.RightPage();
            _pageTextureFront.mainTexture = BuildDetails.NextLeftPage();

            _bookCoverInsideFrontPage.mainTexture = BuildDetails.LeftPage();
            _bookCoverInsideBackPage.mainTexture = BuildDetails.NextRightPage();
        }

        public void TurnPageBack()
        {
            ShowPage();
            BuildDetails.TurnPageBack();

            IsPageTurnedBackward = true;
            StartPageTurningBackwards();

            _pageTextureBack.mainTexture = BuildDetails.RightPage();
            _pageTextureFront.mainTexture = BuildDetails.NextLeftPage();

            _bookCoverInsideFrontPage.mainTexture = BuildDetails.LeftPage();
        }

        public void Update()
        {
            if (IsPageTurningForward || IsPageTurnedBackward)
            {
                AnimatorStateInfo currentBaseState = _pageAnimator.GetCurrentAnimatorStateInfo(0);
                if (currentBaseState.IsName("PageClosedWait"))
                {
                    if (IsPageTurnedBackward)
                    {
                        IsPageTurnedBackward = false;
                        HidePage();
                        ClearAllAnims();
                        _bookCoverInsideBackPage.mainTexture = BuildDetails.RightPage();
                    }
                }
                if (currentBaseState.IsName("PageOpenWait"))
                {
                    if (IsPageTurningForward)
                    {
                        IsPageTurningForward = false;
                        BuildDetails.TurnPage();

                        HidePage();
                        ClearAllAnims();
                        _bookCoverInsideFrontPage.mainTexture = BuildDetails.LeftPage();
                    }
                }
            }
        }

        private void ClearAllAnims()
        {
            _pageAnimator.SetBool("TurnPageNormal", false);
            _pageAnimator.SetBool("TurnPageNormalA", false);
            _pageAnimator.SetBool("TurnPageBackNormal", false);
        }

        private void StartPageTurningBackwards()
        {
            bool TurnPageNormal = false;
            bool TurnPageNormalA = false;
            bool TurnPageBackNormal = false;
            bool TurnPageBackNormalA = false;
            if (BuildDetails.BookPagesDefinition.PageTurnStyle == PageTurnStyles.Flat)
            {
                TurnPageBackNormal = true;
            }
            if (BuildDetails.BookPagesDefinition.PageTurnStyle == PageTurnStyles.BottomPage)
            {
                TurnPageBackNormalA = true;
            }

            _pageAnimator.SetBool("TurnPageNormal", TurnPageNormal);
            _pageAnimator.SetBool("TurnPageNormalA", TurnPageNormalA);

            _pageAnimator.SetBool("TurnPageBackNormal", TurnPageBackNormal);
            _pageAnimator.SetBool("TurnPageBackNormalA", TurnPageBackNormalA);
        }

        private void StartPageTurningForward()
        {
            bool TurnPageNormal = false;
            bool TurnPageNormalA = false;
            bool TurnPageBackNormal = false;
            if (BuildDetails.BookPagesDefinition.PageTurnStyle == PageTurnStyles.Flat)
            {
                TurnPageNormal = true;
            }
            if (BuildDetails.BookPagesDefinition.PageTurnStyle == PageTurnStyles.BottomPage)
            {
                TurnPageNormalA = true;
            }

            _pageAnimator.SetBool("TurnPageNormal", TurnPageNormal);
            _pageAnimator.SetBool("TurnPageNormalA", TurnPageNormalA);

            _pageAnimator.SetBool("TurnPageBackNormal", TurnPageBackNormal);
        }
    }
}