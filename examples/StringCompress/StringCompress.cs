using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Extension;
using System.IO;
using System.IO.Compression;

namespace Glare.VX
{
	/// <summary>
	/// Compression Method
	/// </summary>
	public enum CompressionMethodEnum
	{
		/// <summary>
		/// No compression, used to disable compression.
		/// </summary>
		None,

		/// <summary>
		/// GZip compression
		/// </summary>
		GZip,

		/// <summary>
		/// Deflate algorithm
		/// </summary>
		Deflate
	}

	[VxCategories("Control", "Input", "Output")]

	[VxToolBox("String Compress VX", "Text", "Compress", "Compress a given string. The result is then base64-encoded.")]
	[VxIcon("NodeIcons.Logic.Expressions")]

	[VxDescriptionAttribute("The compression method to be used", "Method")]
	[VxCategory("Control", "Method")]
	[VxDefaultValue(Glare.VX.CompressionMethodEnum.GZip, "Method")]

	[VxDescriptionAttribute("The text that will be compressed", "Text")]
	[VxCategory("Input", "Text")]
	
	[VxCategory("Output", "Compressed")]
	[VxDescriptionAttribute("The compressed and then base64-encoded text", "Compressed")]
	public class StringCompress : VxContentNode
	{
		/// <summary>
		/// Compress a given string and then base64 encode it.
		/// </summary>
		/// <param name="text">The text to be compressed.</param>
		/// <param name="type">The compression type to be used.</param>
		/// <returns>A base64-encoded compressed version of <paramref name="text"/></returns>
		protected string CompressString(string text, CompressionMethodEnum type)
		{
			// avoid compressing thin air
			if (string.IsNullOrEmpty(text)) return string.Empty;

			// not enabled
			if(type == CompressionMethodEnum.None) return text;

			try
			{
				byte[] buffer = Encoding.UTF8.GetBytes(text);
				var memoryStream = new MemoryStream();

				switch (type)
				{
					case CompressionMethodEnum.GZip:
						using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
						{
							gZipStream.Write(buffer, 0, buffer.Length);
						}
						break;
					case CompressionMethodEnum.Deflate:
						using (var deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress, true))
						{
							deflateStream.Write(buffer, 0, buffer.Length);
						}
						break;
				}
				

				memoryStream.Position = 0;

				var compressedData = new byte[memoryStream.Length];
				memoryStream.Read(compressedData, 0, compressedData.Length);

				var compressedBuffer = new byte[compressedData.Length + 4];
				Buffer.BlockCopy(compressedData, 0, compressedBuffer, 4, compressedData.Length);
				Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, compressedBuffer, 0, 4);
				
				return Convert.ToBase64String(compressedBuffer);
			}
			catch (Exception ex)
			{
			 	Ventuz.Extension.VX.Log.Error("Glare.VX.StringCompress", $"Could not compress string: {ex.Message}");
				return string.Empty;
			}
		}

		protected string ValidateCompressed(string Text, CompressionMethodEnum Method) => CompressString(Text, Method);
    }

	[VxCategories("Control", "Input", "Output")]
	
	[VxToolBox("String Decompress VX", "Text", "Decompress", "Base64-decode a given string and then decompress it.")]
	[VxIcon("NodeIcons.Logic.Expressions")]

	[VxDescriptionAttribute("The decompression method to be used", "Method")]
	[VxCategory("Control", "Method")]
	[VxDefaultValue(CompressionMethodEnum.GZip, "Method")]

	[VxDescriptionAttribute("The base64-encoded data that will be decompressed", "Compressed")]
	[VxCategory("Input", "Compressed")]

	[VxCategory("Output", "Text")]
	[VxDescriptionAttribute("The compressed and then base64-encoded text", "Text")]
	public class StringDecompress : VxContentNode
	{
		/// <summary>
		/// Base64-decode a gfiven string and then gzip-decompress it.
		/// </summary>
		/// <param name="compressed">The compressed and base64-encoded data.</param>
		/// <returns>The decompressed text</returns>
		protected string DecompressString(string compressed, CompressionMethodEnum type)
		{
			// avoid compressing thin air
			if (string.IsNullOrEmpty(compressed)) return string.Empty;

			// not enabled
			if (type == CompressionMethodEnum.None) return compressed;

			try
			{
				byte[] rawBuffer = Convert.FromBase64String(compressed);
				using (var memoryStream = new MemoryStream())
				{
					int dataLength = BitConverter.ToInt32(rawBuffer, 0);
					memoryStream.Write(rawBuffer, 4, rawBuffer.Length - 4);

					var buffer = new byte[dataLength];

					memoryStream.Position = 0;
					switch (type)
					{
						
						case CompressionMethodEnum.GZip:
							using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
							{
								gZipStream.Read(buffer, 0, buffer.Length);
							}
							break;
						
						case CompressionMethodEnum.Deflate:
							using (var deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress))
							{
								deflateStream.Read(buffer, 0, buffer.Length);
							}
							break;
						
					}
					

					return Encoding.UTF8.GetString(buffer);
				}
			}
			catch (Exception ex)
			{
                Ventuz.Extension.VX.Log.Error("Glare.VX.StringDecompress", $"Could not decompress string: {ex.Message}");
				return string.Empty;
			}
		}

		protected string ValidateText(string Compressed, CompressionMethodEnum Method) => DecompressString(Compressed, Method);
	}
}
