using BookEffectV3.BookMeshActions;
using BookEffectV3.Builder;
using BookEffectV3.Definitions;
using BookEffectV3.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace BookEffectV3.MeshBuilders
{
    public class MeshBuilderPage : IMeshSectionBuilder
    {
        public MeshBuilderPage(MeshBuilder meshBuilder, BuildDetails createBookMesh)
        {
            MeshBuilder = meshBuilder;
            BuildDetails = createBookMesh;
        }

        public BuildDetails BuildDetails { get; set; }
        public MeshBuilder MeshBuilder { get; }

        public void Build()
        {
            List<float> acrossSteps = BookMeshPageCreator.GetAcrossSteps(BuildDetails);
            List<float> vertical = new List<float>();

            float girdAcross = 9;
            float gridSize = (BuildDetails.BookDefinition.Height - (BuildDetails.BookPagesDefinition.EdgeGap * 2)) / girdAcross;

            for (int i = 0; i < girdAcross + 1; i++)
            {
                vertical.Add(gridSize);
            }

            float edgeGap = BuildDetails.BookPagesDefinition.EdgeGap / 2;
            MeshBuilder.SetMaterial(BuildDetails.Materials.InsideFrontPage.name);

            float YoffSet = BuildDetails.BookDefinition.CoverThickness;

            Vector3 pos = new Vector3(0, YoffSet, (BuildDetails.BookDefinition.Height / 2) - BuildDetails.BookPagesDefinition.EdgeGap);
            List<BoneWeight> weights = MeshBuilder.BuildQuadGrid(pos, acrossSteps, vertical, false);
            MeshBuilder.Weights.AddRange(weights);

            MeshBuilder.SetMaterial(BuildDetails.Materials.InsideBackPage.name);
            weights = MeshBuilder.BuildQuadGrid(pos, acrossSteps, vertical, true);
            MeshBuilder.Weights.AddRange(weights);
        }
    }
}