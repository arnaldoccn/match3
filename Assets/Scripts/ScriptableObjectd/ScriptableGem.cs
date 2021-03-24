using UnityEngine;
namespace MatchThree
{
    [CreateAssetMenu(fileName = "Gem", menuName = "ScriptableObjects/GemScriptableObject", order = 1)]
    public class ScriptableGem : ScriptableObject
    {
        public Gem.GemType GemType;
        public int Score;
    }
}