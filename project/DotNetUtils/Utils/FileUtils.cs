using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DotNetUtils
{
    public static class FileUtils
    {
        /// <summary>
        /// Load a file as xml
        /// </summary>
        /// <param name="filePath">The path of xml file</param>
        /// <returns>
        ///     XElement represents xml
        /// </returns>
        public static XElement LoadFileAsXml(this string filePath)
        {
            if (filePath.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException("filePath");
            }
            if (!File.Exists(filePath))
            {
                throw new ArgumentNullException("Cannot find file with path '{0}'".FormatInvariantCulture(filePath));
            }
            using (var reader = File.OpenText(filePath))
            {
                return XElement.Load(reader);
            }
        }

        /// <summary>
        /// Function to get byte array from a file. 
        /// </summary>
        /// <param name="fileName">File name to get byte array</param>
        /// <returns>Byte Array</returns>
        public static byte[] FileToByteArray(this string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new ArgumentNullException("Cannot find file with path '{0}'".FormatInvariantCulture(fileName));
            }
            byte[] buffer = null;
            BinaryReader binaryReader = null;
            try
            {
                using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    binaryReader = new BinaryReader(fileStream);
                    long totalBytes = new FileInfo(fileName).Length;
                    buffer = binaryReader.ReadBytes((Int32)totalBytes);
                    fileStream.Close();
                    fileStream.Dispose();
                }
                binaryReader.Close();
            }
            catch (Exception exception)
            {
                throw new Exception("Exception caught in process: {0}".FormatInvariantCulture(exception.ToString()));
            }
            finally
            {
                if (binaryReader != null)
                {
                    binaryReader.Dispose();
                }
            }
            return buffer;
        }
    }
}
