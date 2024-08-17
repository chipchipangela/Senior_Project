using UnityEngine;

namespace eWolf.BookEffectV2.Interfaces
{
    public interface IBookMaterials
    {
        Material InsideBackCover { get; set; }
        Material InsideFrontCover { get; set; }
        Material PageBack { get; set; }
        Material PageFront { get; set; }
    }
}