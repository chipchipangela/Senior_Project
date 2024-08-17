using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BookEffectV3.Builder
{
    public class UVSet : ICloneable
    {
        public const int BL = 2;
        public const int BR = 3;
        public const int OtherA = 4;
        public const int TL = 0;
        public const int TR = 1;
        public List<Vector2> UVList = new List<Vector2>();

        public UVSet(Vector2 topLeft, Vector2 topRight, Vector2 botLeft, Vector3 botRight)
        {
            PopulateEmptyUV(4);

            UVList[TL] = topLeft;
            UVList[TR] = topRight;
            UVList[BL] = botLeft;
            UVList[BR] = botRight;
        }

        public UVSet(Vector2 topLeft, Vector2 topRight, Vector2 botLeft, Vector3 botRight, Vector2 otherA)
        {
            PopulateEmptyUV(5);

            UVList[TL] = topLeft;
            UVList[TR] = topRight;
            UVList[BL] = botLeft;
            UVList[BR] = botRight;
            UVList[OtherA] = otherA;
        }

        public UVSet()
        {
            UVList = new List<Vector2>();
            for (int i = 0; i < 4; i++)
            {
                UVList.Add(new Vector2());
            }
        }

        public UVSet(float x, float y)
        {
            PopulateEmptyUV(4);

            UVList[TL] = new Vector2(0, 0);
            UVList[TR] = new Vector2(0, y);
            UVList[BL] = new Vector2(x, 0);
            UVList[BR] = new Vector2(x, y);
        }

        public UVSet(float offSetX, float offSetY, float x, float y)
        {
            PopulateEmptyUV(4);

            UVList[TL] = new Vector2(offSetX, offSetY);
            UVList[TR] = new Vector2(offSetX, y);
            UVList[BL] = new Vector2(x, offSetY);
            UVList[BR] = new Vector2(x, y);
        }

        public object Clone()
        {
            return new UVSet(UVList[TL], UVList[TR], UVList[BL], UVList[BR]);
        }

        public void FlipHorizontal()
        {
            FlipHorizontalNormal();
        }

        public void FlipHorizontalNormal()
        {
            for (int index = 0; index < UVList.Count; index++)
            {
                var ps = UVList[index];
                ps.x = 1 - ps.x;

                UVList[index] = ps;
            }
        }

        public void Move(float move, float newLength)
        {
            var temp = UVList[TL];
            temp.x += newLength;
            UVList[TL] = temp;

            temp = UVList[TR];
            temp.x += move;
            UVList[TR] = temp;

            temp = UVList[BL];
            temp.x += newLength;
            UVList[BL] = temp;

            temp = UVList[BR];
            temp.x += move;
            UVList[BR] = temp;
        }

        public UVSet Rotate90Degree(int rotation)
        {
            UVSet rots = (UVSet)Clone();
            for (int i = 0; i < rotation; i++)
            {
                UVSet old = (UVSet)rots.Clone();

                rots.UVList[TR] = old.UVList[TL];
                rots.UVList[BR] = old.UVList[TR];
                rots.UVList[BL] = old.UVList[BR];
                rots.UVList[TL] = old.UVList[BL];
            }
            return rots;
        }

        public void SetUVFromVectors(Vector2[] verts)
        {
            var minX = verts.Min(x => x.x);
            var minY = verts.Min(x => x.y);

            // Remove offset
            List<Vector2> vertNormal = new List<Vector2>();
            foreach (var vert in verts)
            {
                var temp = new Vector2(vert.x, vert.y);
                temp.x -= minX;
                temp.y -= minY;
                vertNormal.Add(temp);
            }

            // Check for man
            var maxX = vertNormal.Max(x => x.x);
            var maxY = vertNormal.Max(x => x.y);

            float ratioX = 1;
            float ratioY = 1;

            ratioX = 1 / maxX;
            ratioY = 1 / maxY;

            List<Vector2> uvList = new List<Vector2>();
            foreach (var vect in vertNormal)
            {
                var temp = new Vector2(vect.x * ratioX, vect.y * ratioY);
                uvList.Add(temp);
            }
            UVList = uvList;
        }

        public void SetUVFromVectors(Vector3[] vectors)
        {
            var leftX = vectors.Min(x => x.x);
            var bottomZ = vectors.Min(x => x.z);

            List<Vector2> verts = new List<Vector2>();
            foreach (var v in vectors)
            {
                var temp = new Vector2(v.x, v.z);
                temp.x -= leftX;
                temp.y -= bottomZ;
                verts.Add(temp);
            }

            var maxX = verts.Max(x => x.x);
            var maxY = verts.Max(x => x.y);

            float ratioX = 1;
            float ratioY = 1;

            ratioX = 1 / maxX;
            ratioY = 1 / maxY;

            List<Vector2> uvList = new List<Vector2>();
            foreach (var v in verts)
            {
                var temp = new Vector2(v.x * ratioX, v.y * ratioY);
                uvList.Add(temp);
            }
            UVList = uvList;
        }

        public void SetUVFromVectorsMagnitude(Vector3[] setA, Vector3[] setB)
        {
            List<Vector2> vectors = new List<Vector2>();

            Vector3 pos = setA[0];
            Vector2 posMore = new Vector2();
            posMore.y = pos.z;
            vectors.Add(posMore);

            for (int i = 1; i < setA.Length; i++)
            {
                Vector3 posA = setA[i];
                var diff = (pos - posA).magnitude;

                Vector2 more = new Vector2(diff, 0);
                posMore += more;
                posMore.y = posA.z;
                pos = posA;
                vectors.Add(posMore);
            }

            pos = setB[0];
            posMore = new Vector2();
            posMore.y = pos.z;
            vectors.Add(posMore);

            for (int i = 1; i < setB.Length; i++)
            {
                Vector3 posA = setB[i];
                var diff = (pos - posA).magnitude;

                Vector2 more = new Vector2(diff, 0);
                posMore += more;
                posMore.y = posA.z;

                pos = posA;
                vectors.Add(posMore);
            }
            SetUVFromVectors(vectors.ToArray());
        }

        private void PopulateEmptyUV(int count)
        {
            UVList = new List<Vector2>();
            for (int i = 0; i < count; i++)
            {
                UVList.Add(new Vector2());
            }
        }
    }
}