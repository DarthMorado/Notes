using DarthNotes.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace DarthNotes.Web.Controllers;

public class NotesController : Controller
{
    public NotesController()
    {
        
    }

    public async Task<IActionResult> CreateQuick()
    {
        var model = new QuickNoteModel()
        {
            UserId = int.Parse(User.FindFirst("Id").Value)
        };
        return View("CreateQuick", model);
    }
}