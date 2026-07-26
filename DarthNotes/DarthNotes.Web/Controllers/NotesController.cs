using DarthNotes.Web.DTO;
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
    public async Task<IActionResult> ListAsync()
    {
        var userId = User.FindFirst("Id")?.Value;
        var model = new NotesListModel();
        if (int.TryParse(userId, out var userIdValue))
        {
            var notes = await _notesService.ListAsync(userIdValue);
            model = _mapper.Map<NotesListModel>(notes);
        }

        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> SaveAsync(NoteModel model)
    {
        var tags = new List<string>();
        SeparateTagsFromName(model.Name, out tags);
        
        var dto = _mapper.Map<NoteDto>(model);
        if (tags.Any())
        {
            dto.Tags = tags.Select(x => new TagDto()
            {
                Name = x
            }).ToList();
        }
        
        var userId = User.FindFirst("Id")?.Value;
        if (int.TryParse(userId, out var userIdValue))
        {
            dto.UserId = userIdValue;
        }

        if (dto.Id is null || dto.Id == default(int))
        {
            model.Id = await _notesService.CreateAsync(dto);
        }
        else
        {
            await _notesService.UpdateAsync(dto);
        }

        model.Mode = ModelMode.View;
        return View("Note", model);
    }

    [HttpGet]
    public async Task<IActionResult> UpdateAsync(int? id)
    {
        NoteModel model;
        if (id.HasValue)
        {
            var note = await _notesService.GetAsync(id.Value);
            model = _mapper.Map<NoteModel>(note);
            model.Mode = ModelMode.Update;
        }
        else
        {
            model = new();
            model.Mode = ModelMode.Create;
        }
        
        return View("Note", model);
    }
    
    
    [HttpGet]
    public async Task<IActionResult> ViewAsync(int id)
    {
        NoteModel model;
        var note = await _notesService.GetAsync(id);
        model = _mapper.Map<NoteModel>(note);
        model.Mode = ModelMode.View;
        
        return View("Note", model);
    }

    [HttpGet]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        await _notesService.DeleteAsync(id);
        return RedirectToAction("List");
    }
    
    [HttpGet]
    public async Task<IActionResult> CreateAsync()
    {
        var model = new NoteModel()
        {
            Mode = ModelMode.Create
        };
        return View(model);
    }

    private string SeparateTagsFromName(string name, out List<string> tags)
    {
        tags = new();
        if (String.IsNullOrEmpty(name)) return name;
        var parts = name.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        tags = parts.Where(x => x.StartsWith("#")).ToList();
        return name;
    }
}