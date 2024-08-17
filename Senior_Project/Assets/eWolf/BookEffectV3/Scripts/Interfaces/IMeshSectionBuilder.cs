using BookEffectV3.Builder;

namespace BookEffectV3.Interfaces
{
    public interface IMeshSectionBuilder
    {
        MeshBuilder MeshBuilder { get; }

        void Build();
    }
}