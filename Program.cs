using System;
using System.Collections.Generic;

//Singletone логування
public class Logger
{
    private static Logger _instance;
    private static readonly object _lock = new object();

    private Logger() { }

    public static Logger Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new Logger();
                }
                return _instance;
            }
        }
    }

    public void Log(string message)
    {
        Console.WriteLine($"[LOG - {DateTime.Now:HH:mm:ss}]: {message}");
    }
}

//adapter конвентер текстових форматів
public interface ITextReader
{
    string ReadData();
}

public class JsonFormat
{
    public string GetJsonContent() => "{ \"status\": \"success\", \"format\": \"JSON\" }";
}

public class XmlFormat
{
    public string GetXmlContent() => "<status>success</status><format>XML</format>";
}

public class JsonAdapter : ITextReader
{
    private readonly JsonFormat _jsonFormat;
    public JsonAdapter(JsonFormat jsonFormat) { _jsonFormat = jsonFormat; }
    public string ReadData() => $"Адаптовано JSON: {_jsonFormat.GetJsonContent()}";
}

public class XmlAdapter : ITextReader
{
    private readonly XmlFormat _xmlFormat;
    public XmlAdapter(XmlFormat xmlFormat) { _xmlFormat = xmlFormat; }
    public string ReadData() => $"Адаптовано XML: {_xmlFormat.GetXmlContent()}";
}


//Observer Сповіщення в чаті

public interface IChatObserver
{
    void ReceiveNotification(string chatName, string message);
}

public class ChatRoom
{
    private List<IChatObserver> _subscribers = new List<IChatObserver>();
    public string ChatName { get; private set; }

    public ChatRoom(string name) { ChatName = name; }

    public void Subscribe(IChatObserver observer) => _subscribers.Add(observer);
    public void Unsubscribe(IChatObserver observer) => _subscribers.Remove(observer);

    public void SendMessage(string message)
    {
        Console.WriteLine($"\n--- Нове повідомлення у чаті [{ChatName}]: {message} ---");
        NotifySubscribers(message);
    }

    private void NotifySubscribers(string message)
    {
        foreach (var subscriber in _subscribers)
        {
            subscriber.ReceiveNotification(ChatName, message);
        }
    }
}

public class ChatUser : IChatObserver
{
    private string _userName;
    public ChatUser(string name) { _userName = name; }

    public void ReceiveNotification(string chatName, string message)
    {
        Console.WriteLine($"[{_userName}] отримав сповіщення з [{chatName}]: {message}");
    }
}


// КЛІЄНТСЬКИЙ КОД (Точка входу)

class Program
{
    static void Main()
    {
        
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        //Singleton
        Logger.Instance.Log("Система ініціалізована.");
        Logger.Instance.Log("Завантаження модулів...");

        //Adapter
        ITextReader jsonReader = new JsonAdapter(new JsonFormat());
        ITextReader xmlReader = new XmlAdapter(new XmlFormat());
        
        Logger.Instance.Log("Читання форматів через адаптери:");
        Console.WriteLine(jsonReader.ReadData());
        Console.WriteLine(xmlReader.ReadData());

        //Observer
        ChatRoom workChat = new ChatRoom("Робочий Чат (СН-21)");
        ChatRoom gameChat = new ChatRoom("PUBG Squad");

        ChatUser user1 = new ChatUser("Сергій");
        ChatUser user2 = new ChatUser("Олег");

        workChat.Subscribe(user1);
        workChat.Subscribe(user2);
        gameChat.Subscribe(user1);

        workChat.SendMessage("Не забудьте здати звіт з патернів!");
        gameChat.SendMessage("Заходимо в лобі через 5 хвилин.");
    }
}