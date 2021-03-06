﻿using ImdbWeb.Models.MovieModels;
using MovieDAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ImdbWeb.Controllers
{
	[RoutePrefix("Movie")]
    public class MovieController : ImdbControllerBase
    {
		public async Task<ActionResult> Index()
		{
			ViewData.Model = await Db.Movies.ToListAsync();
			return View();
		}

		public async Task<ActionResult> Details(string id)
		{
			var movie = await Db.Movies.FindAsync(id);
			if(movie == null)
			{
				return HttpNotFound();
			}

			ViewData.Model = movie;
			if (Request.IsAjaxRequest())
			{
				return PartialView();
			}
			return View();
		}
		public async Task<ActionResult> Genres()
		{
			ViewData.Model = await Db.Genres.ToListAsync();
			return View();
		}
		[Route("Genre/{genrename:alpha}")]
		public async Task<ActionResult> MoviesByGenre(string genrename)
		{
			ViewData.Model = await Db.Movies.Where(m => m.Genre.Name == genrename).ToListAsync();
			
			ViewBag.Genre = genrename;
			//ViewData["Genre"] = genrename;

			return View("Index");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Vote(string id, int vote)
		{
			var movie = await Db.Movies.FindAsync(id);
			if (movie == null)
			{
				return HttpNotFound();
			}

			movie.Ratings.Add(new Rating { Vote = vote });
			await Db.SaveChangesAsync();

			ViewData.Model = new VoteResultModel {
				MovieId = id,
				YourVote = vote,
				VoteCount = movie.Ratings.Count(),
				AverageVote = movie.Ratings.Average(m => m.Vote)
			};

			if(Request.IsAjaxRequest())
			{
				return PartialView("_VoteResult");
			}

			return PartialView("VoteResult");

		}
	}
}