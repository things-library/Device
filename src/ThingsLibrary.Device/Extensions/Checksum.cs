namespace ThingsLibrary.Device.Extensions
{
    public static class ChecksumExtensions
    {
        /// <summary>
        /// Calculate a checksum value based on the NMEA calculation
        /// </summary>
        /// <param name="sentence">Sentence</param>
        /// <remarks>Sentences must have a $ start character and * end character</remarks>
        /// <returns>Two character hexadecimal checksum value</returns>
        private static string ToChecksum(this string sentence)
        {
            var startPos = sentence.IndexOf('$');
            var endPos = sentence.IndexOf('*', startPos);

            // make sure we actually have a full sentence
            if (startPos < 0 || endPos < 0) { return string.Empty; }

            //Start with first Item
            int checksum = Convert.ToByte(sentence[startPos + 1]);

            // Loop through all chars to get a checksum
            for (int i = startPos + 2; i < endPos; i++)
            {
                // No. XOR the checksum with this character's value
                checksum ^= Convert.ToByte(sentence[i]);
            }

            // Return the checksum formatted as a two-character hexadecimal
            return checksum.ToString("X2");
        }

        private static bool ValidateChecksum(this string sentence)
        {
            //TODO:
            return false;
        }

        public static long ToEpochMs(DateTime dateTime) => (long)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds * 100;

    }
}
