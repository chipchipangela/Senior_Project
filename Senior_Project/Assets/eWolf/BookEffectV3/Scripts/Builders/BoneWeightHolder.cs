using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace BookEffectV3.Builder
{
    public class BoneWeightHolder
    {
        public BoneWeightHolder(string bones, int[] vectors, Dictionary<string, int> _boneMapping)
        {
            if (bones == "BoneCover0"
               || bones == "BoneCover1"
               || bones == "BoneCover2"
               || bones == "BonePageA0")
            {
                bones += ":1";
                var temp = bones;
                for (int i = 0; i < vectors.Length - 1; i++)
                {
                    bones += $",{temp}";
                }
            }

            BoneWeights = new BoneWeight[vectors.Length];
            for (int i = 0; i < vectors.Length - 1; i++)
            {
                BoneWeights[i] = new BoneWeight();
            }

            string[] names = bones.Split(',');

            int boneIndex = 0;
            foreach (string n in names)
            {
                string[] boneSet = n.Split('|');

                int boneSetIndex = 0;
                foreach (string b in boneSet)
                {
                    string[] nameWeightPairs = b.Split(':');

                    if (boneSetIndex == 0)
                    {
                        BoneWeights[boneIndex].boneIndex0 = _boneMapping[nameWeightPairs[0]];
                        BoneWeights[boneIndex].weight0 = float.Parse(nameWeightPairs[1], CultureInfo.InvariantCulture);
                    }
                    if (boneSetIndex == 1)
                    {
                        BoneWeights[boneIndex].boneIndex1 = _boneMapping[nameWeightPairs[0]];
                        BoneWeights[boneIndex].weight1 = float.Parse(nameWeightPairs[1], CultureInfo.InvariantCulture);
                    }
                    boneSetIndex++;
                }
                boneIndex++;
            }
        }

        public BoneWeight[] BoneWeights { get; }

        public static void Create2Bones(ref BoneWeight boneWeight, string boneDetails, Dictionary<string, int> _boneMapping)
        {
            string[] boneSet = boneDetails.Split('|');

            int boneSetIndex = 0;
            foreach (string b in boneSet)
            {
                string[] nameWeightPairs = b.Split(':');

                if (boneSetIndex == 0)
                {
                    boneWeight.boneIndex0 = _boneMapping[nameWeightPairs[0]];
                    boneWeight.weight0 = float.Parse(nameWeightPairs[1], CultureInfo.InvariantCulture);
                }
                if (boneSetIndex == 1)
                {
                    boneWeight.boneIndex1 = _boneMapping[nameWeightPairs[0]];
                    boneWeight.weight1 = float.Parse(nameWeightPairs[1], CultureInfo.InvariantCulture);
                }
                boneSetIndex++;
            }
        }
    }
}