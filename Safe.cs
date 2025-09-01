using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Caffey.Utils;

//TODO: Add TryOrFalse => Return bool si raté;

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
        bool silent = false,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        string first;
        if (name == null)
            first = $"Problème : {problem} \n";
        else
            first = $"[{name}] A eu un problème : {problem} \n";
        string second;

        if (silent)
            second = $"Précision : {error.Message}\n";
        else
            second = $"Précision : {error}\n";

            return  first +
                    second +
                    $"A {memberName} dans {filePath} à la ligne : {lineNumber}\n";
    }
    public static string ThrowWithoutError(
        string problem = "",
        string? name = null,
        Exception? error = null,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        string first;
        if (name == null)
            first = $"Problème : {problem} \n";
        else
            first = $"[{name}] A eu un problème : {problem} \n";

        string second = "";
        if (error == null)
            second = "";
        else
            second = $"Précision : {error}\n";

        return first +
                second +
                $"A {memberName} dans {filePath} à la ligne : {lineNumber}\n";
    }

/// <summary>
/// Retourne sans crash le programme si le programme ne fonctionne pas.
/// Utilisé avec TryOrReturn(() => Func(), fallback)
/// Le reste des paramètres n'est pas obligatoire.
/// </summary>
/// <typeparam name="T">Le type de retour.</typeparam>
/// <param name="func">La fonction à utiliser.</param>
/// <param name="writer">Le SafeWriter, voir SafeWriter</param>
/// <param name="fallback">Le paramètre de fallback à choisir.</param>
/// <param name="context">La ligne affichée dans le log</param>
/// <param name="name">Le nom du script ou de la fonction [Name] dans le log</param>
/// <param name="silent">True, sans stacktrace.</param>
/// <param name="memberName">Automatique</param>
/// <param name="filePath">Automatique</param>
/// <param name="lineNumber">Automatique</param>
/// <returns></returns>
    public static T TryOrReturn<T>(Func<T> func,
            SafeWriter writer,
            T fallback,
            string context = "",
            string? name = null,
            bool silent = false,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
    {
        
        try { return func(); }
        catch (Exception e)
        {
            writer.Log(ThrowError(e, context, name, silent, memberName, filePath, lineNumber));
            
            return fallback;
        }
    }
    public static T TryOrReturn<T>(Func<T> func,
            T fallback,
            string context = "",
            string? name = null,
            bool silent = false,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
    {
        return TryOrReturn<T>(func, DefaultWriter, fallback, context, name, silent, memberName, filePath, lineNumber);
    }
    public static void TryOrPass(Action action,
            SafeWriter writer,
            string context = "",
            string? name = null,
            bool silent = false,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
    {
        try { action(); }
        catch (Exception e)
        {
            writer.Log(ThrowError(e, context, name, silent, memberName, filePath, lineNumber));
            return;
        }
    }
    public static void TryOrPass(Action action,
            string context = "",
            string? name = null,
            bool silent = false,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
    {
        TryOrPass(action, DefaultWriter, context, name, silent, memberName, filePath, lineNumber);
    }
    public static bool TryOrFalse(Action action,
            SafeWriter writer,
            string context = "",
            string? name = null,
            bool silent = false,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
    {
        try
        {
            action();
            return true;
        }
        catch (Exception e)
        {
            writer.Log(ThrowError(e, context, name, silent, memberName, filePath, lineNumber));
            return false;
        }
    }
    public static bool TryOrFalse(Action action,
            string context = "",
            string? name = null,
            bool silent = false,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)

    {
        return TryOrFalse(action, DefaultWriter, context, name, silent, memberName, filePath, lineNumber);
    }

    public static void TryOrPanicNull(Action action,
            string context = "",
            string? name = null,
            bool silent = false,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)

    {
        try { action(); }
        catch (Exception e)
        {
            throw new Exception(ThrowError(e, context, name, silent, memberName, filePath, lineNumber), e);
        }
    }

    public static T TryOrPanic<T>(Func<T> func,
            string context = "",
            string? name = null,
            bool silent = false, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
    {
        try { return func(); }
        catch (Exception e)
        {
            throw new Exception(ThrowError(e, context, name, silent, memberName, filePath, lineNumber), e);
        }
    }

    /// <summary>
    /// Utiliser la variable Silent, sinon, ceci est plus jolie.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="func"></param>
    /// <param name="writer"></param>
    /// <param name="context"></param>
    /// <param name="name"></param>
    /// <param name="silent"></param>
    /// <returns></returns>
    public static T TryOrStackTrace<T>(Func<T> func,
            SafeWriter writer,
            string context = "",
            string? name = null,
            bool silent = false,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
    {
        try { return func(); }
        catch (Exception e)
        {
            writer.Log(ThrowError(e, context, name, silent, memberName, filePath, lineNumber));
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
    public static T TryOrStackTrace<T>(Func<T> func,
            string context = "",
            string? name = null,
            bool silent = false,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
    {
        return TryOrStackTrace<T>(func, DefaultWriter, context, name, silent, memberName, filePath,lineNumber);
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
