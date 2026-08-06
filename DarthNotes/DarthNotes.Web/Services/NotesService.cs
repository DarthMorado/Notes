using DarthNotes.DB;
using DarthNotes.DB.Entities;
using DarthNotes.DB.Repositories;
using DarthNotes.Web.DTO;
using DarthNotes.Web.Models;
using DarthNotes.Web.Services.Auth;
using MapsterMapper;

namespace DarthNotes.Web.Services;

public interface INotesService
{
    Task<int?> CreateAsync(NoteDto note);
    Task<NoteDto> GetAsync(int id);
    Task UpdateAsync(NoteDto note);
    Task<bool> DeleteAsync(int id);
    
    Task<List<NoteDto>> ListAsync(int userId);
    
}

public class NotesService : INotesService
{
    private readonly IMapper _mapper;    
    private readonly INotesRepository _noteRepository;
    private readonly IBaseRepository<TagEntity> _tagsRepository;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _uow;
    
    public NotesService(IMapper mapper,
        INotesRepository noteRepository,
        IBaseRepository<TagEntity> tagsRepository,
        IUserContext userContext,
        IUnitOfWork uow)
    {
        _uow = uow;
        _mapper = mapper;
        _noteRepository = noteRepository;
        _userContext = userContext;
        _tagsRepository = tagsRepository;
    }

    public async Task UpdateAsync(NoteDto note)
    {
        var entity = _mapper.Map<NoteEntity>(note);
        await _noteRepository.UpdateAsync(entity);
        await _uow.SaveChangesAsync();
    }

    private async Task<TagEntity> GetTagEntityAsync(string tagName)
    {
        var existingTags = await _tagsRepository.FindAsync(x => x.Name == tagName);
        if (existingTags != null && existingTags.Any())
        {
            return existingTags.First();
        }

        var newTag = new TagEntity()
        {
            Name = tagName
        };
        await _tagsRepository.AddAsync(newTag);
        await _tagsRepository.SaveChangesAsync();

        return newTag;
    }
    
    public async Task<int?> CreateAsync(NoteDto note)
    {
        //await PrepareAsync(note); //Populates Tags parameter
        var entity = _mapper.Map<NoteEntity>(note);
        
        //Check user
        if (entity.UserId != _userContext.GetUserId())
        {
            return null;
        }

        using (var scope = _uow.CreateScope())
        {
            entity.Tags = new();
            foreach (var tagName in ParseTagNames(entity.Name))
            {
                var tag = await GetTagEntityAsync(tagName);
                entity.Tags.Add(tag);
            }
            
            await _noteRepository.AddAsync(entity);
            await scope.Complete();
        }

        return entity.Id;
    }

    public async Task<List<NoteDto>> ListAsync(int userId)
    {
        var entities = await _noteRepository.FindAsync(x => x.UserId == userId);
        var notes = _mapper.Map<List<NoteDto>>(entities);
        return notes;
    }

    public async Task<NoteDto> GetAsync(int id)
    {
        var note = await _noteRepository.GetByIdAsync(id);
        var noteDto = _mapper.Map<NoteDto>(note);
        return noteDto;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var note = await _noteRepository.GetByIdAsync(id);
        if (note.UserId == _userContext.GetUserId())
        {
            await _noteRepository.RemoveAsync(note);
            await _noteRepository.SaveChangesAsync();
            return true;
        }

        return false;
    }

    private List<string> ParseTagNames(string input)
    {
        return input.Split(' ', '\r', '\n')
            .Where(x => !String.IsNullOrWhiteSpace(x)).ToList()
            .Where(x => x.StartsWith("#"))
            .ToList();
    }
    
    // private async Task PrepareAsync(NoteDto note)
    // {
    //     note.Tags = note.Name.Split(' ', '\r', '\n')
    //         .Where(x => !String.IsNullOrWhiteSpace(x)).ToList()
    //         .Where(x => x.StartsWith("#"))
    //         //.ToList();
    //         .Select(x => new TagDto() { Name = x })
    //         .ToList();
    //
    //     foreach (var tag in note.Tags)
    //     {
    //             var existingTags = await _tagsRepository.FindAsync(x => x.Name == tag.Name, noTracking: true);
    //             if (existingTags.Any())
    //             {
    //                 var existingTag = existingTags.First();
    //                 tag.Id = existingTag.Id;
    //             }
    //     }
    //
    // }
}