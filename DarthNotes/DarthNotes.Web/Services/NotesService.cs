using DarthNotes.DB;
using DarthNotes.DB.Entities;
using DarthNotes.DB.Repositories;
using DarthNotes.Web.Models;
using DarthNotes.Web.Services.Auth;
using MapsterMapper;

namespace DarthNotes.Web.Services;

public interface INotesService
{
    Task CreateAsync(NoteDto note);
    Task<NoteDto> GetAsync(int id);
    Task UpdateAsync(NoteDto note);
    Task<bool> DeleteAsync(int id);
    
    Task<List<NoteDto>> ListAsync(int userId);
    
}

public class NotesService : INotesService
{
    private readonly IMapper _mapper;    
    private readonly IBaseRepository<NoteEntity> _noteRepository;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _uow;
    
    public NotesService(IMapper mapper,
        IBaseRepository<NoteEntity> noteRepository,
        IUserContext userContext,
        IUnitOfWork uow)
    {
        _uow = uow;
        _mapper = mapper;
        _noteRepository = noteRepository;
        _userContext = userContext;
    }

    public async Task UpdateAsync(NoteDto note)
    {
        var entity = _mapper.Map<NoteEntity>(note);
        await _noteRepository.UpdateAsync(entity);
        await _uow.SaveChangesAsync();
    }
    
    public async Task CreateAsync(NoteDto note)
    {
        var entity = _mapper.Map<NoteEntity>(note);
        
        //Check user
        if (entity.UserId != _userContext.GetUserId())
        {
            return;
        }

        using (var scope = _uow.CreateScope())
        {
            await _noteRepository.AddAsync(entity);
            await scope.Complete();
        }
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
}