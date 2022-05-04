using FilmsStorage.DAL;
using FilmsStorage.Models;
using FilmsStorage.Models.Entities;
using FilmsStorage.SL;
using System;
using System.Web.Mvc;

namespace FilmsStorage.Controllers
{
    [Authorize]
    public class FilesController : BaseController
    {
        // GET: Files By User
        public PartialViewResult ByUser()
        {
            //TODO: Get Files Of Given User

            return PartialView();
        }

  

        //Show add file form
        public ViewResult Add()
        {
            return View(FilmAddModel.getFilmModelTemplate());
        }

        [HttpPost] 
        public ActionResult Add(FilmAddModel addFilmModel)
        {
            if (Request.Files[0].ContentLength > 0)
            {
                FileSaveResult fileInfo = _SL.Files.SaveFilm(Request.Files[0], CurrentUser.UserID);

                if (fileInfo.isSaved)
                {
                    addFilmModel.UserID = CurrentUser.UserID; 
                    addFilmModel.FilePath = fileInfo.FilePath;
                    addFilmModel.FileName = fileInfo.FileName;

                    Film addedFilm = _DAL.Films.Add(addFilmModel);
                    //addedFilm.FilmID = 0; 
                    //if film added
                    if (addedFilm.FilmID > 0)
                    {
                        return RedirectToAction("Index", "Account"); 
                    }
                    else
                    {
                        if(!_SL.Files.RemoveFIle(fileInfo))
                        {
                            ViewBag.ErrorMsg = fileInfo.Error;
                        }
                        else
                        {
                            ViewBag.ErrorMsg = "File does'n saved";
                        }
        
                    }
                } 

                return View(FilmAddModel.getFilmModelTemplate());
            }
            else
            {
                ViewBag.ErrorMsg = "No file posted";

                addFilmModel.Genres = _DAL.Genres.All();

                return View(FilmAddModel.getFilmModelTemplate());
            }
        }
    }
}