using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

public enum GameMode
{
    Offline = 0,
    Online  = 1
};

/// <remarks>Для корректного запуска игры важно, чтобы первые 3 элемента (симуляторы) имели именно такие значения</remarks>
public enum PlatformType
{
    XDMotion    = 0,
    FlyMotion   = 1,
    FiveDMotion = 2,
//----------------------
    Desktop     = 3,
    HTCVive     = 4,
    Spectator   = 5
};

/// <summary>
/// Сообщение, содержащее информацию о настройках, задаваемых сервером
/// </summary>
public class SettingsMessage : UnityEngine.Networking.MessageBase
{
    public bool friendlyFire;
}

/// <summary>
/// Этот класс обеспечивает доступ к настройкам приложения, т.е.
/// к инфе из конфиг-файла и из параметров запуска
/// </summary>
public static class Settings
{
    public static GameMode gameMode
    {
        get; private set;
    }

    public static PlatformType platformType
    {
        get; private set;
    }

    public static List<string> networkServersIP
    {
        get; private set;
    }

    public static bool friendlyFire { get; private set; }

    private static string[]  serverIPs;
    private static int[]     serverPorts;
    private static string    configFilePath;

    /// <summary>
    /// Конструктор. При создании объекта задаются значения по умолчанию и делается
    /// попытка чтения параметров сначала из файла, а затем из аргументов запуска.
    /// Значения из аргументов запуска перекрывают значения из конфигурационного файла.
    /// </summary>
    static Settings()
    {
        gameMode         = GameMode.Offline;
        platformType     = PlatformType.Desktop;
        serverIPs        = new string[3];
        serverPorts      = new int[3];
        networkServersIP = new List<string>() {"127.0.0.1"};
        configFilePath   = Application.dataPath + "/StreamingAssets/config.xml";

        ReadConfigurationFile();
        ReadStartArguments();
    }

    /// <summary>
    /// Возращает IP адрес Fly, XD или 5D сервера
    /// </summary>
    /// <param name="platformType">Тип платформы</param>
    /// <returns>IP адрес</returns>
    public static string GetServerIP(PlatformType platformType)
    {
        return serverIPs[(int) platformType];
    }

    /// <summary>
    /// Возращает порт Fly, XD или 5D сервера
    /// </summary>
    /// <param name="platformType">Тип платформы</param>
    /// <returns>Порт</returns>
    public static int GetServerPort(PlatformType platformType)
    {
        return serverPorts[(int) platformType];
    }

    /// <summary>
    /// Чтение настроек из аргументов запуска
    /// </summary>
    private static void ReadStartArguments()
    {
        //Первый параметр - имя файла
        //Второй параметр - gameMode/platformType
        string[] arguments = Environment.GetCommandLineArgs();

        if (arguments.Length == 2 && arguments[1].Contains(@"/"))
        {
            //Строка должна быть вида "x/y", где x - 0 или 1 (gameMode),
            //y - 0, 1 или 2 (platformType)
            Match match = Regex.Match(arguments[1], @"^[01]\/[012]$");

            if (match.Success)
            {
                string[] gameSettings = arguments[1].Split('/');
                gameMode     = (GameMode)     short.Parse(gameSettings[0]);
                platformType = (PlatformType) short.Parse(gameSettings[1]);
            }
        }
    }

    /// <summary>
    /// Чтение настроек из конфигурационного файла
    /// </summary>
    private static void ReadConfigurationFile()
    {
        if (!File.Exists(configFilePath))
        {
            Debug.LogError("Файл конфигурации не найден!");
            return;
        }

        var doc = new XmlDocument();
        doc.Load(configFilePath);

        foreach (XmlNode node in doc.SelectNodes("configuration"))
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                switch (child.Name)
                {
                    case "FlyServerIP":
                        serverIPs[(int) PlatformType.FlyMotion] = child.InnerText;
                        continue;

                    case "FlyServerPort":
                        serverPorts[(int) PlatformType.FlyMotion] = int.Parse(child.InnerText);
                        continue;

                    case "XDServerIP":
                        serverIPs[(int) PlatformType.XDMotion] = child.InnerText;
                        continue;

                    case "XDServerPort":
                        serverPorts[(int) PlatformType.XDMotion] = int.Parse(child.InnerText);
                        continue;

                    case "FiveDServerIP":
                        serverIPs[(int) PlatformType.FiveDMotion] = child.InnerText;
                        continue;

                    case "FiveDServerPort":
                        serverPorts[(int) PlatformType.FiveDMotion] = int.Parse(child.InnerText);
                        continue;

                    case "GameMode":
                        gameMode = (GameMode) Enum.Parse(typeof(GameMode), child.InnerText);
                        continue;

                    case "PlatformType":
                        platformType = (PlatformType) Enum.Parse(typeof(PlatformType), child.InnerText);
                        continue;

                    case "FriendlyFire":
                        friendlyFire = bool.Parse(child.InnerText);
                        continue;
                }

                Match nameMatch = Regex.Match(child.Name, @"Server\d");
                if (nameMatch.Success)
                    networkServersIP.Add(child.InnerText);
            }
        }
    }

    /// <summary>
    /// Создаёт сообщение с настройками для отправки их от сервера клиентам
    /// </summary>
    public static SettingsMessage GetSettingsMessage()
    {
        SettingsMessage settingsMessage = new SettingsMessage();

        settingsMessage.friendlyFire = Settings.friendlyFire;

        return settingsMessage;
    }
    /// <summary>
    /// Обновляет настроки на стороне клиента на те, что пришли от сервера
    /// </summary>
    /// <param name="settings"></param>
    public static void UpdateClientSettings(SettingsMessage settings)
    {
        friendlyFire = settings.friendlyFire;
    }
}
