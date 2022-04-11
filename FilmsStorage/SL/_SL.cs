using System.Configuration;

using System.Security.Cryptography;
using System.Text;
using FilmsStorage.Models.Entities;
using FilmsStorage.Models.Serialization;

using System.Web.Security;
using System.Web.Script.Serialization;
using System;
using System.Web;

namespace FilmsStorage.SL
{
    public static class _SL
    {
        public static class Hasher
        {
            public static byte[] createHash(string strToHash)
            {
                SHA512 hasher = SHA512.Create();

                byte[] hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(strToHash));

                return hash;
            }

            public static bool checkPassword(byte[] sourcePasswordHash, string testPasswordString)
            {
                byte[] testPasswordStringHash = createHash(testPasswordString);

                return compareTwoHashes(sourcePasswordHash, testPasswordStringHash);
            }

            public static bool compareTwoHashes(byte[] sourceHash, byte[] destinationHash)
            {
                bool areHashedEqual = false;

                //It makes sense to check hashes with the same length
                if (sourceHash.Length == destinationHash.Length)
                {
                    areHashedEqual = true;

                    for (int i = 0; i < sourceHash.Length; i++)
                    {
                        if (sourceHash[i] != destinationHash[i]) 
                        {
                            areHashedEqual = false;
                            break;
                        }
                    }
                }

                return areHashedEqual;
            }
        }

        public static class Cookies
        {
            public static void SetLoginCookie(User loggedInUser)
            {
                UserSerializationModel userSerializationModel = new UserSerializationModel() { UserID = loggedInUser.UserID };

                JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();

                string userDataJson = jsonSerializer.Serialize(userSerializationModel); 

                int cookieTimeOutMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["LoginTimeout"]);

                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                    version: 1,
                    name: loggedInUser.Login,
                    issueDate: DateTime.Now, 
                    expiration: DateTime.Now.AddMinutes(cookieTimeOutMinutes),
                    isPersistent: true, 
                    userData: userDataJson
                    );

                string encryptedAuthTicket = FormsAuthentication.Encrypt(authTicket);

                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedAuthTicket);

                //Cookie lifetime should be no longer than auth ticket lifetime
                authCookie.Expires = authTicket.Expiration;

                HttpContext.Current.Response.Cookies.Add(authCookie); 
            }
        }

    }
}