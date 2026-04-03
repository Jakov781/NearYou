using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using WebTemplate.Services;

namespace WebTemplate.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IChatService chatService, ApplicationDbContext context, ILogger<ChatHub> logger)
    {
        _chatService = chatService;
        _context = context;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        if (userId.HasValue)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");

            try
            {
                var chatovi = await _chatService.GetUserChatsAsync(userId.Value);
                await Clients.Caller.SendAsync("ActiveChats", chatovi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška pri slanju aktivnih chatova korisniku {UserId}", userId);
            }
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        if (userId.HasValue)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(int chatId, string message)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            await Clients.Caller.SendAsync("Error", "Korisnik nije autentifikovan");
            return;
        }

        try
        {
            var porukaDto = await _chatService.SendMessageAsync(chatId, userId.Value, message);
            await Clients.Caller.SendAsync("MessageSent", porukaDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Greška pri slanju poruke u chat {ChatId}", chatId);
            await Clients.Caller.SendAsync("Error", ex.Message);
        }
    }

    public async Task CreateChatFromPrijava(int prijavaId)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            await Clients.Caller.SendAsync("Error", "Korisnik nije autentifikovan");
            return;
        }

        try
        {
            var chatDto = await _chatService.CreateChatFromPrijavaAsync(prijavaId, userId.Value);

            await Clients.Group($"user_{chatDto.KlijentId}").SendAsync("ChatCreated", chatDto);
            await Clients.Group($"user_{chatDto.OglasivacId}").SendAsync("ChatCreated", chatDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Greška pri kreiranju chata iz prijave {PrijavaId}", prijavaId);
            await Clients.Caller.SendAsync("Error", ex.Message);
        }
    }

    private int? GetUserId()
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}