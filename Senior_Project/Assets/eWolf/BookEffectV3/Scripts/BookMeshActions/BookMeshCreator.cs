using BookEffectV3.Builder;
using BookEffectV3.Definitions;
using BookEffectV3.MeshBuilders;
using eWolf.Common.Helper;
using System.Collections.Generic;
using UnityEngine;

namespace BookEffectV3.BookMeshActions
{
    public class BookMeshCreator
    {
        private MeshBuilder _meshBuilder = new MeshBuilder();
        public GameObject BaseObject { get; set; }
        public BookMeshControl BookBuilderControl { get; set; }
        public BuildDetails BuildDetails { get; set; }

        public void BuildMeshOpenBook()
        {
            var quaternion = BaseObject.transform.rotation;
            BaseObject.transform.rotation = Quaternion.identity;

            _meshBuilder = new MeshBuilder();
            ObjectHelper.RemoveAllObjectFromContaining(BaseObject, "BoneCover");

            float width = BuildDetails.BookDefinition.Width;
            float coverThickness = BuildDetails.BookDefinition.CoverThickness;
            float thickness = BuildDetails.BookDefinition.Thickness;
            float halfThickness = BuildDetails.BookDefinition.Thickness / 2;

            AddBones(width, thickness, halfThickness, coverThickness / 2);

            var boneRoot = ObjectHelper.FindChildObject(BaseObject, "BoneCover0");
            var animControaller = boneRoot.AddComponent<Animator>();

            RuntimeAnimatorController runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("BookMeshController");
            animControaller.runtimeAnimatorController = runtimeAnimatorController;
            SetAnimationForEditorMode(boneRoot);

            var meshBuilderBookCover = new MeshBuilderBookCover(_meshBuilder, BuildDetails);
            meshBuilderBookCover.Build();

            var meshBuilderBookPage = new MeshBuilderBookPage(_meshBuilder, BuildDetails, -1, BuildDetails.Materials.InsideFrontPage.name, "BoneCover2");
            meshBuilderBookPage.Build();

            var meshBuilderBookPageRight = new MeshBuilderBookPage(_meshBuilder, BuildDetails, 1, BuildDetails.Materials.InsideBackPage.name, "BoneCover0");
            meshBuilderBookPageRight.Build();

            List<Material> materials = BuildDetails.Materials.GetAll();

            _meshBuilder.ApplyMeshDetails(BaseObject, materials, "Book cover");

            BaseObject.transform.rotation = quaternion;
        }

        private static void SetAnimationForEditorMode(GameObject boneRoot)
        {
            var animation = boneRoot.AddComponent<Animation>();
            AnimationClip animationClip = (AnimationClip)Resources.Load("CloseBookEditor");
            animation.AddClip(animationClip, "CloseBookEditor");
            animationClip = (AnimationClip)Resources.Load("OpenBookEditor");
            animation.AddClip(animationClip, "OpenBookEditor");
            animation.playAutomatically = false;
            animation.clip = animationClip;
        }

        private void AddBones(float width, float thickness, float halfThickness, float halfcoverThickness)
        {
            var posA = new Vector3(halfThickness + width, halfcoverThickness, 0);
            var bone = _meshBuilder.AddBone(posA, BaseObject.transform, "BoneCover");

            var posB = new Vector3(-width, 0, 0);
            bone = _meshBuilder.AddBone(posB, bone, "BoneCover");

            var posC = new Vector3(-thickness, 0, 0);
            bone = _meshBuilder.AddBone(posC, bone, "BoneCover");

            var posD = new Vector3(-width, 0, 0);
            _meshBuilder.AddBone(posD, bone, "BoneCover");
        }
    }
}