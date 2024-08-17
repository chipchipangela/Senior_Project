using UnityEngine;

namespace eWolf.BookEffectV2.Interfaces
{
    public interface IBookGameObjects
    {
        GameObject BookCoverAnimation { get; set; }
        GameObject BookCoverMesh { get; set; }
        GameObject PageAnimation { get; set; }
        GameObject PageMesh { get; set; }
    }
}