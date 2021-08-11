using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DichVuGame.Data;
using DichVuGame.Models;
using DichVuGame.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.Net.WebSockets;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;
using DichVuGame.Utility;
using Newtonsoft.Json;
using System.Net;
using Microsoft.AspNetCore.Routing;
using DichVuGame.Services;
using System.Net.Http;

namespace DichVuGame.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GamesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        [BindProperty]
        public GamesViewModel GamesViewModel { get; set; }
        public HttpApi api;
        public GamesController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostEnvironment;
            GamesViewModel = new GamesViewModel()
            {
                Countries = _context.Countries.AsNoTracking().ToList(),
                GameTags = _context.GameTags.AsNoTracking().ToList(),
                Studios = _context.Studios.AsNoTracking().ToList(),
                Games = _context.Games.AsNoTracking().ToList(),
                Game = new Game(),
                Studio = new Studio(),
                SystemRequirement = new SystemRequirement()
            };
            api = new HttpApi("https://localhost:44387/api/games");
        }
        // GET: Admin/Games
        public ActionResult Index(string q = null)
        {
            CallAPI callAPI = new CallAPI("https://localhost:44387/api/games"); 
            List<Game> games = JsonConvert.DeserializeObject<List<Game>>(callAPI.GetResponse());
            GamesViewModel.Games = games;
            if (q == "Selling")
            {
                GamesViewModel.Games = games.Where(u => u.IsPublish==true).ToList();
            }
            if (q == "Waiting")
            {
                GamesViewModel.Games = games.Where(u => u.IsPublish == false).ToList();
            }
            return View(GamesViewModel);
            
        }
         // GET: Admin/Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Studio)
                .Include(g => g.SystemRequirement)
                .FirstOrDefaultAsync(m => m.ID == id);
            var studio = await _context.Studios.Where(s => s.ID == game.StudioID).FirstOrDefaultAsync();
            var gameTag = await _context.GameTags.Where(g => g.GameID == game.ID).ToListAsync();
            if (game == null)
            {
                return NotFound();
            }
            GamesViewModel.Game = game;
            GamesViewModel.Studio = studio;
            GamesViewModel.GameTags = gameTag;
            return View(GamesViewModel);
        }
        // GET: Admin/Games/Create
        public IActionResult Create()
        {
            ViewData["StudioID"] = new SelectList(_context.Studios, "ID", "Studioname");
            return View();
        }

        // POST: Admin/Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            if (ModelState.IsValid)
            {
                if(GameExists(GamesViewModel.Game.Gamename) == false)
                {
                    GamesViewModel.Game.AvailableCode = 0;
                    _context.Games.Add(GamesViewModel.Game);
                    await _context.SaveChangesAsync();

                    var gameFromDb = _context.Games.Find(GamesViewModel.Game.ID);
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    var files = HttpContext.Request.Form.Files;
                    if (GamesViewModel.Tags != null)
                    {
                        List<string> tags = HandingBareTag(GamesViewModel.Tags);
                        foreach (var temptag in tags)
                        {
                            Tag tag = new Tag()
                            {
                                Tagname = temptag
                            };
                            _context.Tags.Add(tag);
                            await _context.SaveChangesAsync();
                            GameTag gameTag = new GameTag()
                            {
                                GameID = GamesViewModel.Game.ID,
                                TagID = tag.ID
                            };
                            _context.GameTags.Add(gameTag);
                            await _context.SaveChangesAsync();
                        }
                    }
                    if (files.Count != 0)
                    {
                        var uploads = Path.Combine(webRootPath, SD.GameImageFolder);
                        var extension = Path.GetExtension(files[0].FileName);
                        using (var fileStream = new FileStream(Path.Combine(uploads, GamesViewModel.Game.ID + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        gameFromDb.GamePoster = @"\" + SD.GameImageFolder + @"\" + GamesViewModel.Game.ID + extension;
                    }
                    else
                    {
                        var uploads = Path.Combine(webRootPath, SD.GameImageFolder + @"\" + SD.DefaultGameImage);
                        System.IO.File.Copy(uploads, webRootPath + @"\" + SD.GameImageFolder + @"\" + GamesViewModel.Game.ID + ".png");
                        gameFromDb.GamePoster = @"\" + SD.GameImageFolder + @"\" + GamesViewModel.Game.ID + ".png";
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("SameGame", "Đã có game này trên hệ thống");
                    return View(GamesViewModel);
                }
                
            }
            ViewData["StudioID"] = new SelectList(_context.Studios, "ID", "Studioname", GamesViewModel.Game.StudioID);
            return View(GamesViewModel);
        }

        // GET: Admin/Games/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var game = await _context.Games.Where(u => u.ID == id).FirstOrDefaultAsync();
            if (game == null)
            {
                return NotFound();
            }
            ViewData["StudioID"] = new SelectList(_context.Studios, "ID", "Studioname", game.StudioID);
            return View(game);
        }

        // POST: Admin/Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost,ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPOST(int id, [Bind("ID,Gamename,GameDescription,GamePoster,Release,StudioID,Price,Available")] Game game)
        {
            if (id != game.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var webRootPath = _hostingEnvironment.WebRootPath;
                    var files = HttpContext.Request.Form.Files;         
                    if (files.Count > 0)
                    {
                        if (game.GamePoster != null)
                        {
                            var old = webRootPath + @"" + game.GamePoster;
                            System.IO.File.Delete(old);
                        }
                        var path = Path.Combine(webRootPath, SD.GameImageFolder);
                        var extension = Path.GetExtension(files[0].FileName);
                        using(var fileStream = new FileStream(Path.Combine(path,game.ID + extension),FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        game.GamePoster = @"\" + SD.GameImageFolder + @"\" + game.ID + extension;
                    }
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["StudioID"] = new SelectList(_context.Studios, "ID", "Studioname", game.StudioID);
            return View(game);
        }

        // GET: Admin/Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Studio)
                .Include(g => g.SystemRequirement)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Admin/Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int gameid)
        {
            HttpClient client = api.init();
            HttpResponseMessage response = await client.DeleteAsync(api.ApiUrl + $"/{gameid}");
            var result = response.Content.ReadAsStringAsync().Result;
            var game = JsonConvert.DeserializeObject<Game>(result);
            if(game.GamePoster != null)
            {
                System.IO.File.Delete(_hostingEnvironment.WebRootPath + @"" + game.GamePoster);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.ID == id);
        }
        public List<string> HandingBareTag(string bareTag)
        {
            List<string> tag = new List<string>();
            int start = 0;
            int end = 0;
            while (start < bareTag.Length)
            {
                string temp = "";
                for (int j = start; j < bareTag.Length; j++)
                {
                    if (bareTag[j] == ',')
                    {
                        end = j;
                        break;
                    }
                    if (j == bareTag.Length - 1)
                    {
                        end = j + 1;
                        break;
                    }
                }
                for (int k = start; k < end; k++)
                {
                    temp += bareTag[k];
                }
                start = end + 1;
                tag.Add(temp.Trim());
            }
            return tag;
        }
        public async Task<IActionResult> ManageGame()
        {
            var games = await _context.Games.Include(u => u.Studio).ToListAsync();
            return View(games);
        }
        public async Task<IActionResult> Publish(int id)
        {
            var game = await _context.Games.Where(u => u.ID == id).FirstOrDefaultAsync();
            game.IsPublish = true;
            _context.SaveChanges();
            return RedirectToAction("ManageGame");
        }
        public async Task<IActionResult> DePublish(int id)
        {
            var game = await _context.Games.Where(u => u.ID == id).FirstOrDefaultAsync();
            game.IsPublish = false;
            _context.SaveChanges();
            return RedirectToAction("ManageGame");
        }
        private bool GameExists(string gamename)
        {
            return _context.Games.Any(e => e.Gamename == gamename);
        }
    }
}
