using System.Collections.Generic;
using System.Linq;

namespace Pantheon.Utils
{
    static class PantheonExtensions
    {   
        ///<summary>
        /// Uses Unity Randoms to select any object out of the list
        /// </summary>
        public static T Any<T>(this List<T> list)
        {
            int i = UnityEngine.Random.Range(0, list.Count);
            return list.ElementAt(i);
        }
    }
}
