using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BookEffectV3.Builder
{
    public class MeshBuilder
    {
        private readonly List<Matrix4x4> _bindPoses = new List<Matrix4x4>();
        private readonly Dictionary<string, int> _boneMapping = new Dictionary<string, int>();
        private readonly List<Transform> _bones = new List<Transform>();
        private readonly Dictionary<string, List<int>> _meshMaterialsTriangles = new Dictionary<string, List<int>>();
        private string _currentMaterialName = string.Empty;

        public string CurrentMaterialName
        {
            get { return _currentMaterialName; }
        }

        public List<Vector3> MeshVertices { get; set; } = new List<Vector3>();
        public List<BoneWeight> Weights { get; } = new List<BoneWeight>();
        private List<Vector2> MeshUVs { get; set; } = new List<Vector2>();

        public Transform AddBone(Vector3 pos, Transform baseObject, string name, bool dontAddCount = false)
        {
            string boneName;
            if (dontAddCount)
                boneName = name;
            else
                boneName = $"{name}{_bones.Count}";

            Transform bone = new GameObject(boneName).transform;
            bone.parent = baseObject;

            // Set the position relative to the parent
            bone.localRotation = Quaternion.identity;
            bone.localPosition = pos;

            Matrix4x4 matrix4x4 = bone.worldToLocalMatrix;

            _bones.Add(bone);
            _bindPoses.Add(matrix4x4);

            _boneMapping.Add(boneName, _bones.Count - 1);

            return bone;
        }

        public Mesh ApplyMeshDetails(GameObject baseobject, List<Material> AllMaterials, string meshName)
        {
            Mesh mesh = new Mesh
            {
                name = $"{meshName} {DateTime.Now.ToShortDateString()}"
            };
            SkinnedMeshRenderer rend = baseobject.GetComponent<SkinnedMeshRenderer>();
            if (rend == null)
            {
                baseobject.AddComponent<SkinnedMeshRenderer>();
                rend = baseobject.GetComponent<SkinnedMeshRenderer>();
            }
            rend.sharedMesh = mesh;

            List<Vector3> finalVertices = new List<Vector3>();
            foreach (var vert in MeshVertices)
            {
                finalVertices.Add(vert + baseobject.transform.position);
            }

            mesh.vertices = finalVertices.ToArray();
            mesh.uv = MeshUVs.ToArray();

            // Create the material and assign triangles
            Renderer r = baseobject.GetComponent<Renderer>();
            List<Material> materials = new List<Material>();
            int count = 0;
            mesh.subMeshCount = _meshMaterialsTriangles.Count;

            foreach (KeyValuePair<string, List<int>> meshTris in _meshMaterialsTriangles)
            {
                if (meshTris.Value.Count == 0)
                {
                    continue;
                }

                if (!materials.Any(x => x.name == meshTris.Key))
                {
                    Material mat = AllMaterials.Find((m) => m.name == meshTris.Key);
                    materials.Add(mat);
                }

                mesh.SetTriangles(meshTris.Value.ToArray(), count++);
            }

            mesh.subMeshCount = count; // just in case we didn't add all of them
            r.materials = materials.ToArray();

            // Bones and weights
            mesh.boneWeights = Weights.ToArray();
            mesh.bindposes = _bindPoses.ToArray();
            rend.bones = _bones.ToArray();

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public void AssignVerticesToBones(int[] weights, int boneIndex)
        {
            foreach (var weight in weights)
            {
                BoneWeight w = new BoneWeight();
                w.boneIndex0 = boneIndex;
                w.weight0 = 1;

                Weights.Add(w);
            }
        }

        public void AssignVerticesToBones(int[] vectors, string boneName)
        {
            BoneWeightHolder boneWeight = new BoneWeightHolder(boneName, vectors, _boneMapping);
            Weights.AddRange(boneWeight.BoneWeights);
        }

        public int[] BuildFan(Vector3 a, Vector3 b, Vector3 c, Vector3 d, UVSet uvset)
        {
            int indexA = AddVectorUVSets(a, uvset.UVList[UVSet.TL]);
            int indexB = AddVectorUVSets(b, uvset.UVList[UVSet.TR]);
            int indexC = AddVectorUVSets(c, uvset.UVList[UVSet.BL]);
            int indexD = AddVectorUVSets(d, uvset.UVList[UVSet.BR]);

            GetTriangles().AddRange(new int[] { indexA, indexB, indexC });
            GetTriangles().AddRange(new int[] { indexA, indexC, indexD });

            return new int[] { indexA, indexB, indexC, indexD };
        }

        public int[] BuildFanSwap(Vector3 a, Vector3 b, Vector3 c, Vector3 d, UVSet uvset)
        {
            int indexA = AddVectorUVSets(a, uvset.UVList[UVSet.TL]);
            int indexB = AddVectorUVSets(b, uvset.UVList[UVSet.TR]);
            int indexC = AddVectorUVSets(c, uvset.UVList[UVSet.BL]);
            int indexD = AddVectorUVSets(d, uvset.UVList[UVSet.BR]);

            GetTriangles().AddRange(new int[] { indexA, indexC, indexB });
            GetTriangles().AddRange(new int[] { indexA, indexD, indexC });

            return new int[] { indexA, indexB, indexC, indexD };
        }

        public int[] BuildQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, UVSet uvset)
        {
            int indexA = AddVectorUVSets(a, uvset.UVList[UVSet.BR]);
            int indexB = AddVectorUVSets(b, uvset.UVList[UVSet.TR]);
            int indexC = AddVectorUVSets(c, uvset.UVList[UVSet.BL]);
            int indexD = AddVectorUVSets(d, uvset.UVList[UVSet.TL]);

            GetTriangles().AddRange(new int[] { indexA, indexB, indexC });
            GetTriangles().AddRange(new int[] { indexD, indexC, indexB });

            return new int[] { indexA, indexB, indexC, indexD };
        }

        public int[] BuildQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 e, UVSet uvset)
        {
            int indexA = AddVectorUVSets(a, uvset.UVList[UVSet.TL]);
            int indexB = AddVectorUVSets(b, uvset.UVList[UVSet.TR]);
            int indexC = AddVectorUVSets(c, uvset.UVList[UVSet.BL]);
            int indexD = AddVectorUVSets(d, uvset.UVList[UVSet.BR]);
            int indexE = AddVectorUVSets(e, uvset.UVList[UVSet.OtherA]);

            GetTriangles().AddRange(new int[] { indexA, indexB, indexC });
            GetTriangles().AddRange(new int[] { indexA, indexC, indexD });
            GetTriangles().AddRange(new int[] { indexA, indexD, indexE });

            return new int[] { indexA, indexB, indexC, indexD, indexE };
        }

        public int[] BuildQuad(Vector3 a, float width, float height, UVSet uvset, bool flip, string boneName)
        {
            return BuildQuadUp(a, Vector3.forward, width, height, uvset, flip, boneName);
        }

        public int[] BuildQuadForward(Vector3 a, Vector3 direction, float width, float height, UVSet uvset, bool flip, int boneIndex)
        {
            int[] weights;
            Vector3 b = a + Vector3.forward * height;
            Vector3 c = a + direction * width;
            Vector3 d = b + direction * width;

            if (flip)
            {
                weights = BuildQuad(c, d, a, b, uvset);
            }
            else
            {
                weights = BuildQuad(a, b, c, d, uvset);
            }

            AssignVerticesToBones(weights, boneIndex);

            return weights;
        }

        public int[] BuildQuadForward(Vector3 a, Vector3 direction, float width, float height, UVSet uvset, bool flip, string boneName)
        {
            int[] weights;
            Vector3 b = a + Vector3.forward * height;
            Vector3 c = a + direction * width;
            Vector3 d = b + direction * width;

            if (flip)
            {
                weights = BuildQuad(c, d, a, b, uvset);
            }
            else
            {
                weights = BuildQuad(a, b, c, d, uvset);
            }

            AssignVerticesToBones(weights, boneName);

            return weights;
        }

        public int[] BuildQuadMapOrder(Vector3 a, Vector3 b, Vector3 c, Vector3 d, UVSet uvset)
        {
            int indexA = AddVectorUVSets(a, uvset.UVList[UVSet.TL]);
            int indexB = AddVectorUVSets(b, uvset.UVList[UVSet.TR]);
            int indexC = AddVectorUVSets(c, uvset.UVList[UVSet.BL]);
            int indexD = AddVectorUVSets(d, uvset.UVList[UVSet.BR]);

            GetTriangles().AddRange(new int[] { indexA, indexB, indexC });
            GetTriangles().AddRange(new int[] { indexD, indexC, indexB });

            return new int[] { indexA, indexB, indexC, indexD };
        }

        public int[] BuildQuadRight(Vector3 a, Vector3 direction, float width, float height, UVSet uvset, bool flip, int boneIndex)
        {
            int[] weights;
            Vector3 b = a + Vector3.right * height;
            Vector3 c = a + direction * width;
            Vector3 d = b + direction * width;

            if (flip)
            {
                weights = BuildQuad(c, d, a, b, uvset);
            }
            else
            {
                weights = BuildQuad(a, b, c, d, uvset);
            }
            AssignVerticesToBones(weights, boneIndex);

            return weights;
        }

        public int[] BuildQuadSwap(Vector3 a, Vector3 b, Vector3 c, Vector3 d, UVSet uvset)
        {
            int indexA = AddVectorUVSets(a, uvset.UVList[UVSet.TL]);
            int indexB = AddVectorUVSets(b, uvset.UVList[UVSet.TR]);
            int indexC = AddVectorUVSets(c, uvset.UVList[UVSet.BL]);
            int indexD = AddVectorUVSets(d, uvset.UVList[UVSet.BR]);

            GetTriangles().AddRange(new int[] { indexA, indexC, indexB });
            GetTriangles().AddRange(new int[] { indexA, indexD, indexC });

            return new int[] { indexA, indexB, indexC, indexD };
        }

        public int[] BuildQuadUp(Vector3 a, Vector3 direction, float width, float height, UVSet uvset, bool flip, string boneName)
        {
            int[] weights;
            Vector3 b = a + Vector3.up * height;
            Vector3 c = a + direction * width;
            Vector3 d = b + direction * width;

            if (flip)
            {
                weights = BuildQuad(c, d, a, b, uvset);
            }
            else
            {
                weights = BuildQuad(a, b, c, d, uvset);
            }

            AssignVerticesToBones(weights, boneName);

            return weights;
        }

        public BoneWeightHolder CreateBoneWeights(int[] vectors, string boneName)
        {
            BoneWeightHolder boneWeight = new BoneWeightHolder(boneName, vectors, _boneMapping);

            return boneWeight;
        }

        public void CreateMaterialsArray()
        {
            _meshMaterialsTriangles.Add("Base", new List<int>());
        }

        public int GetBoneIndex(string boneName)
        {
            return _boneMapping[boneName];
        }

        public List<int> GetTriangles(string materialName)
        {
            if (string.IsNullOrEmpty(materialName))
                materialName = "Base";

            if (!_meshMaterialsTriangles.ContainsKey(materialName))
                _meshMaterialsTriangles.Add(materialName, new List<int>());

            return _meshMaterialsTriangles[materialName];
        }

        public List<int> GetTriangles()
        {
            return GetTriangles(CurrentMaterialName);
        }

        public void SetMaterial(string name)
        {
            _currentMaterialName = name;
        }

        internal int[] BuildConnectedArray(List<Vector3> bottomArray, List<Vector3> topArray, int start, int end, UVSet uvset)
        {
            List<int> index = new List<int>();
            for (int i2 = start; i2 < end; i2++)
            {
                index.Add(AddVectorUVSets(bottomArray[i2], uvset.UVList[i2]));
                index.Add(AddVectorUVSets(topArray[i2], uvset.UVList[end + i2]));
            }

            for (int i = 0; i < index.Count - 2; i += 2)
            {
                GetTriangles().AddRange(new int[] { index[0 + i], index[2 + i], index[3 + i] });
                GetTriangles().AddRange(new int[] { index[0 + i], index[3 + i], index[1 + i] });
            }
            return index.ToArray();
        }

        internal int[] BuildConnectedArraySwap(List<Vector3> bottomArray, List<Vector3> topArray, int start, int end, UVSet uvset)
        {
            List<int> index = new List<int>();

            for (int i2 = start; i2 < end; i2++)
            {
                index.Add(AddVectorUVSets(bottomArray[i2], uvset.UVList[i2]));
                index.Add(AddVectorUVSets(topArray[i2], uvset.UVList[end + i2]));
            }

            for (int i = 0; i < index.Count - 2; i += 2)
            {
                GetTriangles().AddRange(new int[] { index[0 + i], index[3 + i], index[2 + i] });
                GetTriangles().AddRange(new int[] { index[0 + i], index[1 + i], index[3 + i] });
            }
            return index.ToArray();
        }

        internal List<BoneWeight> BuildQuadGrid(Vector3 startingPos, List<float> acrossSteps, List<float> vertical, bool flip)
        {
            List<BoneWeight> boneWeights = new List<BoneWeight>();
            UVSet pageUVs = BuildUVSet(acrossSteps, vertical);

            List<int> index = new List<int>();
            Vector3 currentPos = startingPos;

            List<Vector3> store = new List<Vector3>();

            int countAcross = vertical.Count;
            for (int y = 0; y < acrossSteps.Count; y++)
            {
                for (int boneIndex = 0; boneIndex < vertical.Count; boneIndex++)
                {
                    float a = 0;
                    float b = 1;
                    float c = 0;

                    if (boneIndex == 0 || boneIndex == 1)
                    {
                        a = 1;
                        b = 0;
                        c = 0;
                    }
                    if (boneIndex == 2)
                    {
                        a = 0.75f;
                        b = 0.25f;
                        c = 0;
                    }
                    if (boneIndex == 3)
                    {
                        a = 0.50f;
                        b = 0.50f;
                        c = 0;
                    }
                    if (boneIndex == 4)
                    {
                        a = 0.25f;
                        b = 0.75f;
                        c = 0;
                    }
                    if (boneIndex == 5)
                    {
                        a = 0f;
                        b = 0.75f;
                        c = 0.25f;
                    }
                    if (boneIndex == 6)
                    {
                        a = 0f;
                        b = 0.50f;
                        c = 0.50f;
                    }
                    if (boneIndex == 7)
                    {
                        a = 0f;
                        b = 0.25f;
                        c = 0.75f;
                    }
                    if (boneIndex == 8 || boneIndex == 9)
                    {
                        a = 0;
                        b = 0;
                        c = 1;
                    }

                    Vector2 uvPossition = pageUVs.UVList[y * countAcross + boneIndex];
                    if (flip)
                    {
                        uvPossition.y = 1 - uvPossition.y;
                    }
                    else
                    {
                        uvPossition.x = 1 - uvPossition.x;
                        uvPossition.y = 1 - uvPossition.y;
                    }

                    index.Add(AddVectorUVSets(currentPos, uvPossition));
                    currentPos.z -= vertical[boneIndex];
                    BoneWeight boneWeight = new BoneWeight();

                    if (a != 0)
                    {
                        boneWeight.weight0 = a;
                        boneWeight.boneIndex0 = _boneMapping[$"BonePageA{y}"];
                        if (b != 0)
                        {
                            boneWeight.weight1 = b;
                            boneWeight.boneIndex1 = _boneMapping[$"BonePageB{y}"];
                        }
                    }
                    else if (b != 0)
                    {
                        boneWeight.weight0 = b;
                        boneWeight.boneIndex0 = _boneMapping[$"BonePageB{y}"];
                        if (c != 0)
                        {
                            boneWeight.weight1 = c;
                            boneWeight.boneIndex1 = _boneMapping[$"BonePageC{y}"];
                        }
                    }
                    else
                    {
                        boneWeight.weight0 = c;
                        boneWeight.boneIndex0 = _boneMapping[$"BonePageC{y}"];
                    }

                    boneWeights.Add(boneWeight);
                }
                currentPos.z += vertical[vertical.Count - 1];
                store.Add(currentPos);
                currentPos.z = startingPos.z;
                currentPos.y += acrossSteps[y];
            }

            for (int indexUp = 0; indexUp < acrossSteps.Count - 1; indexUp++)
            {
                int offSet = indexUp * countAcross;
                for (int indexValue = 0; indexValue < countAcross - 1; indexValue++)
                {
                    if (flip)
                    {
                        GetTriangles().AddRange(new int[] {
                                offSet + index[indexValue],
                                offSet + index[indexValue + countAcross],
                                offSet + index[indexValue + countAcross + 1] });
                        GetTriangles().AddRange(new int[] {
                                offSet + index[indexValue ],
                                offSet + index[indexValue + countAcross + 1],
                                offSet + index[indexValue +1]
                        });
                    }
                    else
                    {
                        GetTriangles().AddRange(new int[] {
                            offSet + index[indexValue],
                            offSet + index[indexValue + 1],
                            offSet + index[indexValue + countAcross] });
                        GetTriangles().AddRange(new int[] {
                            offSet + index[indexValue + 1],
                            offSet + index[indexValue + countAcross + 1],
                            offSet + index[indexValue + countAcross] });
                    }
                }
            }
            return boneWeights;
        }

        internal void Create2Bones(ref BoneWeight boneWeight, string boneDetails)
        {
            BoneWeightHolder.Create2Bones(ref boneWeight, boneDetails, _boneMapping);
        }

        private static UVSet BuildUVSet(List<float> gaps, List<float> vertical)
        {
            UVSet pageUVs = new UVSet(1, 1);
            List<Vector2> vectUVS = new List<Vector2>();
            float y2 = 0;
            foreach (var y in gaps)
            {
                float x2 = 0;
                foreach (var x in vertical)
                {
                    var temp = new Vector2();
                    temp.x = y2;
                    temp.y = x2;

                    x2 += x;

                    vectUVS.Add(temp);
                }
                y2 += y;
            }

            pageUVs.SetUVFromVectors(vectUVS.ToArray());
            return pageUVs;
        }

        private int AddVectorUVSets(Vector3 points, Vector2 uvs)
        {
            MeshVertices.Add(points);
            MeshUVs.Add(uvs);
            return MeshVertices.Count - 1;
        }
    }
}