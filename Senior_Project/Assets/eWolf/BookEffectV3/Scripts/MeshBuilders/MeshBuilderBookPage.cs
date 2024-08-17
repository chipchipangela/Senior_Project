using BookEffectV3.Builder;
using BookEffectV3.Definitions;
using BookEffectV3.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BookEffectV3.MeshBuilders
{
    public class MeshBuilderBookPage : IMeshSectionBuilder
    {
        private readonly string _boneName;
        private readonly float _direction = 0;
        private readonly string _materialPageName;

        public MeshBuilderBookPage(MeshBuilder meshBuilder, BuildDetails createBookMesh, float direction, string materialPageName, string boneName)
        {
            MeshBuilder = meshBuilder;
            BuildDetails = createBookMesh;
            _direction = direction;
            _materialPageName = materialPageName;
            _boneName = boneName;
        }

        public BuildDetails BuildDetails { get; set; }
        public MeshBuilder MeshBuilder { get; }

        public static void BuildVectorArrays(BuildDetails buildDetails, float direction, out List<Vector3> topArray, out List<Vector3> bottomArray)
        {
            float width = buildDetails.BookDefinition.Width;
            float height = buildDetails.BookDefinition.Height;
            float halfWidth = width / 2;
            float halfHeight = height / 2;

            float coverThickness = buildDetails.BookDefinition.CoverThickness;
            float halfThickness = buildDetails.BookDefinition.Thickness / 2;

            Vector3 posStart = new Vector3(0, coverThickness, -halfHeight + buildDetails.BookPagesDefinition.EdgeGap);
            Vector3 posTopStart = new Vector3(0, coverThickness, halfHeight - buildDetails.BookPagesDefinition.EdgeGap);

            float thickCourve = halfThickness - (coverThickness / 2);
            List<Vector3> courvePoints = new List<Vector3>() {
                new Vector3(0.0f * direction, 0f, 0),
                new Vector3(0.5f * direction, 0.4f, 0),
                new Vector3(1.1f * direction, 0.745f, 0),
                new Vector3(1.7f * direction, 1f, 0),
                new Vector3((halfWidth*2) * direction, 1, 0),
            };

            topArray = new List<Vector3>();
            bottomArray = new List<Vector3>();

            bottomArray.Add(posStart + (courvePoints[0] * thickCourve));
            topArray.Add(posTopStart + (courvePoints[0] * thickCourve));

            bottomArray.Add(posStart + (courvePoints[1] * thickCourve));
            topArray.Add(posTopStart + (courvePoints[1] * thickCourve));

            bottomArray.Add(posStart + (courvePoints[2] * thickCourve));
            topArray.Add(posTopStart + (courvePoints[2] * thickCourve));

            bottomArray.Add(posStart + (courvePoints[3] * thickCourve));
            topArray.Add(posTopStart + (courvePoints[3] * thickCourve));

            // set the X to be the position from the edge.
            var temppos = posStart + (courvePoints[4] * thickCourve);
            temppos.x = (width + halfThickness - buildDetails.BookPagesDefinition.EdgeGap) * direction;

            bottomArray.Add(temppos);

            temppos = posTopStart + (courvePoints[4] * thickCourve);
            temppos.x = (width + halfThickness - buildDetails.BookPagesDefinition.EdgeGap) * direction;
            topArray.Add(temppos);

            var pos = bottomArray[bottomArray.Count - 1];
            var posTop = topArray[bottomArray.Count - 1];
            pos.y = bottomArray[0].y;
            posTop.y = topArray[0].y;
            bottomArray.Add(pos);
            topArray.Add(posTop);

            pos = bottomArray[3];
            posTop = topArray[3];
            pos.y = bottomArray[0].y;
            posTop.y = topArray[0].y;
            bottomArray.Add(pos);
            topArray.Add(posTop);

            pos = bottomArray[2];
            posTop = topArray[2];
            pos.y = bottomArray[0].y;
            posTop.y = topArray[0].y;
            bottomArray.Add(pos);
            topArray.Add(posTop);
        }

        public void Build()
        {
            MeshBuilder.SetMaterial(BuildDetails.Materials.InsideFrontPage.name);

            List<Vector3> topArray, bottomArray;
            BuildVectorArrays(BuildDetails, _direction, out topArray, out bottomArray);

            MeshBuilder.SetMaterial(BuildDetails.Materials.Pages.name);
            int[] weights = null;

            UVSet uVSet = new UVSet(
               new Vector2(1, 1),
               new Vector2(1, 0),
               new Vector2(0, 1),
               new Vector2(0f, 0)
               );

            // far left edge
            if (_direction > 0)
                weights = MeshBuilder.BuildQuadMapOrder(topArray[4], topArray[5], bottomArray[4], bottomArray[5], uVSet);
            else
                weights = MeshBuilder.BuildQuadMapOrder(bottomArray[4], bottomArray[5], topArray[4], topArray[5], uVSet);
            MeshBuilder.AssignVerticesToBones(weights, _boneName);

            // bottom edge
            if (_direction > 0)
                weights = MeshBuilder.BuildQuadMapOrder(bottomArray[4], bottomArray[5], bottomArray[3], bottomArray[6], uVSet);
            else
                weights = MeshBuilder.BuildQuadMapOrder(bottomArray[3], bottomArray[6], bottomArray[4], bottomArray[5], uVSet);
            MeshBuilder.AssignVerticesToBones(weights, _boneName);

            UVSet uVSetConer = new UVSet(
                new Vector2(0.1f, 0),
                new Vector2(0.1f, 0.8f),
                new Vector2(0.2f, 0.4f),
                new Vector2(0.30f, 0.0f)
                );

            // bottom edge inner corner
            if (_direction > 0)
                weights = MeshBuilder.BuildFanSwap(bottomArray[7], bottomArray[2], bottomArray[1], bottomArray[0], uVSetConer);
            else
                weights = MeshBuilder.BuildFan(bottomArray[7], bottomArray[2], bottomArray[1], bottomArray[0], uVSetConer);

            var boneWeight = MeshBuilder.CreateBoneWeights(weights, _boneName);
            MeshBuilder.Create2Bones(ref boneWeight.BoneWeights[0], $"{_boneName}:1");
            MeshBuilder.Create2Bones(ref boneWeight.BoneWeights[1], $"{_boneName}:1");
            MeshBuilder.Create2Bones(ref boneWeight.BoneWeights[2], $"{_boneName}:0.5|BoneCover1:0.5");
            MeshBuilder.Create2Bones(ref boneWeight.BoneWeights[3], "BoneCover1:1");
            MeshBuilder.Weights.AddRange(boneWeight.BoneWeights);

            // Bottom edge inner corner - side bit
            UVSet uVSetConerBit = new UVSet(
                new Vector2(0, 0),
                new Vector2(0.0f, 1f),
                new Vector2(0.1f, 0f),
                new Vector2(0.1f, 0.8f)
                );

            if (_direction > 0)
                weights = MeshBuilder.BuildQuadMapOrder(bottomArray[3], bottomArray[6], bottomArray[2], bottomArray[7], uVSetConerBit);
            else
                weights = MeshBuilder.BuildQuadMapOrder(bottomArray[6], bottomArray[3], bottomArray[7], bottomArray[2], uVSetConerBit);

            boneWeight = MeshBuilder.CreateBoneWeights(weights, _boneName);
            MeshBuilder.Weights.AddRange(boneWeight.BoneWeights);

            // top edge
            if (_direction > 0)
                weights = MeshBuilder.BuildQuadMapOrder(topArray[3], topArray[6], topArray[4], topArray[5], uVSet);
            else
                weights = MeshBuilder.BuildQuadMapOrder(topArray[4], topArray[5], topArray[3], topArray[6], uVSet);
            MeshBuilder.AssignVerticesToBones(weights, _boneName);

            // top edge inner corner
            uVSetConer = new UVSet(
                new Vector2(0.1f, 0),
                new Vector2(0.1f, 0.8f),
                new Vector2(0.2f, 0.4f),
                new Vector2(0.30f, 0.0f)
                );
            if (_direction > 0)
                weights = MeshBuilder.BuildFan(topArray[7], topArray[2], topArray[1], topArray[0], uVSetConer);
            else
                weights = MeshBuilder.BuildFanSwap(topArray[7], topArray[2], topArray[1], topArray[0], uVSetConer);
            boneWeight = MeshBuilder.CreateBoneWeights(weights, _boneName);
            MeshBuilder.Create2Bones(ref boneWeight.BoneWeights[0], $"{_boneName}:1");
            MeshBuilder.Create2Bones(ref boneWeight.BoneWeights[1], $"{_boneName}:1");
            MeshBuilder.Create2Bones(ref boneWeight.BoneWeights[2], $"{_boneName}:0.5|BoneCover1:0.5");
            MeshBuilder.Create2Bones(ref boneWeight.BoneWeights[3], "BoneCover1:1");
            MeshBuilder.Weights.AddRange(boneWeight.BoneWeights);

            uVSetConerBit = new UVSet(
              new Vector2(0.1f, 0f),
              new Vector2(0.1f, 0.8f),
              new Vector2(0, 0),
              new Vector2(0.0f, 1f)
              );

            uVSetConerBit.FlipHorizontal();
            // Bottom edge inner corner - side bit
            if (_direction > 0)
                weights = MeshBuilder.BuildQuadMapOrder(topArray[6], topArray[3], topArray[7], topArray[2], uVSetConerBit);
            else
                weights = MeshBuilder.BuildQuadMapOrder(topArray[7], topArray[2], topArray[6], topArray[3], uVSetConerBit);
            MeshBuilder.AssignVerticesToBones(weights, _boneName);

            MeshBuilder.SetMaterial(_materialPageName);
            List<Vector3> allVerts = new List<Vector3>();
            allVerts.AddRange(bottomArray);
            allVerts.AddRange(topArray);

            UVSet pageTop = new UVSet(1, 1);
            pageTop.SetUVFromVectorsMagnitude(bottomArray.Take(5).ToArray(), topArray.Take(5).ToArray());

            if (_direction > 0)
                weights = MeshBuilder.BuildConnectedArraySwap(bottomArray, topArray, 0, 5, pageTop);
            else
            {
                pageTop.FlipHorizontalNormal();
                weights = MeshBuilder.BuildConnectedArray(bottomArray, topArray, 0, 5, pageTop);
            }

            boneWeight = MeshBuilder.CreateBoneWeights(weights, _boneName);
            MeshBuilder.Create2Bones(ref boneWeight.BoneWeights[0], "BoneCover1:1");
            MeshBuilder.Create2Bones(ref boneWeight.BoneWeights[1], "BoneCover1:1");

            MeshBuilder.Create2Bones(ref boneWeight.BoneWeights[2], $"{_boneName}:0.4|BoneCover1:0.6");
            MeshBuilder.Create2Bones(ref boneWeight.BoneWeights[3], $"{_boneName}:0.4|BoneCover1:0.6");
            MeshBuilder.Weights.AddRange(boneWeight.BoneWeights);
        }
    }
}