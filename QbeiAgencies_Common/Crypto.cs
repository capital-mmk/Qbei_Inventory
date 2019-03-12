using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace QbeiAgencies_Common
{
    /// &lt;summary>
    /// basic Encrption/decryption functionaility
    /// &lt;/summary>
    public class Crypto
    {
        #region enums, constants & fields
        //types of symmetric encyption
        public enum CryptoTypes
        {
            encTypeDES = 0,
            encTypeRC2,
            encTypeRijndael,
            encTypeTripleDES
        }

        private const string CRYPT_DEFAULT_PASSWORD = "CB06cfE507a1";
        private const CryptoTypes CRYPT_DEFAULT_METHOD = CryptoTypes.encTypeRijndael;

        private byte[] mKey = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 };
        private byte[] mIV = { 65, 110, 68, 26, 69, 178, 200, 219 };
        private byte[] SaltByteArray = { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 };
        private CryptoTypes mCryptoType = CRYPT_DEFAULT_METHOD;
        private string mPassword = CRYPT_DEFAULT_PASSWORD;
        #endregion

        #region Constructors

        public Crypto()
        {
            calculateNewKeyAndIV();
        }

        public Crypto(CryptoTypes CryptoType)
        {
            this.CryptoType = CryptoType;
        }
        #endregion

        #region Props

        /// &lt;summary>
        ///     type of encryption / decryption used
        /// &lt;/summary>
        public CryptoTypes CryptoType
        {
            get
            {
                return mCryptoType;
            }
            set
            {
                if (mCryptoType != value)
                {
                    mCryptoType = value;
                    calculateNewKeyAndIV();
                }
            }
        }

        /// &lt;summary>
        ///     Passsword Key Property.
        ///     The password key used when encrypting / decrypting
        /// &lt;/summary>
        public string Password
        {
            get
            {
                return mPassword;
            }
            set
            {
                if (mPassword != value)
                {
                    mPassword = value;
                    calculateNewKeyAndIV();
                }
            }
        }
        #endregion

        #region Encryption

        /// &lt;summary>
        ///     Encrypt a string
        /// &lt;/summary>
        /// &lt;param name="inputText">text to encrypt&lt;/param>
        /// &lt;returns>an encrypted string&lt;/returns>
        public string Encrypt(string inputText)
        {
            //declare a new encoder
            UTF8Encoding UTF8Encoder = new UTF8Encoding();
            //get byte representation of string
            byte[] inputBytes = UTF8Encoder.GetBytes(inputText);

            //convert back to a string
            return Convert.ToBase64String(EncryptDecrypt(inputBytes, true));
        }

        /// &lt;summary>
        ///     Encrypt string with user defined password
        /// &lt;/summary>
        /// &lt;param name="inputText">text to encrypt&lt;/param>
        /// &lt;param name="password">password to use when encrypting&lt;/param>
        /// &lt;returns>an encrypted string&lt;/returns>
        public string Encrypt(string inputText, string password)
        {
            this.Password = password;
            return this.Encrypt(inputText);
        }

        /// &lt;summary>
        ///     Encrypt string acc. to cryptoType and with user defined password
        /// &lt;/summary>
        /// &lt;param name="inputText">text to encrypt&lt;/param>
        /// &lt;param name="password">password to use when encrypting&lt;/param>
        /// &lt;param name="cryptoType">type of encryption&lt;/param>
        /// &lt;returns>an encrypted string&lt;/returns>
        public string Encrypt(string inputText, string password, CryptoTypes cryptoType)
        {
            mCryptoType = cryptoType;
            return this.Encrypt(inputText, password);
        }

        /// &lt;summary>
        ///     Encrypt string acc. to cryptoType
        /// &lt;/summary>
        /// &lt;param name="inputText">text to encrypt&lt;/param>
        /// &lt;param name="cryptoType">type of encryption&lt;/param>
        /// &lt;returns>an encrypted string&lt;/returns>
        public string Encrypt(string inputText, CryptoTypes cryptoType)
        {
            this.CryptoType = cryptoType;
            return this.Encrypt(inputText);
        }

        #endregion

        #region Decryption

        /// &lt;summary>
        ///     decrypts a string
        /// &lt;/summary>
        /// &lt;param name="inputText">string to decrypt&lt;/param>
        /// &lt;returns>a decrypted string&lt;/returns>
        public string Decrypt(string inputText)
        {
            //declare a new encoder
            UTF8Encoding UTF8Encoder = new UTF8Encoding();
            //get byte representation of string
            byte[] inputBytes = Convert.FromBase64String(inputText);

            //convert back to a string
            return UTF8Encoder.GetString(EncryptDecrypt(inputBytes, false));
        }

        /// &lt;summary>
        ///     decrypts a string using a user defined password key
        /// &lt;/summary>
        /// &lt;param name="inputText">string to decrypt&lt;/param>
        /// &lt;param name="password">password to use when decrypting&lt;/param>
        /// &lt;returns>a decrypted string&lt;/returns>
        public string Decrypt(string inputText, string password)
        {
            this.Password = password;
            return Decrypt(inputText);
        }

        /// &lt;summary>
        ///     decrypts a string acc. to decryption type and user defined password key
        /// &lt;/summary>
        /// &lt;param name="inputText">string to decrypt&lt;/param>
        /// &lt;param name="password">password key used to decrypt&lt;/param>
        /// &lt;param name="cryptoType">type of decryption&lt;/param>
        /// &lt;returns>a decrypted string&lt;/returns>
        public string Decrypt(string inputText, string password, CryptoTypes cryptoType)
        {
            mCryptoType = cryptoType;
            return Decrypt(inputText, password);
        }

        /// &lt;summary>
        ///     decrypts a string acc. to the decryption type
        /// &lt;/summary>
        /// &lt;param name="inputText">string to decrypt&lt;/param>
        /// &lt;param name="cryptoType">type of decryption&lt;/param>
        /// &lt;returns>a decrypted string&lt;/returns>
        public string Decrypt(string inputText, CryptoTypes cryptoType)
        {
            this.CryptoType = cryptoType;
            return Decrypt(inputText);
        }
        #endregion

        #region Symmetric Engine

        /// &lt;summary>
        ///     performs the actual enc/dec.
        /// &lt;/summary>
        /// &lt;param name="inputBytes">input byte array&lt;/param>
        /// &lt;param name="Encrpyt">wheather or not to perform enc/dec&lt;/param>
        /// &lt;returns>byte array output&lt;/returns>
        private byte[] EncryptDecrypt(byte[] inputBytes, bool Encrpyt)
        {
            //get the correct transform
            ICryptoTransform transform = getCryptoTransform(Encrpyt);

            //memory stream for output
            MemoryStream memStream = new MemoryStream();

            try
            {
                //setup the cryption - output written to memstream
                CryptoStream cryptStream = new CryptoStream(memStream, transform, CryptoStreamMode.Write);

                //write data to cryption engine
                cryptStream.Write(inputBytes, 0, inputBytes.Length);

                //we are finished
                cryptStream.FlushFinalBlock();

                //get result
                byte[] output = memStream.ToArray();

                //finished with engine, so close the stream
                cryptStream.Close();

                return output;
            }
            catch (Exception e)
            {
                //throw an error
                throw new Exception("Error in symmetric engine. Error : " + e.Message, e);
            }
        }

        /// &lt;summary>
        ///     returns the symmetric engine and creates the encyptor/decryptor
        /// &lt;/summary>
        /// &lt;param name="encrypt">whether to return a encrpytor or decryptor&lt;/param>
        /// &lt;returns>ICryptoTransform&lt;/returns>
        private ICryptoTransform getCryptoTransform(bool encrypt)
        {
            SymmetricAlgorithm SA = selectAlgorithm();
            SA.Key = mKey;
            SA.IV = mIV;
            if (encrypt)
            {
                return SA.CreateEncryptor();
            }
            else
            {
                return SA.CreateDecryptor();
            }
        }
        /// &lt;summary>
        ///     returns the specific symmetric algorithm acc. to the cryptotype
        /// &lt;/summary>
        /// &lt;returns>SymmetricAlgorithm&lt;/returns>
        private SymmetricAlgorithm selectAlgorithm()
        {
            SymmetricAlgorithm SA;
            switch (mCryptoType)
            {
                case CryptoTypes.encTypeDES:
                    SA = DES.Create();
                    break;
                case CryptoTypes.encTypeRC2:
                    SA = RC2.Create();
                    break;
                case CryptoTypes.encTypeRijndael:
                    SA = Rijndael.Create();
                    break;
                case CryptoTypes.encTypeTripleDES:
                    SA = TripleDES.Create();
                    break;
                default:
                    SA = TripleDES.Create();
                    break;
            }
            return SA;
        }

        /// &lt;summary>
        ///     calculates the key and IV acc. to the symmetric method from the password
        ///     key and IV size dependant on symmetric method
        /// &lt;/summary>
        private void calculateNewKeyAndIV()
        {
            //use salt so that key cannot be found with dictionary attack
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(mPassword, SaltByteArray);
            SymmetricAlgorithm algo = selectAlgorithm();
            mKey = pdb.GetBytes(algo.KeySize / 8);
            mIV = pdb.GetBytes(algo.BlockSize / 8);
        }

        #endregion
    }

    /// &lt;summary>
    /// Hashing class. Only static members so no need to create an instance
    /// &lt;/summary>
    public class Hashing
    {
        #region enum, constants and fields
        //types of hashing available
        public enum HashingTypes
        {
            SHA, SHA256, SHA384, SHA512, MD5
        }
        #endregion

        #region static members
        public static string Hash(String inputText)
        {
            return ComputeHash(inputText, HashingTypes.MD5);
        }

        public static string Hash(String inputText, HashingTypes hashingType)
        {
            return ComputeHash(inputText, hashingType);
        }

        /// &lt;summary>
        ///     returns true if the input text is equal to hashed text
        /// &lt;/summary>
        /// &lt;param name="inputText">unhashed text to test&lt;/param>
        /// &lt;param name="hashText">already hashed text&lt;/param>
        /// &lt;returns>boolean true or false&lt;/returns>
        public static bool isHashEqual(string inputText, string hashText)
        {
            return (Hash(inputText) == hashText);
        }

        public static bool isHashEqual(string inputText, string hashText, HashingTypes hashingType)
        {
            return (Hash(inputText, hashingType) == hashText);
        }
        #endregion

        #region Hashing Engine

        /// &lt;summary>
        ///     computes the hash code and converts it to string
        /// &lt;/summary>
        /// &lt;param name="inputText">input text to be hashed&lt;/param>
        /// &lt;param name="hashingType">type of hashing to use&lt;/param>
        /// &lt;returns>hashed string&lt;/returns>
        private static string ComputeHash(string inputText, HashingTypes hashingType)
        {
            HashAlgorithm HA = getHashAlgorithm(hashingType);

            //declare a new encoder
            UTF8Encoding UTF8Encoder = new UTF8Encoding();
            //get byte representation of input text
            byte[] inputBytes = UTF8Encoder.GetBytes(inputText);


            //hash the input byte array
            byte[] output = HA.ComputeHash(inputBytes);

            //convert output byte array to a string
            return Convert.ToBase64String(output);
        }

        /// &lt;summary>
        ///     returns the specific hashing alorithm
        /// &lt;/summary>
        /// &lt;param name="hashingType">type of hashing to use&lt;/param>
        /// &lt;returns>HashAlgorithm&lt;/returns>
        private static HashAlgorithm getHashAlgorithm(HashingTypes hashingType)
        {
            switch (hashingType)
            {
                case HashingTypes.MD5:
                    return new MD5CryptoServiceProvider();
                case HashingTypes.SHA:
                    return new SHA1CryptoServiceProvider();
                case HashingTypes.SHA256:
                    return new SHA256Managed();
                case HashingTypes.SHA384:
                    return new SHA384Managed();
                case HashingTypes.SHA512:
                    return new SHA512Managed();
                default:
                    return new MD5CryptoServiceProvider();
            }
        }
        #endregion

    }

    //public class testCrypt
    //{
    //    public void testEncryption()
    //    {
    //        string input = "Thi$ is @ str!&n to tEst encrypti0n!";
    //        Crypto c = new Crypto(Utils.Crypto.CryptoTypes.encTypeTripleDES);
    //        string s1 = c.Encrypt(input);
    //        string s2 = c.Decrypt(s1);
    //        Assert.IsTrue(s2 == input);

    //        s1 = Hashing.Hash(input);
    //        s2 = Hashing.Hash(input,Utils.Hashing.HashingTypes.MD5);
    //        Assert.IsTrue(s1 == s2);
    //        Assert.IsTrue( Hashing.isHashEqual(input,s1));

    //        s1 = Hashing.Hash(input,Utils.Hashing.HashingTypes.SHA512);
    //    }
    //}
}