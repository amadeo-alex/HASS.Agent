using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.Base.Helpers;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using MQTTnet.Exceptions;

namespace HASS.Agent.Base.Managers;
public class LogManager : ILogManager
{
    private static string RemoveNonAlphanumericCharacters(string value) => new(value.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray());

    private readonly List<string> LoggedFirstChanceHttpRequestExceptions = new();
    private readonly IVariableManager _variableManager;
    private readonly Logger? _logger;

    private string _lastLog = string.Empty;


    public bool ExtendedLoggingEnabled { get; set; } = false;
    public LoggingLevelSwitch LoggingLevelSwitch { get; } = new()
    {
        MinimumLevel = LogEventLevel.Information
    };

    public LogManager(IVariableManager variableManager)
    {
        _variableManager = variableManager;
    }

    public Logger GetLogger(string[] arguments)
    {
        if (_logger != null)
            return _logger;

        var logTag = arguments.Length > 1
            ? $"{RemoveNonAlphanumericCharacters(arguments.First(x => !string.IsNullOrEmpty(x)))}_"
            : string.Empty;

        var elevatedTag = ElevationHelper.RunningElevated() ? "[E]" : "";
        var logName = $"[{DateTime.Now:yyyy-MM-dd}]{elevatedTag} {_variableManager.ApplicationName}_{logTag}.log";

        var logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(LoggingLevelSwitch)
            .WriteTo.Async(a =>
                a.File(Path.Combine(_variableManager.LogPath, logName),
                    rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: 10000000,
                    retainedFileCountLimit: 10,
                    rollOnFileSizeLimit: true,
                    buffered: true,
                    flushToDiskInterval: TimeSpan.FromMilliseconds(150)))
            .CreateLogger();

        return logger ?? throw new Exception("cannot create logger instance");
    }
    public void OnFirstChanceExceptionHandler(object sender, FirstChanceExceptionEventArgs eventArgs)
    {
        try
        {

            if (!string.IsNullOrEmpty(_lastLog))
            {
                if (_lastLog == eventArgs.Exception.ToString())
                    return;
                if (eventArgs.Exception.ToString().Contains(_lastLog))
                    return;
            }

            _lastLog = eventArgs.Exception.ToString();


            switch (eventArgs.Exception)
            {
                case FileNotFoundException fileNotFoundException:
                    HandleFirstChanceFileNotFoundException(fileNotFoundException);
                    return;

                case SocketException socketException:
                    HandleFirstChanceSocketException(socketException);
                    return;

                case WebException webException:
                    HandleFirstChanceWebException(webException);
                    return;

                case HttpRequestException httpRequestException:
                    HandleFirstChanceHttpRequestException(httpRequestException);
                    return;

                case MqttCommunicationException mqttCommunicationException:
                    HandleFirstChanceMqttCommunicationException(mqttCommunicationException);
                    return;

                default:
                    Log.Fatal(eventArgs.Exception, "[PROGRAM] FirstChanceException: {err}", eventArgs.Exception.Message);
                    break;
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[PROGRAM] Error processing FirstChanceException: {err}", ex.Message);
        }
    }

    private void HandleFirstChanceFileNotFoundException(FileNotFoundException fileNotFoundException)
    {
        // ignore resources
        if (fileNotFoundException.FileName != null && fileNotFoundException.FileName.Contains("resources")) return;

        Log.Error("[PROGRAM] FileNotFoundException: {err}", fileNotFoundException.Message);
    }

    private void HandleFirstChanceSocketException(SocketException socketException)
    {
        var socketErrorCode = socketException.SocketErrorCode;
        switch (socketErrorCode)
        {
            case SocketError.ConnectionRefused:
                Log.Error("[NET] [{type}] {err}", socketErrorCode.ToString(), socketException.Message);
                return;

            case SocketError.ConnectionReset:
                Log.Error("[NET] [{type}] {err}", socketErrorCode.ToString(), socketException.Message);
                return;

            default:
                Log.Fatal(socketException, "[NET] [{type}] {err}", socketErrorCode.ToString(), socketException.Message);
                break;
        }
    }

    private void HandleFirstChanceWebException(WebException webException)
    {
        var status = webException.Status;
        switch (status)
        {
            case WebExceptionStatus.ConnectFailure:
                Log.Error("[NET] [{type}] {err}", status.ToString(), webException.Message);
                return;

            case WebExceptionStatus.Timeout:
                Log.Error("[NET] [{type}] {err}", status.ToString(), webException.Message);
                return;

            default:
                Log.Fatal(webException, "[NET] [{type}] {err}", status.ToString(), webException.Message);
                return;
        }
    }

    private void HandleFirstChanceHttpRequestException(HttpRequestException httpRequestException)
    {
        // usually contains a more interesting inner exception
        if (httpRequestException.InnerException != null)
        {
            switch (httpRequestException.InnerException)
            {
                case SocketException sE:
                    HandleFirstChanceSocketException(sE);
                    break;
                case WebException wE:
                    HandleFirstChanceWebException(wE);
                    break;
            }
        }

        // only log once to prevent spamming
        var excMsg = httpRequestException.ToString();
        if (LoggedFirstChanceHttpRequestExceptions.Contains(excMsg)) return;
        LoggedFirstChanceHttpRequestExceptions.Add(excMsg);

        if (excMsg.Contains("SocketException"))
        {
            Log.Error("[NET] [{type}] {err}", "FirstChanceHttpRequestException.SocketException", httpRequestException.Message);
            return;
        }

        // just log it
        Log.Fatal(httpRequestException, "[NET] FirstChanceHttpRequestException: {err}", httpRequestException.Message);
    }

    private void HandleFirstChanceMqttCommunicationException(MqttCommunicationException mqttCommunicationException)
    {
        // usually contains a more interesting inner exception
        if (mqttCommunicationException.InnerException != null)
        {
            switch (mqttCommunicationException.InnerException)
            {
                case SocketException sE:
                    HandleFirstChanceSocketException(sE);
                    break;
                case WebException wE:
                    HandleFirstChanceWebException(wE);
                    break;
            }
        }

        // check for exceptions in message
        var excMsg = mqttCommunicationException.ToString();
        if (excMsg.Contains("SocketException"))
        {
            Log.Error("[NET] [{type}] {err}", "MqttCommunicationException.SocketException", mqttCommunicationException.Message);
            return;
        }
        if (excMsg.Contains("OperationCanceledException"))
        {
            Log.Error("[NET] [{type}] {err}", "MqttCommunicationException.OperationCanceledException", mqttCommunicationException.Message);
            return;
        }

        // just log it
        Log.Fatal(mqttCommunicationException, "[NET] FirstChancemqttCommunicationException: {err}", mqttCommunicationException.Message);
    }
}
