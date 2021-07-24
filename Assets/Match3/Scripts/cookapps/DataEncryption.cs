using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace cookapps
{
	public class DataEncryption
	{
		private static readonly string key = "j*p6aPrcHuc2u8!k7c2pTsag3esWe4ha";

		public static string EncryptString(string toEncrypt)
		{
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(key);
                byte[] bytes2 = Encoding.UTF8.GetBytes(toEncrypt);
                RijndaelManaged rijndaelManaged = new RijndaelManaged();
                rijndaelManaged.Key = bytes;
                rijndaelManaged.Mode = CipherMode.ECB;
                rijndaelManaged.Padding = PaddingMode.PKCS7;
                ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor();
                byte[] array = cryptoTransform.TransformFinalBlock(bytes2, 0, bytes2.Length);
                return Convert.ToBase64String(array, 0, array.Length);
            }
            catch(Exception ex)
            {
                Debug.Log("JungleGame DataEncryption.EncryptBytes ex: " + ex.Message);
            }
            return string.Empty;
		}

		public static byte[] EncryptBytes(byte[] toEncryptArray)
		{
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(key);
                RijndaelManaged rijndaelManaged = new RijndaelManaged();
                rijndaelManaged.Key = bytes;
                rijndaelManaged.Mode = CipherMode.ECB;
                rijndaelManaged.Padding = PaddingMode.PKCS7;
                ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor();
                return cryptoTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            }
            catch(Exception ex)
            {
                Debug.Log("JungleGame DataEncryption.EncryptBytes ex: " + ex.Message);
            }
            return null;
		}

		public static string DecryptString(string toDecrypt)
		{
			if (string.IsNullOrEmpty(toDecrypt))
			{
				return string.Empty;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(key);
			if (toDecrypt.Length % 4 != 0)
			{
				return null;
			}
			byte[] array = Convert.FromBase64String(toDecrypt);
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Key = bytes;
			rijndaelManaged.Mode = CipherMode.ECB;
			rijndaelManaged.Padding = PaddingMode.PKCS7;
			ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor();
			try
			{
				byte[] bytes2 = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
				return Encoding.UTF8.GetString(bytes2);
			}
			catch (Exception ex)
			{
                Debug.Log("JungleGame DataEncryption.DecryptString ex: " + ex.Message);
            }
            return null;
		}

		public static byte[] DecryptBytes(byte[] toEncryptArray)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(key);
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Key = bytes;
			rijndaelManaged.Mode = CipherMode.ECB;
			rijndaelManaged.Padding = PaddingMode.PKCS7;
			ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor();
			try
			{
				return cryptoTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
			}
			catch (Exception ex)
			{
                Debug.Log("JungleGame DataEncryption.DecryptBytes ex: " + ex.Message);
            }
            return null;
		}
	}
}
