using FilmsStorage.Models;
using FilmsStorage.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FilmsStorage.Mappers
{
    public class FilmMapper
    {
        public static Film FromAddModel(FilmAddModel filmAddModel)
        {
            return new Film()
            {
                FilmName = filmAddModel.FilmName,
                ReleaseYear = filmAddModel.ReleaseYear,
                fk_GenreID = filmAddModel.GenreID,
                fk_UserID = filmAddModel.UserID,
                FileName = filmAddModel.FileName,
                FilePath = filmAddModel.FilePath,
                FilmDescription = filmAddModel.FilmDescription
            };
        }
        public static Film FromFilmToNewFilm(ref Film oldFilm, Film newFilm)
        {
            oldFilm.FilmName = newFilm.FilmName;
            oldFilm.ReleaseYear = newFilm.ReleaseYear;
            oldFilm.fk_GenreID = newFilm.fk_GenreID;
            oldFilm.fk_UserID = newFilm.fk_UserID;
            oldFilm.FileName = newFilm.FileName;
            oldFilm.FilePath = newFilm.FilePath;
            oldFilm.FilmDescription = newFilm.FilmDescription;
            return oldFilm;
        }

    }
}