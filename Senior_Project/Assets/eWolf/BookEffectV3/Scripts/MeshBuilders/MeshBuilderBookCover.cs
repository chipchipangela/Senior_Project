using BookEffectV3.Builder;
using BookEffectV3.Definitions;
using BookEffectV3.Interfaces;
using UnityEngine;

namespace BookEffectV3.MeshBuilders
{
    // TODO: When we change the edge width, the pages needs to move - up or down
    public class MeshBuilderBookCover : IMeshSectionBuilder
    {
        private float _coverThickness;
        private float _coverUVWidth;
        private float _edgeBorderX;
        private float _edgeBorderY;
        private float _halfHeight;
        private float _halfThickness;
        private float _height;
        private float _spineUVWidth;
        private float _thickness;
        private float _width;

        public MeshBuilderBookCover(MeshBuilder meshBuilder, BuildDetails createBookMesh)
        {
            MeshBuilder = meshBuilder;
            BuildDetails = createBookMesh;
        }

        public BuildDetails BuildDetails { get; set; }
        public MeshBuilder MeshBuilder { get; }

        public void Build()
        {
            _edgeBorderX = BuildDetails.Materials.BookCoverUVSize.UVEdgeBorderX;
            _edgeBorderY = BuildDetails.Materials.BookCoverUVSize.UVEdgeBorderY;
            _coverUVWidth = BuildDetails.Materials.BookCoverUVSize.UVCoverWidth;
            _spineUVWidth = (1 - (_edgeBorderX * 2)) - (_coverUVWidth * 2);

            UVSet bookOuterCover = new UVSet(
                new Vector2(1f - _edgeBorderX, 1f - _edgeBorderY),
                new Vector2(1f - _edgeBorderX - _coverUVWidth, 1f - _edgeBorderY),
                new Vector2(1f - _edgeBorderX, _edgeBorderY),
                new Vector2(1f - _edgeBorderX - _coverUVWidth, _edgeBorderY)
                );

            _width = BuildDetails.BookDefinition.Width;
            _height = BuildDetails.BookDefinition.Height;
            _halfHeight = _height / 2;

            _coverThickness = BuildDetails.BookDefinition.CoverThickness;
            _thickness = BuildDetails.BookDefinition.Thickness;
            _halfThickness = BuildDetails.BookDefinition.Thickness / 2;

            MeshBuilder.SetMaterial(BuildDetails.Materials.CoverOuter.name);

            // Front in side cover
            Vector3 pos = new Vector3(-_halfThickness, 0, -_halfHeight);
            MeshBuilder.BuildQuadForward(pos, Vector3.right, -_width, _height, bookOuterCover, false,
                "BoneCover2:0.5|BoneCover1:0.5,BoneCover2:0.5|BoneCover1:0.5,BoneCover2:1,BoneCover2:1");

            CreateEdgeLeftSide();

            CreateEdgeLeftBottom();

            CreateEdgeLeftTop();

            bookOuterCover.Move(-_spineUVWidth, -_coverUVWidth);

            // Back Cover end
            pos = new Vector3(_halfThickness, 0, -_halfHeight);
            MeshBuilder.BuildQuadForward(pos, Vector3.right, -_thickness, _height, bookOuterCover, false,
                "BoneCover0:0.5|BoneCover1:0.5,BoneCover0:0.5|BoneCover1:0.5,BoneCover2:0.5|BoneCover1:0.5,BoneCover2:0.5|BoneCover1:0.5");

            bookOuterCover.Move(-_coverUVWidth, -_spineUVWidth);

            // Back cover inside
            pos = new Vector3(_halfThickness, 0, -_halfHeight);
            MeshBuilder.BuildQuadForward(pos, Vector3.right, _width, _height, bookOuterCover, true,
                "BoneCover0:1,BoneCover0:1,BoneCover0:0.5|BoneCover1:0.5,BoneCover0:0.5|BoneCover1:0.5");

            CreateEdgeRightEnd();

            CreateEdgeRightBottom();

            CreateEdgeRightTop();

            CreateEdgeMiddleBottom();

            CreateEdgeMiddleTop();

            CreateInsideCover();
        }

        private void CreateEdgeLeftBottom()
        {
            UVSet edgeLeftBottom = new UVSet(
                            new Vector2(1 - _edgeBorderX, _edgeBorderY),
                            new Vector2(1 - _edgeBorderX - _coverUVWidth, _edgeBorderY),
                            new Vector2(1 - _edgeBorderX, 0.0f),
                            new Vector2(1 - _edgeBorderX - _coverUVWidth, 0.0f)
                            );

            Vector3 pos = new Vector3(-_halfThickness, _coverThickness, -_halfHeight);
            MeshBuilder.BuildQuadUp(pos, Vector3.right, -_width, -_coverThickness, edgeLeftBottom, false,
                "BoneCover2:0.5|BoneCover1:0.5,BoneCover2:0.5|BoneCover1:0.5,BoneCover2:1,BoneCover2:1");
        }

        private void CreateEdgeLeftSide()
        {
            UVSet edgeLeftSide = new UVSet(
                            new Vector2(1 - _edgeBorderX, 1 - _edgeBorderY),
                            new Vector2(1 - _edgeBorderX, _edgeBorderY),
                            new Vector2(1, 1 - _edgeBorderY),
                            new Vector2(1, _edgeBorderY)
                            );

            Vector3 pos = new Vector3(-_halfThickness - _width, _coverThickness, -_halfHeight);
            MeshBuilder.BuildQuadUp(pos, Vector3.forward, _height, -_coverThickness, edgeLeftSide, false,
                "BoneCover2");
        }

        private void CreateEdgeLeftTop()
        {
            UVSet edgeLeftTop = new UVSet(
                new Vector2(1 - _edgeBorderX - _coverUVWidth, 1 - _edgeBorderY),
                new Vector2(1 - _edgeBorderX, 1 - _edgeBorderY),
                new Vector2(1 - _edgeBorderX - _coverUVWidth, 1),
                new Vector2(1 - _edgeBorderX, 1)
                );

            Vector3 pos = new Vector3(-_halfThickness, _coverThickness, _halfHeight);
            MeshBuilder.BuildQuadUp(pos, Vector3.right, -_width, -_coverThickness, edgeLeftTop, true,
                "BoneCover2:1,BoneCover2:1,BoneCover2:0.5|BoneCover1:0.5,BoneCover2:0.5|BoneCover1:0.5");
        }

        private void CreateEdgeMiddleBottom()
        {
            UVSet edgeMiddleBottom = new UVSet(
                new Vector2(1 - _edgeBorderX - _coverUVWidth, _edgeBorderY),
                new Vector2(_edgeBorderX + _coverUVWidth, _edgeBorderY),
                new Vector2(1 - _edgeBorderX - _coverUVWidth, 0.0f),
                new Vector2(_edgeBorderX + _coverUVWidth, 0.0f)
                );

            Vector3 pos = new Vector3(_halfThickness, _coverThickness, -_halfHeight);
            MeshBuilder.BuildQuadUp(pos, Vector3.right, -_thickness, -_coverThickness, edgeMiddleBottom, false,
                "BoneCover0:0.5|BoneCover1:0.5," +
                "BoneCover0:0.5|BoneCover1:0.5," +
                "BoneCover2:0.5|BoneCover1:0.5," +
                "BoneCover2:0.5|BoneCover1:0.5");
        }

        private void CreateEdgeMiddleTop()
        {
            UVSet edgeMiddleTop = new UVSet(
                new Vector2(_edgeBorderX + _coverUVWidth, 1 - _edgeBorderY),
                new Vector2(1 - _edgeBorderX - _coverUVWidth, 1 - _edgeBorderY),
                new Vector2(_edgeBorderX + _coverUVWidth, 1),
                new Vector2(1 - _edgeBorderX - _coverUVWidth, 1)
                );

            Vector3 pos = new Vector3(_halfThickness, _coverThickness, _halfHeight);
            MeshBuilder.BuildQuadUp(pos, Vector3.right, -_thickness, -_coverThickness, edgeMiddleTop, true,
                "BoneCover2:0.5|BoneCover1:0.5," +
                "BoneCover2:0.5|BoneCover1:0.5," +
                "BoneCover0:0.5|BoneCover1:0.5," +
                "BoneCover0:0.5|BoneCover1:0.5");
        }

        private void CreateEdgeRightBottom()
        {
            UVSet edgeRightBottom = new UVSet(
                            new Vector2(_edgeBorderX + _coverUVWidth, _edgeBorderY),
                            new Vector2(_edgeBorderX, _edgeBorderY),
                            new Vector2(_edgeBorderX + _coverUVWidth, 0.0f),
                            new Vector2(_edgeBorderX, 0.0f)
                            );

            Vector3 pos = new Vector3(_halfThickness, _coverThickness, -_halfHeight);
            MeshBuilder.BuildQuadUp(pos, Vector3.right, _width, -_coverThickness, edgeRightBottom, true,
                "BoneCover0:1," +
                "BoneCover0:1," +
                "BoneCover0:0.5|BoneCover1:0.5," +
                "BoneCover0:0.5|BoneCover1:0.5");
        }

        private void CreateEdgeRightEnd()
        {
            UVSet edgeRightSide = new UVSet(
                            new Vector2(_edgeBorderX, _edgeBorderY),
                            new Vector2(_edgeBorderX, 1 - _edgeBorderY),
                            new Vector2(0, _edgeBorderY),
                            new Vector2(0, 1 - _edgeBorderY)
                            );

            Vector3 pos = new Vector3(_width + _halfThickness, _coverThickness, -_halfHeight);
            MeshBuilder.BuildQuadUp(pos, Vector3.forward, _height, -_coverThickness, edgeRightSide, true, "BoneCover0");
        }

        private void CreateEdgeRightTop()
        {
            UVSet edgeRightTop = new UVSet(
                         new Vector2(_edgeBorderX, 1 - _edgeBorderY),
                         new Vector2(_edgeBorderX + _coverUVWidth, 1 - _edgeBorderY),
                         new Vector2(_edgeBorderX, 1),
                         new Vector2(_edgeBorderX + _coverUVWidth, 1)
                         );

            Vector3 pos = new Vector3(_halfThickness, _coverThickness, _halfHeight);
            MeshBuilder.BuildQuadUp(pos, Vector3.right, _width, -_coverThickness, edgeRightTop, false,
                "BoneCover0:0.5|BoneCover1:0.5," +
                "BoneCover0:0.5|BoneCover1:0.5," +
                "BoneCover0:1," +
                "BoneCover0:1");
        }

        private void CreateInsideCover()
        {
            MeshBuilder.SetMaterial(BuildDetails.Materials.CoverInner.name);

            Vector3 pos;
            UVSet bookInnerCover = new UVSet(
                            new Vector2(_coverUVWidth, 1f),
                            new Vector2(0, 1f),
                            new Vector2(_coverUVWidth, 0),
                            new Vector2(0, 0)
                        );

            // Front Out side cover
            pos = new Vector3(-_halfThickness, _coverThickness, -_halfHeight);

            MeshBuilder.BuildQuadForward(pos, Vector3.right, -_width, _height, bookInnerCover, true,
                    "BoneCover2:1,BoneCover2:1,BoneCover2:0.5|BoneCover1:0.5,BoneCover2:0.5|BoneCover1:0.5");

            bookInnerCover.Move(_coverUVWidth, _spineUVWidth);

            pos = new Vector3(_halfThickness, _coverThickness, -_halfHeight);
            MeshBuilder.BuildQuadForward(pos, Vector3.right, -_thickness, _height, bookInnerCover, true,
                "BoneCover2:0.5|BoneCover1:0.5,BoneCover2:0.5|BoneCover1:0.5,BoneCover0:0.5|BoneCover1:0.5,BoneCover0:0.5|BoneCover1:0.5");

            bookInnerCover.Move(_spineUVWidth, _coverUVWidth);

            // Back cover outside
            pos = new Vector3(_halfThickness, _coverThickness, -_halfHeight);
            MeshBuilder.BuildQuadForward(pos, Vector3.right, _width, _height, bookInnerCover, false,
                "BoneCover0:0.5|BoneCover1:0.5,BoneCover0:0.5|BoneCover1:0.5,BoneCover0:1,BoneCover0:1");
        }
    }
}