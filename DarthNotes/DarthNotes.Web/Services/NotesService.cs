using DarthNotes.DB.Entities;
using DarthNotes.DB.Repositories;
using DarthNotes.Web.Models;
using DarthNotes.Web.Services.Auth;
using MapsterMapper;

namespace DarthNotes.Web.Services;

public interface INotesService
{
    Task UpdateAsync(QuickNoteDto note);
    Task CreateQuickAsync(QuickNoteDto note);
    Task<List<QuickNoteDto>> ListAsync(int userId);
    Task<QuickNoteDto> GetNoteAsync(int id);
    Task<bool> DeleteAsync(int id);
}

public class NotesService : INotesService
{
    private readonly IMapper _mapper;    
    private readonly IBaseRepository<QuickNoteEntity> _quickNoteRepository;
    public readonly IUserContext _userContext;
    
    public NotesService(IMapper mapper,
        IBaseRepository<QuickNoteEntity> quickNoteRepository,
        IUserContext userContext)
    {
        _mapper = mapper;
        _quickNoteRepository = quickNoteRepository;
        _userContext = userContext;
    }

    public async Task UpdateAsync(QuickNoteDto note)
    {
        var entity = _mapper.Map<QuickNoteEntity>(note);
        await _quickNoteRepository.UpdateAsync(entity);
        await _quickNoteRepository.SaveChangesAsync();
    }
    
    public async Task CreateQuickAsync(QuickNoteDto note)
    {
        var entity = _mapper.Map<QuickNoteEntity>(note);
        // entity.User = new()
        // {
        //     Id = entity.UserId
        // };
        await _quickNoteRepository.AddAsync(entity);
        await _quickNoteRepository.SaveChangesAsync();
    }

    public async Task<List<QuickNoteDto>> ListAsync(int userId)
    {
        var entities = await _quickNoteRepository.FindAsync(x => x.UserId == userId);
        var notes = _mapper.Map<List<QuickNoteDto>>(entities);
        return notes;
    }

    public async Task<QuickNoteDto> GetNoteAsync(int id)
    {
        var note = await _quickNoteRepository.GetByIdAsync(id);
        var noteDto = _mapper.Map<QuickNoteDto>(note);
        return noteDto;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var note = await _quickNoteRepository.GetByIdAsync(id);
        if (note.UserId == _userContext.GetUserId())
        {
            await _quickNoteRepository.RemoveAsync(note);
            await _quickNoteRepository.SaveChangesAsync();
            return true;
        }

        return false;
    }
}