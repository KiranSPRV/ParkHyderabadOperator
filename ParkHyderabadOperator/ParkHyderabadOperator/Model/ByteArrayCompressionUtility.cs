using System;
using System.IO;
using System.IO.Compression;


namespace ParkHyderabadOperator.Model
{
   public static class ByteArrayCompressionUtility
    {
        static int inputByteSize = 0;
        private static int BUFFER_SIZE = 100 * 1024; //64kB
        public static byte[] Compress(byte[] inputData)
        {
          
            if (inputData == null)
                throw new ArgumentNullException("inputData must be non-null");

            using (var compressIntoMs = new MemoryStream())
            {
                using (var gzs = new BufferedStream(new GZipStream(compressIntoMs,
                 CompressionMode.Compress), BUFFER_SIZE))
                {
                    gzs.Write(inputData, 0, inputData.Length);
                }
                return compressIntoMs.ToArray();
            }
        }
        public static byte[] Decompress(byte[] inputData)
        {
            if (inputData == null)
                throw new ArgumentNullException("inputData must be non-null");

            using (var compressedMs = new MemoryStream(inputData))
            {
                using (var decompressedMs = new MemoryStream())
                {
                    using (var gzs = new BufferedStream(new GZipStream(compressedMs, 
                     CompressionMode.Decompress), BUFFER_SIZE))
                    {
                        gzs.CopyTo(decompressedMs);
                    }
                    return decompressedMs.ToArray(); 
                }
            }
        }

        public static byte[] GetImageStreamAsBytes(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }


    }
}
