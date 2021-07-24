#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("DowdFTLigVb5Ls81h0eTaIgBbahqxtQU5nSawzI/tyuFTvfeKGj5/ltDCO/lxZqlALk1v0Z2CV5vFj54mezv1lKnn5ev2HcZcm1HL8yaexlU0tIV0KX4OG7Kz9S17HhIcdj1+iirpaqaKKugqCirq6oXencPGd3H1tybtZMrjkvKvtKeVqMbDPs/MXs1yEX9JLt3FRwpE5BHagQ9wAxfpBTCWD7heQE2uLiEmGHYFjWa6GV99np+jFmb4z7jX6Jdf2fnVDrnhJQONbAA98+Yb5akprWiRI/ICQ0hmFq342GxuyEehgs5cLdDZJP4CN7quzoRjVD7RfJhH8krfgO1Su9dCDGaKKuImqeso4As4ixdp6urq6+qqY0S7pPAU81lf6ipq6qr");
        private static int[] order = new int[] { 6,11,4,3,13,11,12,10,9,13,10,11,13,13,14 };
        private static int key = 170;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
