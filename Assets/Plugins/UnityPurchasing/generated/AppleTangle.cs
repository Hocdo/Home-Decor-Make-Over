#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class AppleTangle
    {
        private static byte[] data = System.Convert.FromBase64String("Y28sKj07JikmLC47Km8/ICMmLDZHZElOSkpITU5ZUSc7Oz88dWBgOHnWA2I3+KLD1JO8ONS9OZ04fwCOSKMydszEHG+cd4v+8NUARSSwZLNyaShvxXwluELNgJGk7GC2HCUUKzg4YS4/PyMqYSwgImAuPz8jKiwu8bs81KGdK0CENgB7l+1xtjewJIfPW2SfJgjbOUaxuyTCYQ/puAgCMAoxUAMkH9kOxos7LURfzA7IfMXOxFbGkbYEI7pI5G1/TadXcbcfRpwdKiMmLiEsKm8gIW87JyY8bywqPQaXOdB8WyruONuGYk1MTk9O7M1OOycgPSY7Nn5Zf1tJTBpLTFxCDj9vIClvOycqbzsnKiFvLj8/IyYsLj8jKm8dICA7bwwOf1FYQn95f3t92tE1Q+sIxBSbWXh8hItAAoFbJp5Jf0BJTBpSXE5OsEtKf0xOTrB/UuTsPt0IHBqO4GAO/Le0rD+CqewDSk9MzU5AT3/NTkVNzU5OT6ve5kb6deK7QEFP3UT+bllhO5pzQpQtWWutpJ74P5BACq5ohb4iN6Ko+lhYf15JTBpLRVxFDj8/IypvBiEsYX5Q3pRRCB+kSqIRNstipHntGAMao3/NS/R/zUzs70xNTk1NTk1/QklGOyYpJiwuOypvLTZvLiE2bz8uPTsremxaBFoWUvzbuLnT0YAf9Y4XH2XJB8m4Qk5OSkpPfy1+RH9GSUwaKMBH+2+4hONjbyA/+XBOf8P4DIAtIypvPDsuISsuPStvOyo9IjxvLsA8zi+JVBRGYN39twsHvy930Vq6Nm8uPDw6Iio8by4sLCo/Oy4hLCojKm8GISxhfml/a0lMGktEXFIOP2B/zoxJR2RJTkpKSE1Nf875Vc78S0lcTRocflx/XklMGktFXEUOPz9CSUZlyQfJuEJOTkpKT0zNTk5PEz8jKm8MKj07JikmLC47JiAhbw46UMrMylTWcgh4vebUD8Fjm/7fXZd8eRV/LX5Ef0ZJTBpLSVxNGhx+XM1OT0lGZckHybgsK0pOf869f2VJQNJyvGQGZ1WHsYH69kGWEVOZhHL4VPLcDWtdZYhAUvkC0xEshwTPWI8sfDi4dUhjGaSVQG5BlfU8VgD6IStvLCAhKyY7JiAhPG8gKW86PCpHEX/NTl5JTBpSb0vNTkd/zU5Lf2l/a0lMGktEXFIOPz8jKm8MKj07/n8XoxVLfcMn/MBSkSo8sCgRKvOGVj26EkGaMBDUvWpM9RrAAhJCvueTMW16hWqalkCZJJvta2xeuO7jby4hK28sKj07JikmLC47JiAhbz+WeTCOyBqW6Nb2fQ20l5o+0THuHTAO59e2noUp02skXp/s9KtUZYxQWX9bSUwaS0xcQg4/PyMqbx0gIDsW6EpGM1gPGV5RO5z4xGx0COyaICYpJiwuOyYgIW8OOjsnID0mOzZ+NX/NTjl/QUlMGlJATk6wS0tMTU5vDA5/zU5tf0JJRmXJB8m4Qk5OTklMGlJBS1lLW2SfJgjbOUaxuyTCen1+e398eRVYQnx6f31/dn1+e39hD+m4CAIwRxF/UElMGlJsS1d/WT0uLDsmLCpvPDsuOyoiKiE7PGF/H+XFmpWrs59GSHj/Ojpu");
        private static int[] order = new int[] { 44,35,48,51,56,36,50,32,25,16,38,27,36,41,50,18,29,24,50,23,53,29,50,44,39,53,35,29,42,55,40,51,37,48,47,56,49,53,47,50,59,57,43,48,47,56,47,54,56,58,50,58,52,55,56,57,59,57,58,59,60 };
        private static int key = 79;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
