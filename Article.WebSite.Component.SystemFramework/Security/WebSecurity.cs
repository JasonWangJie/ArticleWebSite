using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;

namespace Article.WebSite.Component.SystemFramework.Security
{
	/// <summary>
	/// 站点安全性相关类
	/// </summary>
	public static class WebSecurity
	{
		#region 二进制转换
		public static string BinaryToHex(byte[] data)
		{
			return BinaryToHex(data, false);
		}

		public static string BinaryToHex(byte[] data, bool lower)
		{
			if (data == null)
				return null;

			char[] chArray = new char[data.Length * 2];
			for (int i = 0; i < data.Length; i++)
			{
				byte num2 = data[i];
				chArray[2 * i] = NibbleToHex((byte)(num2 >> 4), lower);
				chArray[(2 * i) + 1] = NibbleToHex((byte)(num2 & 15), lower);
			}
			return new string(chArray);
		}
		private static char NibbleToHex(byte nibble, bool lower)
		{
			return nibble < 10 ? (char)(nibble + 0x30) : (lower ? (char)(nibble - 10 + 0x61) : (char)(nibble - 10 + 0x41));
		}
		public static byte[] HexToBinary(string inputString)
		{
			if (inputString == null || inputString.Length == 0)
				return null;

			var length = inputString.Length;
			if (length % 2 == 1)
				throw new ArgumentException("输入参数的长度异常，必须是2的倍数");

			if (!Regex.IsMatch(inputString, "^[0-9A-Fa-f]*$"))
				throw new ArgumentException("输入字符串格式错误");

			length = length / 2;
			byte[] result = new byte[length];
			char[] achar = inputString.ToCharArray();
			for (var index = 0; index < length; index++)
			{
				int position = index * 2;
				result[index] = (byte)(HexToNibble(achar[position]) << 4 | HexToNibble(achar[position + 1]));
			}
			return result;
		}
		private static byte HexToNibble(char ch)
		{
			return (byte)((ch - 0x30 < 10) ? (ch - 0x30) : ((ch - 0x41 < 27) ? (ch - 0x41 + 10) : (ch - 0x61 + 10)));
		}
		#endregion

		#region Hash
		/// <summary>Hash加密</summary>
		/// <param name="inputString">输入内容</param>
		/// <returns>Hash结果</returns>
		public static string Hash(string inputString)
		{
			return Hash(inputString, HashTypes.MD5, Encoding.UTF8, false);
		}

		/// <summary>Hash加密</summary>
		/// <param name="inputString">输入内容</param>
		/// <param name="lower">输出是否小写</param>
		/// <returns>Hash结果</returns>
		public static string Hash(string inputString, bool lower)
		{
			return Hash(inputString, HashTypes.MD5, Encoding.UTF8, lower);
		}

		/// <summary>Hash加密</summary>
		/// <param name="inputString">输入内容</param>
		/// <param name="hashType">Hash类型</param>
		/// <returns>Hash结果</returns>
		public static string Hash(string inputString, HashTypes hashType)
		{
			return Hash(inputString, hashType, Encoding.UTF8, false);
		}

		/// <summary>Hash加密</summary>
		/// <param name="inputString">输入内容</param>
		/// <param name="hashType">Hash类型</param>
		/// <param name="lower">输出是否小写</param>
		/// <returns>Hash结果</returns>
		public static string Hash(string inputString, HashTypes hashType, bool lower)
		{
			return Hash(inputString, hashType, Encoding.UTF8, lower);
		}

		/// <summary>Hash加密</summary>
		/// <param name="inputString">输入内容</param>
		/// <param name="encoding">编码类型</param>
		/// <returns>Hash结果</returns>
		public static string Hash(string inputString, Encoding encoding)
		{
			return Hash(inputString, HashTypes.MD5, encoding, false);
		}

		/// <summary>Hash加密</summary>
		/// <param name="inputString">输入内容</param>
		/// <param name="encoding">编码类型</param>
		/// <param name="lower">输出是否小写</param>
		/// <returns>Hash结果</returns>
		public static string Hash(string inputString, Encoding encoding, bool lower)
		{
			return Hash(inputString, HashTypes.MD5, encoding, lower);
		}

		/// <summary>Hash加密</summary>
		/// <param name="inputString">输入内容</param>
		/// <param name="hashType">Hash类型</param>
		/// <param name="encoding">编码类型</param>
		/// <param name="lower">输出是否小写</param>
		/// <returns>Hash结果</returns>
		public static string Hash(string inputString, HashTypes hashType, Encoding encoding, bool lower)
		{
			if (inputString == null)
				return null;

			return BinaryToHex(Hash(encoding.GetBytes(inputString), hashType), lower);
		}

		/// <summary>Hash加密</summary>
		/// <param name="buffer">输入内容</param>
		/// <returns>Hash结果</returns>
		public static byte[] Hash(byte[] buffer)
		{
			return Hash(buffer, HashTypes.MD5);
		}

		/// <summary>Hash加密</summary>
		/// <param name="buffer">输入内容</param>
		/// <param name="hashType">Hash类型</param>
		/// <returns>Hash结果</returns>
		public static byte[] Hash(byte[] buffer, HashTypes hashType)
		{
			if (buffer == null)
				return null;

			HashAlgorithm hashAlgorithm;
			switch (hashType)
			{
				case HashTypes.SHA1:
					hashAlgorithm = new SHA1CryptoServiceProvider();
					break;
				case HashTypes.SHA256:
					hashAlgorithm = new SHA256Managed();
					break;
				case HashTypes.SHA384:
					hashAlgorithm = new SHA384Managed();
					break;
				case HashTypes.SHA512:
					hashAlgorithm = new SHA512Managed();
					break;
				case HashTypes.MD5:
				default:
					hashAlgorithm = new MD5CryptoServiceProvider();
					break;
			}

			return hashAlgorithm.ComputeHash(buffer);
		}
		#endregion

		#region 对称加密
		#region GetSymmetricAlgorithm
		/// <summary>获取对称算法</summary>
		/// <param name="type">算法类型</param>
		/// <param name="key">key</param>
		/// <param name="iv">iv</param>
		/// <returns>算法</returns>
		public static SymmetricAlgorithm GetSymmetricAlgorithm(string key, string iv)
		{
			return GetSymmetricAlgorithm(SymmetricAlgorithmTypes.AES, key, iv);
		}

		/// <summary>获取对称算法</summary>
		/// <param name="type">算法类型</param>
		/// <param name="key">key</param>
		/// <param name="iv">iv</param>
		/// <returns>算法</returns>
		public static SymmetricAlgorithm GetSymmetricAlgorithm(SymmetricAlgorithmTypes type, string key, string iv)
		{
			SymmetricAlgorithm algorithm = null;
			switch (type)
			{
				case SymmetricAlgorithmTypes.AES:
					algorithm = new AesCryptoServiceProvider();
					break;
				case SymmetricAlgorithmTypes.DES:
					algorithm = new DESCryptoServiceProvider();
					break;
				case SymmetricAlgorithmTypes.RC2:
					algorithm = new RC2CryptoServiceProvider();
					break;
				case SymmetricAlgorithmTypes.TripleDES:
					algorithm = new TripleDESCryptoServiceProvider();
					break;
				case SymmetricAlgorithmTypes.Rijndael:
					algorithm = new RijndaelManaged();
					break;
				default:
					algorithm = new AesCryptoServiceProvider();
					break;
			}

			algorithm.Key = Convert.FromBase64String(key);
			algorithm.IV = Convert.FromBase64String(iv);

			return algorithm;
		} 
		#endregion

		#region Decrypt
		public static string Decrypt(this SymmetricAlgorithm algorithm, string base64String)
		{
			return Encoding.UTF8.GetString(Decrypt(algorithm, Convert.FromBase64String(base64String)));
		}
		public static byte[] Decrypt(this SymmetricAlgorithm algorithm, byte[] encryptedBuffer)
		{
			using (var stream = new MemoryStream())
			using (var cs = new CryptoStream(stream, algorithm.CreateDecryptor(), CryptoStreamMode.Write))
			{
				cs.Write(encryptedBuffer, 0, encryptedBuffer.Length);
				cs.FlushFinalBlock();

				return stream.ToArray();
			}
		}
		#endregion

		#region Encrypt
		public static string Encrypt(this SymmetricAlgorithm algorithm, string input)
		{
			return Convert.ToBase64String(Encrypt(algorithm, Encoding.UTF8.GetBytes(input)));
		}
		public static byte[] Encrypt(this SymmetricAlgorithm algorithm, byte[] buffer)
		{
			using (var stream = new MemoryStream())
			using (var cs = new CryptoStream(stream, algorithm.CreateEncryptor(), CryptoStreamMode.Write))
			{
				cs.Write(buffer, 0, buffer.Length);
				cs.FlushFinalBlock();

				return stream.ToArray();
			}
		} 
		#endregion
		#endregion

		#region ValidatePassword
		public static bool ValidatePassword(string password, string hashedPassword)
		{
			if (hashedPassword == null || hashedPassword.Length < 8)
				return false;

			string saltValue = hashedPassword.Substring(0, 8);

			return hashedPassword == HashPassword(password, saltValue, HashTypes.SHA256) || hashedPassword == HashPassword(password, saltValue, HashTypes.MD5);
		}

		public static string HashPassword(string password)
		{
			return HashPassword(password, GenerateSaltValue(), HashTypes.SHA256);
		}

		private static string HashPassword(string password, string saltValue, HashTypes hashType)
		{
			if ((password == null) || (saltValue == null))
				return null;

			var saltBuffer = new byte[] { 
				byte.Parse(saltValue.Substring(0, 2), NumberStyles.HexNumber),
				byte.Parse(saltValue.Substring(2, 2), NumberStyles.HexNumber),
				byte.Parse(saltValue.Substring(4, 2), NumberStyles.HexNumber),
				byte.Parse(saltValue.Substring(6, 2), NumberStyles.HexNumber) };

			var passwordBuffer = Encoding.Unicode.GetBytes(password);

			var hashBuffer = new byte[4 + passwordBuffer.Length];
			saltBuffer.CopyTo(hashBuffer, 0);
			passwordBuffer.CopyTo(hashBuffer, 4);

			return (saltValue + BinaryToHex(Hash(hashBuffer, hashType)));
		}

		private static string GenerateSaltValue()
		{
			var buffer = new byte[4];
			new Random((int)DateTime.Now.Ticks).NextBytes(buffer);

			return BinaryToHex(buffer);
		}
		#endregion
	}

	/// <summary>HashType</summary>
	public enum HashTypes
	{
		/// <summary>MD5</summary>
		MD5,
		/// <summary>SHA1</summary>
		SHA1,
		/// <summary>SHA256</summary>
		SHA256,
		/// <summary>SHA384</summary>
		SHA384,
		/// <summary>SHA512</summary>
		SHA512
	}

	public enum SymmetricAlgorithmTypes
	{
		AES,
		DES,
		RC2,
		TripleDES,
		Rijndael
	}
}
