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
        var model = new NotesListModel();
        if (int.TryParse(userId, out var userIdValue))
        {
            var notes = await _notesService.ListAsync(userIdValue);
            model = _mapper.Map<NotesListModel>(notes);
        }

        return View("ListQuick", model);
    }
    
    [HttpPost]
    public async Task<IActionResult> SaveQuick(NoteModel model)
    {
        var dto = _mapper.Map<NoteDto>(model);
        var userId = User.FindFirst("Id")?.Value;
        if (int.TryParse(userId, out var userIdValue))
        {
            dto.UserId = userIdValue;
        }

        if (dto.Id is null || dto.Id == default(int))
        {
            await _notesService.CreateAsync(dto);
        }
        else
        {
            await _notesService.UpdateAsync(dto);
        }

        return RedirectToAction("Index","Home");
    }

    [HttpGet]
    public async Task<IActionResult> UpdateAsync(int? id)
    {
        NoteModel model;
        if (id.HasValue)
        {
            var note = await _notesService.GetAsync(id.Value);
            model = _mapper.Map<NoteModel>(note);
        }
        else
        {
            model = new();
        }

        return View("Update", model);
    }

    // [HttpPost]
    // public async Task<IActionResult> EditNoteAsync(QuickNoteModel model)
    // {
    // }

    [HttpGet]
    public async Task<IActionResult> DeleteNoteAsync(int id)
    {
        await _notesService.DeleteAsync(id);
        return RedirectToAction("ListQuick");
    }
    
    public async Task<IActionResult> CreateQuick()
    {
        var model = new NoteModel();
        return View("CreateQuick", model);
    }
}