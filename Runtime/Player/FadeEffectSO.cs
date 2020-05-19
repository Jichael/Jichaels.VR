using Sirenix.OdinInspector;
using UnityEngine;

namespace Jichaels.VRSDK
{
    [CreateAssetMenu(menuName = "FadeEffect")]
    public class FadeEffectSO : ScriptableObject
    {
        public bool fadeInAndOut;

        public float durationAB;

        [ShowIf("fadeInAndOut")]

        public float durationBC;

        public float delayStart;
        
        [ShowIf("fadeInAndOut")] 

        public float delayAfterB;


        public float delayEnd;

        public Color colorA;
        public Color colorB;
        
        [ShowIf("fadeInAndOut")] 

        public Color colorC;
    }
}