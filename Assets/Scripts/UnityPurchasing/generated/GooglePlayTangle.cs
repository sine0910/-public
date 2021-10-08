// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("1yl0fBcAa1b/fdmdY8P1GvLkRKRDKPE8W73SSAMPlmrw4n1LsqzTwJk5eVMiNrSzndi4MyuKwAHS+l6i3zd8JyA5aFblgQfPQ4xc93cQZ2v0Oyv8Dy11q+yFaGWy1ZDWnLDZlksVgez51XWXGbkzNc7wSawXBWWSNpgKTATW4yJKrNh3mMvl8rKHLjDOY/l5OMB+sE4myEiDht0bkRm408nj8vLctIAp/gvTDoruPeXsz7N7nB8RHi6cHxQcnB8fHrkrpq/y7c8unB88LhMYFzSYVpjpEx8fHxseHTH9OapK9IXACawITIBmh1NEyr2nRIFJf505JjYwtBj310vqVmTlWxaBUDv88L5imisMavIMBwtcMAeE/iScQTnDHOlbPRwdHx4f");
        private static int[] order = new int[] { 11,3,7,4,7,12,9,9,8,9,11,11,12,13,14 };
        private static int key = 30;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
