using DarthNotes.Web.Models;
using DarthNotes.Web.Services;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace DarthNotes.Web.Controllers;

public class NotesController : Controller
{
    private readonly IMapper _mapper;
    private readonly INotesService _notesService;
    public NotesController(IMapper mapper,
        INotesService notesService)
    {
        _mapper = mapper;
        _notesService = notesService;
    }

    [HttpGet]
    public async Task<IActionResult> ListQuick()
    {
        var userId = User.FindFirst("Id")?.Value;
        var model = new ListQuickModel();
        if (int.TryParse(userId, out var userIdValue))
        {
            var notes = await _notesService.ListAsync(userIdValue);
            model = _mapper.Map<ListQuickModel>(notes);
        }

        return View("ListQuick", model);
    }
    
    [HttpPost]
    public async Task<IActionResult> SaveQuick(QuickNoteModel model)
    {
        var dto = _mapper.Map<QuickNoteDto>(model);
        var userId = User.FindFirst("Id")?.Value;
        if (int.TryParse(userId, out var userIdValue))
        {
            dto.UserId = userIdValue;
        }

        if (dto.Id is null || dto.Id == default(int))
        {
            await _notesService.CreateQuickAsync(dto);
        }
        else
        {
            await _notesService.UpdateAsync(dto);
        }

        return RedirectToAction("Index","Home");
    }

    [HttpGet]
    public async Task<IActionResult> EditNoteAsync(int id)
    {
        var note = _notesService.GetNoteAsync(id);
        var model = _mapper.Map<QuickNoteModel>(note);
        return View("EditNote", model);
    }

    // [HttpPost]
    // public async Task<IActionResult> EditNoteAsync(QuickNoteModel model)
    // {
    // }

    [HttpPost("{id}")]
    public async Task<IActionResult> DeleteNoteAsync(int id)
    {
        await _notesService.DeleteAsync(id);
        return RedirectToAction("ListQuick");
    }
    
    public async Task<IActionResult> CreateQuick()
    {
        var model = new QuickNoteModel();
        return View("CreateQuick", model);
    }
}