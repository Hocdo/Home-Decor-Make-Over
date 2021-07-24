using System;
using System.Security.Cryptography;
using System.Text;

public class AESWrapper
{
	public static string AESEncrypt128(byte[] Input, string key)
	{
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.Mode = CipherMode.ECB;
		rijndaelManaged.Padding = PaddingMode.PKCS7;
		rijndaelManaged.KeySize = 128;
		rijndaelManaged.BlockSize = 128;
		byte[] bytes = Encoding.UTF8.GetBytes(key);
		byte[] array = new byte[16];
		int num = bytes.Length;
		if (num > array.Length)
		{
			num = array.Length;
		}
		Array.Copy(bytes, array, num);
		rijndaelManaged.Key = array;
		rijndaelManaged.IV = array;
		ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor();
		return BitConverter.ToString(cryptoTransform.TransformFinalBlock(Input, 0, Input.Length)).Replace("-", string.Empty).ToLower();
	}

	public static string AESEncrypt128(string Input, string key)
	{
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.Mode = CipherMode.ECB;
		rijndaelManaged.Padding = PaddingMode.PKCS7;
		rijndaelManaged.KeySize = 128;
		rijndaelManaged.BlockSize = 128;
		byte[] bytes = Encoding.UTF8.GetBytes(key);
		byte[] array = new byte[16];
		int num = bytes.Length;
		if (num > array.Length)
		{
			num = array.Length;
		}
		Array.Copy(bytes, array, num);
		rijndaelManaged.Key = array;
		rijndaelManaged.IV = array;
		ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor();
		byte[] bytes2 = Encoding.UTF8.GetBytes(Input);
		return BitConverter.ToString(cryptoTransform.TransformFinalBlock(bytes2, 0, bytes2.Length)).Replace("-", string.Empty).ToLower();
	}

	public static byte[] AESEncryptByte128(string Input, string key)
	{
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.Mode = CipherMode.ECB;
		rijndaelManaged.Padding = PaddingMode.PKCS7;
		rijndaelManaged.KeySize = 128;
		rijndaelManaged.BlockSize = 128;
		byte[] bytes = Encoding.UTF8.GetBytes(key);
		byte[] array = new byte[16];
		int num = bytes.Length;
		if (num > array.Length)
		{
			num = array.Length;
		}
		Array.Copy(bytes, array, num);
		rijndaelManaged.Key = array;
		rijndaelManaged.IV = array;
		ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor();
		byte[] bytes2 = Encoding.UTF8.GetBytes(Input);
		return cryptoTransform.TransformFinalBlock(bytes2, 0, bytes2.Length);
	}

	public static int GetHexVal(char hex)
	{
		return hex - ((hex >= ':') ? 55 : 48);
	}

	public static byte[] toByteArray(string hex)
	{
		hex = hex.ToUpper();
		byte[] array = new byte[hex.Length >> 1];
		for (int i = 0; i < hex.Length >> 1; i++)
		{
			array[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + GetHexVal(hex[(i << 1) + 1]));
		}
		return array;
	}

	public static byte[] AESDecrypt128(string Input, string key)
	{
		Input = Input.Trim();
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.Mode = CipherMode.ECB;
		rijndaelManaged.Padding = PaddingMode.PKCS7;
		rijndaelManaged.KeySize = 128;
		rijndaelManaged.BlockSize = 128;
		byte[] array = toByteArray(Input);
		byte[] bytes = Encoding.UTF8.GetBytes(key);
		byte[] array2 = new byte[16];
		int num = bytes.Length;
		if (num > array2.Length)
		{
			num = array2.Length;
		}
		Array.Copy(bytes, array2, num);
		rijndaelManaged.Key = array2;
		rijndaelManaged.IV = array2;
		return rijndaelManaged.CreateDecryptor().TransformFinalBlock(array, 0, array.Length);
	}

	public static byte[] AESDecryptByte128(string Input, string key)
	{
		Input = Input.Trim();
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.Mode = CipherMode.ECB;
		rijndaelManaged.Padding = PaddingMode.PKCS7;
		rijndaelManaged.KeySize = 128;
		rijndaelManaged.BlockSize = 128;
		byte[] array = toByteArray(Input);
		byte[] bytes = Encoding.UTF8.GetBytes(key);
		byte[] array2 = new byte[16];
		int num = bytes.Length;
		if (num > array2.Length)
		{
			num = array2.Length;
		}
		Array.Copy(bytes, array2, num);
		rijndaelManaged.Key = array2;
		rijndaelManaged.IV = array2;
		return rijndaelManaged.CreateDecryptor().TransformFinalBlock(array, 0, array.Length);
	}
}
