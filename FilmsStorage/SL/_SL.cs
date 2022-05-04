using FilmsStorage.Models.Entities;
using FilmsStorage.Models.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Configuration;
using System;
using System.Web.Security;
using System.Web;
using System.Configuration;
using System.IO;

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

                //It makes sense to check hashes with the same lenght
                if (sourceHash.Length == destinationHash.Length)
                {
                    areHashedEqual = true;

                    for(int i = 0; i < sourceHash.Length; i++)
                    {
                        if(sourceHash[i] != destinationHash[i])
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
                UserSerializatonModel userSerializatonModel = new UserSerializatonModel() { UserID = loggedInUser.UserID };

                JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();

                string userDataJson = jsonSerializer.Serialize(userSerializatonModel);

                int cookieTimeoutMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["LoginTimeout"]);

                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                    version: 1,
                    name: loggedInUser.Login,
                    issueDate: DateTime.Now,
                    expiration: DateTime.Now.AddMinutes(cookieTimeoutMinutes),
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

        public static class Files
        {
            private static FileSaveResult GetServerFilePath(string serverSaveFolder, string fileName, int id)
            {
                FileSaveResult fileUploadResult = new FileSaveResult();
             
                try
                {   
                    string fileSaveFolder = ConfigurationManager.AppSettings[serverSaveFolder];
                    string webSiteFolder = HttpContext.Current.Server.MapPath("~");

                    string fileSaveRootFolder = Path.Combine(webSiteFolder, fileSaveFolder);
                    string userSaveFolder = Path.Combine(fileSaveRootFolder, id.ToString());
                    string fileSaveName = fileName;

                    if (!Directory.Exists(fileSaveRootFolder))
                    {
                        Directory.CreateDirectory(fileSaveRootFolder);
                    }

                    if (!Directory.Exists(userSaveFolder))
                    {
                        Directory.CreateDirectory(userSaveFolder);
                    }

                    if (File.Exists(Path.Combine(userSaveFolder, fileSaveName)))
                    {
                        //rename strategy: OriginalFileName_Ticks.Ext
                        fileSaveName = Path.GetFileNameWithoutExtension(fileSaveName)
                            + "_"
                            + DateTime.Now.Ticks
                            + Path.GetExtension(fileSaveName);
                    }

                    string fileSaveFullPath = Path.Combine(userSaveFolder, fileSaveName);


                    fileUploadResult.isSaved = true;
                    fileUploadResult.FileName = fileSaveName;
                    fileUploadResult.FilePath = userSaveFolder;
                    fileUploadResult.fullPath = fileSaveFullPath;
                }
                catch (Exception ex)
                {
                    throw ex; 
                }
                
                return fileUploadResult;
            }


                public static FileSaveResult SaveFilm(HttpPostedFileBase postedFile, int uploadedByUser)
                {
                    FileSaveResult fileUploadResult = new FileSaveResult();

                    try
                    {
                        fileUploadResult = Files.GetServerFilePath("FileUploadFolder", postedFile.FileName, uploadedByUser); 
                        postedFile.SaveAs(fileUploadResult.fullPath);
                    
                    }
                    catch (Exception ex)
                    {
                        fileUploadResult.Error = ex;
                    }

                return fileUploadResult;
            }

            public static bool RemoveFIle(FileSaveResult fileUploadResult)
            {
                FileInfo file = new FileInfo(fileUploadResult.fullPath); 
                
                if (file.Exists) 
                {
                    file.Delete(); 
                }
                else
                {
                    fileUploadResult.Error = new Exception("File doesn't exist");
                    return false;
                }

                return true; 
            }
            
            

        }
    }
}