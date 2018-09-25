using System;
using System.Security.Cryptography;
using System.Text;

namespace Newcats.JobManager.Api.Infrastructure.Text.Encrypt
{
    /// <summary>
    /// 加密操作
    /// 说明：
    /// 1. AES加密整理自支付宝SDK
    /// 2. RSA加密采用 https://github.com/stulzq/DotnetCore.RSA/blob/master/DotnetCore.RSA/RSAHelper.cs
    /// </summary>
    public static class Encrypt
    {
        #region RandomString/RandomNumber/RandomKey(Salt)
        /// <summary>
        /// 获取指定长度的随机数字/字母组成的字符串
        /// </summary>
        /// <param name="length">字符串长度</param>
        public static string GetRandomString(int length)
        {
            char[] arrChar = new char[]{
           'a','b','d','c','e','f','g','h','i','j','k','l','m','n','p','r','q','s','t','u','v','w','z','y','x',
           '0','1','2','3','4','5','6','7','8','9',
           'A','B','C','D','E','F','G','H','I','J','K','L','M','N','Q','P','R','T','S','V','U','W','X','Y','Z'
          };

            StringBuilder num = new StringBuilder();

            Random rnd = new Random();
            for (int i = 0; i < length; i++)
            {
                num.Append(arrChar[rnd.Next(0, arrChar.Length)].ToString());
            }
            return num.ToString();
        }

        /// <summary>
        /// 获取指定长度的随机数字组成的字符串
        /// </summary>
        /// <param name="length">字符串长度</param>
        public static string GetRandomNumber(int length)
        {
            char[] arrChar = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            StringBuilder num = new StringBuilder();

            Random rnd = new Random();
            for (int i = 0; i < length; i++)
            {
                num.Append(arrChar[rnd.Next(0, arrChar.Length)].ToString());
            }
            return num.ToString();
        }

        /// <summary>
        /// 获取指定长度的随机数字/字母/特殊字符组成的字符串(Salt)
        /// </summary>
        /// <param name="length">字符串长度</param>
        public static string GetRandomKey(int length)
        {
            char[] arrChar = new char[]{
           'a','b','d','c','e','f','g','h','i','j','k','l','m','n','p','r','q','s','t','u','v','w','z','y','x',
           '0','1','2','3','4','5','6','7','8','9',
           'A','B','C','D','E','F','G','H','I','J','K','L','M','N','Q','P','R','T','S','V','U','W','X','Y','Z',
           '!','@','#','$','%','^','&','*','(',')','_','+','=','|','.',',','/'
          };

            StringBuilder num = new StringBuilder();

            Random rnd = new Random();
            for (int i = 0; i < length; i++)
            {
                num.Append(arrChar[rnd.Next(0, arrChar.Length)].ToString());
            }
            return num.ToString();
        }
        #endregion

        #region Md5加密

        /// <summary>
        /// Md5加密，返回16位结果
        /// </summary>
        /// <param name="value">值</param>
        public static string MD5By16(string value)
        {
            return MD5By16(value, Encoding.UTF8);
        }

        /// <summary>
        /// Md5加密，返回16位结果
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="encoding">字符编码</param>
        public static string MD5By16(string value, Encoding encoding)
        {
            return Md5(value, encoding, 4, 8);
        }

        /// <summary>
        /// Md5加密
        /// </summary>
        private static string Md5(string value, Encoding encoding, int? startIndex, int? length)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;
            var md5 = new MD5CryptoServiceProvider();
            string result;
            try
            {
                var hash = md5.ComputeHash(encoding.GetBytes(value));
                //result = startIndex == null ? BitConverter.ToString(hash) : BitConverter.ToString(hash, startIndex.SafeValue(), length.SafeValue());
                result = startIndex == null ? BitConverter.ToString(hash) : BitConverter.ToString(hash, startIndex ?? default(int), length ?? default(int));
            }
            finally
            {
                md5.Clear();
            }
            return result.Replace("-", "");
        }

        /// <summary>
        /// Md5加密，返回32位结果
        /// </summary>
        /// <param name="value">值</param>
        public static string MD5By32(string value)
        {
            return MD5By32(value, Encoding.UTF8);
        }

        /// <summary>
        /// Md5加密，返回32位结果
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="encoding">字符编码</param>
        public static string MD5By32(string value, Encoding encoding)
        {
            return Md5(value, encoding, null, null);
        }

        #endregion

        #region HMACMD5
        /// <summary>
        /// HMACMD5 hash
        /// </summary>
        /// <param name="str">The string to be encrypted</param>
        /// <param name="key">encrypte key</param>
        /// <returns></returns>
        public static string HMACMD5(string str, string key)
        {
            byte[] secrectKey = Encoding.UTF8.GetBytes(key);
            using (HMACMD5 md5 = new HMACMD5(secrectKey))
            {
                byte[] bytes_md5_in = Encoding.UTF8.GetBytes(str);
                byte[] bytes_md5_out = md5.ComputeHash(bytes_md5_in);
                string str_md5_out = BitConverter.ToString(bytes_md5_out);
                str_md5_out = str_md5_out.Replace("-", "");
                return str_md5_out;
            }
        }

        /// <summary>
        /// HMACMD5 hash
        /// </summary>
        /// <param name="str">The string to be encrypted</param>
        /// <param name="key">encrypte key</param>
        /// <param name="encoding">字符编码</param>
        /// <returns></returns>
        public static string HMACMD5(string str, string key, Encoding encoding)
        {
            byte[] secrectKey = encoding.GetBytes(key);
            using (HMACMD5 md5 = new HMACMD5(secrectKey))
            {
                byte[] bytes_md5_in = encoding.GetBytes(str);
                byte[] bytes_md5_out = md5.ComputeHash(bytes_md5_in);
                string str_md5_out = BitConverter.ToString(bytes_md5_out);
                str_md5_out = str_md5_out.Replace("-", "");
                return str_md5_out;
            }
        }
        #endregion

        #region SHA1
        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="str">The string to be encrypted</param>
        /// <returns></returns>
        public static string Sha1(string str)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] bytes_sha1_in = Encoding.UTF8.GetBytes(str);
                byte[] bytes_sha1_out = sha1.ComputeHash(bytes_sha1_in);
                string str_sha1_out = BitConverter.ToString(bytes_sha1_out);
                str_sha1_out = str_sha1_out.Replace("-", "");
                return str_sha1_out;
            }
        }

        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="str">The string to be encrypted</param>
        /// <param name="encoding">字符编码</param>
        /// <returns></returns>
        public static string Sha1(string str, Encoding encoding)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] bytes_sha1_in = encoding.GetBytes(str);
                byte[] bytes_sha1_out = sha1.ComputeHash(bytes_sha1_in);
                string str_sha1_out = BitConverter.ToString(bytes_sha1_out);
                str_sha1_out = str_sha1_out.Replace("-", "");
                return str_sha1_out;
            }
        }
        #endregion

        #region SHA256
        /// <summary>
        /// SHA256加密
        /// </summary>
        /// <param name="str">The string to be encrypted</param>
        /// <returns></returns>
        public static string Sha256(string str)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes_sha256_in = Encoding.UTF8.GetBytes(str);
                byte[] bytes_sha256_out = sha256.ComputeHash(bytes_sha256_in);
                string str_sha256_out = BitConverter.ToString(bytes_sha256_out);
                str_sha256_out = str_sha256_out.Replace("-", "");
                return str_sha256_out;
            }
        }

        /// <summary>
        /// SHA256加密
        /// </summary>
        /// <param name="str">The string to be encrypted</param>
        /// <param name="encoding">字符编码</param>
        /// <returns></returns>
        public static string Sha256(string str, Encoding encoding)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes_sha256_in = encoding.GetBytes(str);
                byte[] bytes_sha256_out = sha256.ComputeHash(bytes_sha256_in);
                string str_sha256_out = BitConverter.ToString(bytes_sha256_out);
                str_sha256_out = str_sha256_out.Replace("-", "");
                return str_sha256_out;
            }
        }
        #endregion

        #region SHA384
        /// <summary>
        /// SHA384加密
        /// </summary>
        /// <param name="str">The string to be encrypted</param>
        /// <returns></returns>
        public static string Sha384(string str)
        {
            using (SHA384 sha384 = SHA384.Create())
            {
                byte[] bytes_sha384_in = Encoding.UTF8.GetBytes(str);
                byte[] bytes_sha384_out = sha384.ComputeHash(bytes_sha384_in);
                string str_sha384_out = BitConverter.ToString(bytes_sha384_out);
                str_sha384_out = str_sha384_out.Replace("-", "");
                return str_sha384_out;
            }
        }

        /// <summary>
        /// SHA384加密
        /// </summary>
        /// <param name="str">The string to be encrypted</param>
        /// <param name="encoding">字符编码</param>
        /// <returns></returns>
        public static string Sha384(string str, Encoding encoding)
        {
            using (SHA384 sha384 = SHA384.Create())
            {
                byte[] bytes_sha384_in = encoding.GetBytes(str);
                byte[] bytes_sha384_out = sha384.ComputeHash(bytes_sha384_in);
                string str_sha384_out = BitConverter.ToString(bytes_sha384_out);
                str_sha384_out = str_sha384_out.Replace("-", "");
                return str_sha384_out;
            }
        }
        #endregion

        #region SHA512
        /// <summary>
        /// SHA512加密
        /// </summary>
        /// <param name="str">The string to be encrypted</param>
        /// <returns></returns>
        public static string Sha512(string str)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] bytes_sha512_in = Encoding.UTF8.GetBytes(str);
                byte[] bytes_sha512_out = sha512.ComputeHash(bytes_sha512_in);
                string str_sha512_out = BitConverter.ToString(bytes_sha512_out);
                str_sha512_out = str_sha512_out.Replace("-", "");
                return str_sha512_out;
            }
        }

        /// <summary>
        /// SHA512加密
        /// </summary>
        /// <param name="str">The string to be encrypted</param>
        /// <param name="encoding">字符编码</param>
        /// <returns></returns>
        public static string Sha512(string str, Encoding encoding)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] bytes_sha512_in = encoding.GetBytes(str);
                byte[] bytes_sha512_out = sha512.ComputeHash(bytes_sha512_in);
                string str_sha512_out = BitConverter.ToString(bytes_sha512_out);
                str_sha512_out = str_sha512_out.Replace("-", "");
                return str_sha512_out;
            }
        }
        #endregion

        #region HMACSHA1
        /// <summary>
        /// HMAC_SHA1
        /// </summary>
        /// <param name="str">The string to be encrypted</param>
        /// <param name="key">encrypte key</param>
        /// <returns></returns>
        public static string HMACSHA1(string str, string key)
        {
            byte[] secrectKey = Encoding.UTF8.GetBytes(key);
            using (HMACSHA1 hmac = new HMACSHA1(secrectKey))
            {
                hmac.Initialize();

                byte[] bytes_hmac_in = Encoding.UTF8.GetBytes(str);
                byte[] bytes_hamc_out = hmac.ComputeHash(bytes_hmac_in);

                string str_hamc_out = BitConverter.ToString(bytes_hamc_out);
                str_hamc_out = str_hamc_out.Replace("-", "");

                return str_hamc_out;
            }
        }

        /// <summary>
        /// HMAC_SHA1
        /// </summary>
        /// <param name="str">The string to be encrypted</param>
        /// <param name="key">encrypte key</param>
        /// <param name="encoding">字符编码</param>
        /// <returns></returns>
        public static string HMACSHA1(string str, string key, Encoding encoding)
        {
            byte[] secrectKey = encoding.GetBytes(key);
            using (HMACSHA1 hmac = new HMACSHA1(secrectKey))
            {
                hmac.Initialize();

                byte[] bytes_hmac_in = encoding.GetBytes(str);
                byte[] bytes_hamc_out = hmac.ComputeHash(bytes_hmac_in);

                string str_hamc_out = BitConverter.ToString(bytes_hamc_out);
                str_hamc_out = str_hamc_out.Replace("-", "");

                return str_hamc_out;
            }
        }
        #endregion

        #region HMACSHA256
        /// <summary>
        /// HMAC_SHA256 
        /// </summary>
        /// <param name="str">The string to be encrypted</param>
        /// <param name="key">encrypte key</param>
        /// <returns></returns>
        public static string HMACSHA256(string str, string key)
        {
            byte[] secrectKey = Encoding.UTF8.GetBytes(key);
            using (HMACSHA256 hmac = new HMACSHA256(secrectKey))
            {
                hmac.Initialize();

                byte[] bytes_hmac_in = Encoding.UTF8.GetBytes(str);
                byte[] bytes_hamc_out = hmac.ComputeHash(bytes_hmac_in);

                string str_hamc_out = BitConverter.ToString(bytes_hamc_out);
                str_hamc_out = str_hamc_out.Replace("-", "");

                return str_hamc_out;
            }
        }

        /// <summary>
        /// HMAC_SHA256 
        /// </summary>
        /// <param name="str">The string to be encrypted</param>
        /// <param name="key">encrypte key</param>
        /// <param name="encoding">字符编码</param>
        /// <returns></returns>
        public static string HMACSHA256(string str, string key, Encoding encoding)
        {
            byte[] secrectKey = encoding.GetBytes(key);
            using (HMACSHA256 hmac = new HMACSHA256(secrectKey))
            {
                hmac.Initialize();

                byte[] bytes_hmac_in = encoding.GetBytes(str);
                byte[] bytes_hamc_out = hmac.ComputeHash(bytes_hmac_in);

                string str_hamc_out = BitConverter.ToString(bytes_hamc_out);
                str_hamc_out = str_hamc_out.Replace("-", "");

                return str_hamc_out;
            }
        }
        #endregion

        #region HMACSHA384
        /// <summary>
        /// HMAC_SHA384
        /// </summary>
        /// <param name="str">The string to be encrypted</param>
        /// <param name="key">encrypte key</param>
        /// <returns></returns>
        public static string HMACSHA384(string str, string key)
        {
            byte[] secrectKey = Encoding.UTF8.GetBytes(key);
            using (HMACSHA384 hmac = new HMACSHA384(secrectKey))
            {
                hmac.Initialize();

                byte[] bytes_hmac_in = Encoding.UTF8.GetBytes(str);
                byte[] bytes_hamc_out = hmac.ComputeHash(bytes_hmac_in);


                string str_hamc_out = BitConverter.ToString(bytes_hamc_out);
                str_hamc_out = str_hamc_out.Replace("-", "");

                return str_hamc_out;
            }
        }

        /// <summary>
        /// HMAC_SHA384
        /// </summary>
        /// <param name="str">The string to be encrypted</param>
        /// <param name="key">encrypte key</param>
        /// <param name="encoding">字符编码</param>
        /// <returns></returns>
        public static string HMACSHA384(string str, string key, Encoding encoding)
        {
            byte[] secrectKey = encoding.GetBytes(key);
            using (HMACSHA384 hmac = new HMACSHA384(secrectKey))
            {
                hmac.Initialize();

                byte[] bytes_hmac_in = encoding.GetBytes(str);
                byte[] bytes_hamc_out = hmac.ComputeHash(bytes_hmac_in);


                string str_hamc_out = BitConverter.ToString(bytes_hamc_out);
                str_hamc_out = str_hamc_out.Replace("-", "");

                return str_hamc_out;
            }
        }
        #endregion

        #region HMACSHA512
        /// <summary>
        /// HMAC_SHA512
        /// </summary>
        /// <param name="str">The string to be encrypted</param>
        /// <param name="key">encrypte key</param>
        /// <returns></returns>
        public static string HMACSHA512(string str, string key)
        {
            byte[] secrectKey = Encoding.UTF8.GetBytes(key);
            using (HMACSHA512 hmac = new HMACSHA512(secrectKey))
            {
                hmac.Initialize();

                byte[] bytes_hmac_in = Encoding.UTF8.GetBytes(str);
                byte[] bytes_hamc_out = hmac.ComputeHash(bytes_hmac_in);

                string str_hamc_out = BitConverter.ToString(bytes_hamc_out);
                str_hamc_out = str_hamc_out.Replace("-", "");

                return str_hamc_out;
            }
        }

        /// <summary>
        /// HMAC_SHA512
        /// </summary>
        /// <param name="str">The string to be encrypted</param>
        /// <param name="key">encrypte key</param>
        /// <param name="encoding">字符编码</param>
        /// <returns></returns>
        public static string HMACSHA512(string str, string key, Encoding encoding)
        {
            byte[] secrectKey = encoding.GetBytes(key);
            using (HMACSHA512 hmac = new HMACSHA512(secrectKey))
            {
                hmac.Initialize();

                byte[] bytes_hmac_in = encoding.GetBytes(str);
                byte[] bytes_hamc_out = hmac.ComputeHash(bytes_hmac_in);

                string str_hamc_out = BitConverter.ToString(bytes_hamc_out);
                str_hamc_out = str_hamc_out.Replace("-", "");

                return str_hamc_out;
            }
        }
        #endregion

        #region DES加密

        /// <summary>
        /// DES密钥,24位字符串
        /// </summary>
        public static string DESKey = "AU5f6ImsFb,3@6z57j%Y_g7&";

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        public static string DESEncrypt(object value)
        {
            return DESEncrypt(value, DESKey);
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥,24位</param>
        public static string DESEncrypt(object value, string key)
        {
            return DESEncrypt(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥,24位</param>
        /// <param name="encoding">编码</param>
        public static string DESEncrypt(object value, string key, Encoding encoding)
        {
            //string text = value.SafeString();
            string text = value == null ? string.Empty : value.ToString().Trim();
            if (ValidateDes(text, key) == false)
                return string.Empty;
            using (var transform = CreateDesProvider(key).CreateEncryptor())
            {
                return GetEncryptResult(text, encoding, transform);
            }
        }

        /// <summary>
        /// 验证Des加密参数
        /// </summary>
        private static bool ValidateDes(string text, string key)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(key))
                return false;
            return key.Length == 24;
        }

        /// <summary>
        /// 创建Des加密服务提供程序
        /// </summary>
        private static TripleDESCryptoServiceProvider CreateDesProvider(string key)
        {
            return new TripleDESCryptoServiceProvider { Key = Encoding.ASCII.GetBytes(key), Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
        }

        /// <summary>
        /// 获取加密结果
        /// </summary>
        private static string GetEncryptResult(string value, Encoding encoding, ICryptoTransform transform)
        {
            var bytes = encoding.GetBytes(value);
            var result = transform.TransformFinalBlock(bytes, 0, bytes.Length);
            return System.Convert.ToBase64String(result);
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        public static string DESDecrypt(object value)
        {
            return DESDecrypt(value, DESKey);
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        /// <param name="key">密钥,24位</param>
        public static string DESDecrypt(object value, string key)
        {
            return DESDecrypt(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        /// <param name="key">密钥,24位</param>
        /// <param name="encoding">编码</param>
        public static string DESDecrypt(object value, string key, Encoding encoding)
        {
            string text = value == null ? string.Empty : value.ToString().Trim();
            if (!ValidateDes(text, key))
                return string.Empty;
            using (var transform = CreateDesProvider(key).CreateDecryptor())
            {
                return GetDecryptResult(text, encoding, transform);
            }
        }

        /// <summary>
        /// 获取解密结果
        /// </summary>
        private static string GetDecryptResult(string value, Encoding encoding, ICryptoTransform transform)
        {
            var bytes = System.Convert.FromBase64String(value);
            var result = transform.TransformFinalBlock(bytes, 0, bytes.Length);
            return encoding.GetString(result);
        }

        #endregion

        #region AES加密

        /// <summary>
        /// 128位0向量
        /// </summary>
        private static byte[] _iv;
        /// <summary>
        /// 128位0向量
        /// </summary>
        private static byte[] Iv
        {
            get
            {
                if (_iv == null)
                {
                    var size = 16;
                    _iv = new byte[size];
                    for (int i = 0; i < size; i++)
                        _iv[i] = 0;
                }
                return _iv;
            }
        }

        /// <summary>
        /// AES密钥,32位字符串
        /// </summary>
        public static string AESKey = "T&t8C,(YaGyFSkB_fVE1,8(j0v#69At0";

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        public static string AESEncrypt(string value)
        {
            return AESEncrypt(value, AESKey);
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥,32位字符串</param>
        public static string AESEncrypt(string value, string key)
        {
            return AESEncrypt(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥,32位字符串</param>
        /// <param name="encoding">编码</param>
        public static string AESEncrypt(string value, string key, Encoding encoding)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(key))
                return string.Empty;
            var rijndaelManaged = CreateRijndaelManaged(key);
            using (var transform = rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, rijndaelManaged.IV))
            {
                return GetEncryptResult(value, encoding, transform);
            }
        }

        /// <summary>
        /// 创建RijndaelManaged
        /// </summary>
        private static RijndaelManaged CreateRijndaelManaged(string key)
        {
            return new RijndaelManaged
            {
                Key = System.Convert.FromBase64String(key),
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                IV = Iv
            };
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        public static string AESDecrypt(string value)
        {
            return AESDecrypt(value, AESKey);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        /// <param name="key">密钥,32位字符串</param>
        public static string AESDecrypt(string value, string key)
        {
            return AESDecrypt(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        /// <param name="key">密钥,32位字符串</param>
        /// <param name="encoding">编码</param>
        public static string AESDecrypt(string value, string key, Encoding encoding)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(key))
                return string.Empty;
            var rijndaelManaged = CreateRijndaelManaged(key);
            using (var transform = rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV))
            {
                return GetDecryptResult(value, encoding, transform);
            }
        }

        #endregion

        #region RSA签名

        /// <summary>
        /// RSA加密，采用 SHA1 算法
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥(公私钥请使用openssl生成  ssh-keygen -t rsa 命令生成的公钥私钥是不行的)</param>
        public static string RSASign(string value, string key)
        {
            return RSASign(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// RSA加密，采用 SHA1 算法
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥(公私钥请使用openssl生成  ssh-keygen -t rsa 命令生成的公钥私钥是不行的)</param>
        /// <param name="encoding">编码</param>
        public static string RSASign(string value, string key, Encoding encoding)
        {
            return RsaSign(value, key, encoding, RSAType.SHA1);
        }

        /// <summary>
        /// RSA加密，采用 SHA256 算法
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥(公私钥请使用openssl生成  ssh-keygen -t rsa 命令生成的公钥私钥是不行的)</param>
        public static string RSA2Sign(string value, string key)
        {
            return RSA2Sign(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// RSA加密，采用 SHA256 算法
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥(公私钥请使用openssl生成  ssh-keygen -t rsa 命令生成的公钥私钥是不行的)</param>
        /// <param name="encoding">编码</param>
        public static string RSA2Sign(string value, string key, Encoding encoding)
        {
            return RsaSign(value, key, encoding, RSAType.SHA256);
        }

        /// <summary>
        /// Rsa加密
        /// </summary>
        private static string RsaSign(string value, string key, Encoding encoding, RSAType type)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(key))
                return string.Empty;
            var rsa = new RsaHelper(type, encoding, key);
            return rsa.Sign(value);
        }
        #endregion
    }
}
