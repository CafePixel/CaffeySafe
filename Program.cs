using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Cafey.Utils;

/// <summary>
/// Fonction Safe utile lors de mon codage sous c#.
/// J'avais besoin d'éviter les appels à chaque fois.
/// 
/// UTILISATIONS : 
/// 
/// De base, pour l'utiliser il faut faire le code suivant : 
/// 
/// TryOrPass(() => fonctionATest, "contexte si besoin", "nom de la fonction");
/// 
/// Il loguera proprement mais n'interrompera pas le programme.
/// 
/// En revanche 
/// 
/// TryOrPanic(() => fonctionATest, "context si besoin", "nom de la fonction");
/// Va interrompre l'éxecution et renvoyer proprement l'erreur.
/// 
/// Lors du codage, je vous propose de coder proprement avec TryOrPanic(); et d'utiliser TryOrStackTrace(); lors du dernier fichier. 
/// 
/// 2025 - Caffey
/// </summary>
public static class Safe
{

    private static readonly SafeWriter DefaultWriter = new("Console", Console.WriteLine);

    public static string ThrowError(
        Exception error,
        string problem = "",
        string? name = null,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        string first;
        if (name == null)
            first = $"Problème : {problem} \n";
        else
            first = $"[{name}] A eu un problème : {problem} \n";

        return first +
                $"Précision : {error}\n" +
                $"A {memberName} dans {filePath} à la ligne : {lineNumber}\n";

    }

    public static T TryOrReturn<T>(Func<T> func, SafeWriter writer, T fallback, string context = "", string? name = null)
    {
        try { return func(); }
        catch (Exception e)
        {
            writer.Log(ThrowError(e, context, name));
            return fallback;
        }

    }
    public static T TryOrReturn<T>(Func<T> func, T fallback, string context = "", string? name = null)
    {
        return TryOrReturn<T>(func, DefaultWriter, fallback, context, name);
    }
    public static void TryOrPass(Action action, SafeWriter writer, string context = "", string? name = null)
    {
        try { action(); }
        catch (Exception e)
        {
            writer.Log(ThrowError(e, context, name));
            return;
        }
    }
    public static void TryOrPass(Action action, string context = "", string? name = null)
    {
        TryOrPass(action, DefaultWriter, context, name);
    }

    public static T TryOrPanic<T>(Func<T> func, string context = "", string? name = null)
    {
        try { return func(); }
        catch (Exception e)
        {
            throw new Exception(ThrowError(e, context, name), e);
        }

    }
    public static T TryOrStackTrace<T>(Func<T> func, SafeWriter writer, string context = "", string? name = null)
    {
        try { return func(); }
        catch (Exception e)
        {
            writer.Log(ThrowError(e, context, name));
            writer.Log("===== STACK TRACE ===== \n");

            StackTrace st = new StackTrace(true);
            string stackIndent = "";
            for (int i = 0; i < st.FrameCount; i++)
            {
                StackFrame sf = st.GetFrame(i);
                writer.Log("\n");
                writer.Log(stackIndent + $"Method: {sf.GetMethod()}" + "\n");
                writer.Log(stackIndent + $"File: {sf.GetFileName()}" + "\n");
                writer.Log(stackIndent + $"Line number: {sf.GetFileLineNumber()}" + "\n");
                stackIndent += "   ";
            }

            throw e;
        }
    }
    public static T TryOrStackTrace<T>(Func<T> func, string context = "", string? name = null)
    {
        return TryOrStackTrace<T>(func, DefaultWriter, context, name);
    }

}

public class SafeWriter
{
    public string Name { get; init; }
    public Action<string> LogMessage { get; init; }


    public SafeWriter(String name, Action<String> logAction)
    {
        Name = name;
        LogMessage = logAction;

    }

    public void Log(string message)
    {
        LogMessage?.Invoke($"{message}");    

    }

}
