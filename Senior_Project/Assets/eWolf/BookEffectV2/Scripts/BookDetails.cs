using eWolf.BookEffectV2.Interfaces;
using eWolf.Common.Helper;
using UnityEngine;

namespace eWolf.BookEffectV2
{
    public class BookDetails : IBookDetails
    {
        public BookDetails(GameObject gameObject, IDetails details)
        {
            BookCoverAnimation = ObjectHelper.FindChildObjectRecursive(gameObject, "BookCover");
            BookCoverMesh = ObjectHelper.FindChildObjectRecursive(gameObject, "Book_Turn");

            PageAnimation = ObjectHelper.FindChildObjectRecursive(gameObject, "Book01-Page");
            PageMesh = ObjectHelper.FindChildObjectRecursive(gameObject, "Book01_Page");

            BookAnimator = BookCoverAnimation.GetComponent<Animator>();
            PageAnimator = PageAnimation.GetComponent<Animator>();

            PopulateMaterials(details);
            HidePage();
        }

        public Animator BookAnimator { get; set; }

        public GameObject BookCoverAnimation { get; set; }

        public GameObject BookCoverMesh { get; set; }

        public Material InsideBackCover { get; set; }

        public Material InsideFrontCover { get; set; }

        public bool IsBookOpen { get; set; }

        public bool IsPageTurnedBackward { get; set; }

        public bool IsPageTurningForward { get; set; }

        public GameObject PageAnimation { get; set; }

        public Animator PageAnimator { get; set; }

        public Material PageBack { get; set; }

        public Material PageFront { get; set; }

        public GameObject PageMesh { get; set; }

        public void ApplyMaterials(IDetails details)
        {
            var renderer = BookCoverMesh.GetComponent<Renderer>();
            var materials = renderer.materials;

            materials[0] = details.GetBookCover;
            materials[1] = details.GetBookSides;

            renderer.materials = materials;
        }

        public void CloseBook()
        {
            HidePage();
            IsBookOpen = false;
            BookAnimator.SetBool("Open", false);
        }

        public void HidePage()
        {
            PageMesh.SetActive(false);
        }

        public void OpenBook(IDetails details)
        {
            InsideFrontCover.mainTexture = details.LeftPage();
            InsideBackCover.mainTexture = details.RightPage();
            IsBookOpen = true;
            BookAnimator.SetBool("Open", true);
        }

        public void PopulateMaterials(IDetails details)
        {
            foreach (Material material in BookCoverMesh.GetComponent<Renderer>().materials)
            {
                if (material.name == "Cover (Instance)")
                {
                }
                if (material.name == "InsideBackCover (Instance)")
                {
                    InsideBackCover = material;
                }
                if (material.name == "InsideFrontCover (Instance)")
                {
                    InsideFrontCover = material;
                }
            }

            InsideFrontCover.mainTexture = details.LeftPage();
            InsideBackCover.mainTexture = details.RightPage();

            foreach (Material material in PageMesh.GetComponent<Renderer>().materials)
            {
                if (material.name == "PageFront (Instance)")
                {
                    PageFront = material;
                }

                if (material.name == "PageBack (Instance)")
                {
                    PageBack = material;
                }
            }

            PageFront.mainTexture = details.RightPage();
            PageBack.mainTexture = details.NextLeftPage();
        }

        public void SetSpeed(float speed)
        {
            BookAnimator.speed = speed;
            PageAnimator.speed = speed;
        }

        public void ShowPage()
        {
            PageMesh.SetActive(true);
        }

        public void TurnPage(IDetails details)
        {
            ShowPage();

            IsPageTurningForward = true;
            PageAnimator.SetBool("TurnPageNormal", true);
            PageAnimator.SetBool("TurnPageBackNormal", false);

            InsideFrontCover.mainTexture = details.LeftPage();
            PageFront.mainTexture = details.RightPage();
            PageBack.mainTexture = details.NextLeftPage();
            InsideBackCover.mainTexture = details.NextRightPage();
        }

        public void TurnPageBack(IDetails details)
        {
            details.TurnPageBack();
            ShowPage();

            IsPageTurnedBackward = true;
            PageAnimator.SetBool("TurnPageNormal", false);
            PageAnimator.SetBool("TurnPageBackNormal", true);

            InsideFrontCover.mainTexture = details.LeftPage();
            PageFront.mainTexture = details.RightPage();
            PageBack.mainTexture = details.NextLeftPage();
            InsideBackCover.mainTexture = details.NextRightPage();
        }

        public void Update(IDetails Details)
        {
            if (IsPageTurningForward || IsPageTurnedBackward)
            {
                AnimatorStateInfo currentBaseState = PageAnimator.GetCurrentAnimatorStateInfo(0);
                if (currentBaseState.IsName("WaitBackTurn"))
                {
                    if (IsPageTurningForward)
                    {
                        IsPageTurningForward = false;
                        Details.TurnPage();
                        HidePage();
                        PageAnimator.SetBool("TurnPageNormal", false);

                        InsideFrontCover.mainTexture = Details.LeftPage();
                    }
                }
                if (currentBaseState.IsName("WaitTurnMid"))
                {
                    if (IsPageTurnedBackward)
                    {
                        IsPageTurnedBackward = false;

                        HidePage();
                        PageAnimator.SetBool("TurnPageBackNormal", false);
                        InsideBackCover.mainTexture = Details.RightPage();
                    }
                }
            }
        }
    }
}