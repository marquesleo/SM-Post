using CQRS.Core.Commands;
using CQRS.Core.InfraEstructure;

namespace Post.Cmd.Infrastructure.Dispatchers;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly Dictionary<Type, Func<BaseCommand, Task>> _handlers =
        new Dictionary<Type, Func<BaseCommand, Task>>();
    
    public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand
    {
        if (_handlers.ContainsKey(typeof(T)))
        {
            throw new IndexOutOfRangeException($"Handler {typeof(T).Name} already registered");
        }
        _handlers.Add(typeof(T), x => handler((T)x));
    }

    public async Task SendAsync(BaseCommand command)
    {
        if (_handlers.TryGetValue(command.GetType(), out var handler))
        {
            await handler(command); 
        }
        else
        {
            throw new ArgumentException(nameof(handler), "No command handler was registered");
        }
            
    }
}