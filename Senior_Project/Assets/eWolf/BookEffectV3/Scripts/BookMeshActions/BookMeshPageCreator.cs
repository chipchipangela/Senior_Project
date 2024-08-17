using BookEffectV3.Builder;
using BookEffectV3.Definitions;
using BookEffectV3.MeshBuilders;
using eWolf.Common.Helper;
using System.Collections.Generic;
using UnityEngine;

namespace BookEffectV3.BookMeshActions
{
    public class BookMeshPageCreator
    {
        private MeshBuilder _meshBuilder = new MeshBuilder();
        public GameObject BaseObject { get; set; }
        public BookMeshControl BookBuilderControl { get; set; }
        public BuildDetails BuildDetails { get; set; }
        public GameObject PageObject { get; set; }

        public static List<float> GetAcrossSteps(BuildDetails BuildDetails)
        {
            List<Vector3> topArray, bottomArray;
            MeshBuilderBookPage.BuildVectorArrays(BuildDetails, 1, out topArray, out bottomArray);

            List<float> acrossSteps = new List<float>
            {
                (bottomArray[0] - bottomArray[1]).magnitude,
                (bottomArray[1] - bottomArray[2]).magnitude,
                (bottomArray[2] - bottomArray[3]).magnitude
            };

            float count = 9;
            float remainder = (bottomArray[3] - bottomArray[4]).magnitude;
            remainder = remainder / count;

            for (int i = 0; i < count + 1; i++)
            {
                acrossSteps.Add(remainder);
            }
            return acrossSteps;
        }

        public void BuildMeshPages()
        {
            var quaternion = BaseObject.transform.rotation;
            BaseObject.transform.rotation = Quaternion.identity;

            PageObject = ObjectHelper.FindChildObject(BaseObject, "Pages");
            ObjectHelper.RemoveAllObjectFromContaining(PageObject, "BonePage");

            _meshBuilder = new MeshBuilder();

            AddBones();

            var meshBuilderBookCover = new MeshBuilderPage(_meshBuilder, BuildDetails);
            meshBuilderBookCover.Build();

            List<Material> materials = new List<Material>();
            materials.Add(BuildDetails.Materials.InsideFrontPage);
            materials.Add(BuildDetails.Materials.InsideBackPage);

            _meshBuilder.ApplyMeshDetails(PageObject, materials, "Page");

            BaseObject.transform.rotation = quaternion;
        }

        private void AddBones()
        {
            float heightBits = BuildDetails.BookDefinition.Height / 10;
            List<float> acrossSteps = GetAcrossSteps(BuildDetails);

            var pos = new Vector3(0, BuildDetails.BookDefinition.CoverThickness, (heightBits * 3f));
            CreateBoneSet(pos, "BonePageA", acrossSteps);

            pos = new Vector3(0, BuildDetails.BookDefinition.CoverThickness, 0);
            CreateBoneSet(pos, "BonePageB", acrossSteps);

            pos = new Vector3(0, BuildDetails.BookDefinition.CoverThickness, -(heightBits * 3f));
            CreateBoneSet(pos, "BonePageC", acrossSteps);
        }

        private void CreateBoneSet(Vector3 pos, string boneName, List<float> acrossSteps)
        {
            int count = 0;
            var bone = _meshBuilder.AddBone(pos, PageObject.transform, $"{boneName}{count++}", true);

            foreach (var acrossStep in acrossSteps)
            {
                var posB = new Vector3(0, acrossStep, 0);
                bone = _meshBuilder.AddBone(posB, bone, $"{boneName}{count++}", true);
            }
        }
    }
}