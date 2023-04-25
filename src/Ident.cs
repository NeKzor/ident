/*
 * Copyright (c) 2023, NeKz
 *
 * SPDX-License-Identifier: MIT
 * 
 * 
 * This algorithm tries to find all optimal choices for Tron: Identity.
 * 
 * Story files are written with the ink scripting language.
 *      https://www.inklestudios.com/ink
 * 
 * Ink runtime version 20 is needed which is release version 1.0.0.
 *      https://github.com/inkle/ink/releases/tag/v1.0.0
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Story = Ink.Runtime.Story;
using Choice = Ink.Runtime.Choice;
using static System.Diagnostics.Trace;

var start = DateTime.Now;
var rootFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "../../../../");

// Supported languages
var languages = new List<string>()
{
    "Chinese (Simplified)",
    "Chinese (Traditional)",
    "English",
    "French",
    "German",
    "Italian (Italy)",
    "Japanese",
    "Korean",
    "Spanish (Castilian)",
};

var defaultLanguage = args.ElementAtOrDefault(0) ?? "English";
var language = languages.Find((language) => language == defaultLanguage);

if (language == default)
{
    Console.WriteLine("Supported languages:");

    foreach (var supportedLanguage in languages)
        Console.WriteLine($"  - {supportedLanguage}");

    return;
}

var resultsFile = $"{rootFolder}results/{language}.txt";
if (File.Exists(resultsFile))
    File.Delete(resultsFile);

Listeners.Clear();
Listeners.Add(new ConsoleTraceListener());
Listeners.Add(new TextWriterTraceListener(resultsFile));
AutoFlush = true;

// Dumped level1 file with AssetRipper: https://github.com/AssetRipper/AssetRipper/releases
var level1TextAssets = $"{rootFolder}data/level1/ExportedProject/Assets/TextAsset/";

// These embedded files are JSON serialized ink stories.
// They are prefixed in the order you would have to progress the game.
// Defrag minigames are played before "PostGame" conversations.
// Conversations with "_x_" in their name do not contain any meaningful dialogue.
var conversations = new List<string>()
{
    "a1_s1_exteriorIntro",
    "a1_s2_a_lobbyFirstHangout",
    "a1_s2_b_lobbyFirstHangoutPostGame",
    "a1_s3_adminOfficeFirstMeeting",
    "a1_s4_firstVisitToLibrary",
    "a1_s5_a_firstCassEncounter",
    "a1_s5_b_firstCassEncounterPostGame",
    "a1_s6_a_returnToAdmin",
    "a1_s6_b_returnToAdminPostGame",
    //"a1_x_AdminReturnBeforeProxy",
    //"a1_x_AdminReturnTooEarly",
    //"a1_x_lobbyReturnKernelPresent",
    //"a1_x_lobbyReturnNobodyPresent",
    //"a1_x_ReturnToLibrary",
    //"a1_x_ReturnToVault",
    "a2_s1_landingPadProxyArrives",
    "a2_s2_a_returnToCass",
    "a2_s2_b_returnToCassPostGame",
    "a2_s3_sierras question", // Uhm, why is there a space? lol
    "a2_s4_a_lobbyGrishCheckIn",
    "a2_s4_b_lobbyGrishCheckInPostGame",
    "a2_s5_adminReportBack",
    "a2_s6_landingPadProxyOutcome",
    "a2_s7_a_cassIsNotAGuard",
    "a2_s7_b_cassIsNotAGuardPostGame",
    "a2_s8_LibraryClosure",
    "a2_s9_a_AnotherVaultExplosion",
    "a2_s9_b_AnotherVaultExplosionPostGame",
    //"a2_x_adminRetunAfterProxyOutcome", // Typo "Return"
    //"a2_x_landingPadEmpty",
    //"a2_x_libraryReturn",
    //"a2_x_lobbyReturn",
    //"a2_x_vaultReturn",
    "a3_s1_alt1_admin",
    "a3_s1_alt2_grishInLobby",
    "a3_s2_a_culminationLandingPad",
    "a3_s2_b_culminationLandingPadPostGame",
    "a3_s2_c_culminationLandingPadPostPostGame",
    "a3_s3_anExit",
    //"a3_x_lastAdminVisit",
    //"a3_x_lastLandingPadVisit",
    //"a3_x_lastLibraryVisit",
    //"a3_x_lastLobbyVisit",
    //"a3_x_lastVaultVisitCassPresent",
    //"a3_x_lastVaultVisit",
};

// Calculating the shortest path is cheap if we ignore the context of the whole story.
// However, it gets more complicated to add choices from previous conversations as context.
// This means we have to save all consequences after one story ends and then set them again
// once the next one starts.
var globals = new Dictionary<string, string>();

void InitConsequences(Story story)
{
    foreach (var global in globals)
    {
        if (story.variablesState.GlobalVariableExistsWithName(global.Key))
            story.variablesState.SetGlobal(global.Key, Ink.Runtime.Value.Create(global.Value));
    }
}

var totalKeyPresses = 0;

foreach (var conversation in conversations)
{
    var file = $"{level1TextAssets}{conversation}_{language}.json";
    var filename = Path.GetFileName(file);

    WriteLine($"Processing {conversation}");

    var story = new Story(File.ReadAllText(file));
    InitConsequences(story);

    var path = new Node()
    {
        Id = story.currentText,
        MaxChoices = story.currentChoices.Count
    };

    // Keep track of all possible story lines.
    var paths = new List<Node>() { path };

    var bestScore = int.MaxValue;
    var totalPossiblePaths = 0;

    // Only keep the shortest paths that lead to the least amount of keypresses
    void RemovePathIfNotGoodEnough()
    {
        totalPossiblePaths += 1;
        var newPaths = new List<Node>();

        foreach (var path in paths)
        {
            var score = path.Score;
            if (score > bestScore)
                continue;

            newPaths.Add(path);
            bestScore = score;
        }

        paths = newPaths;
    }

    // Stories begins here.
    // Choices have to be made before we can continue a story.
    // A story ends if there is nothing left to continue.
    while (true)
    {
        if (path.HasChoices)
        {
            if (path.HasVisitedAllChoices)
            {
                path = path.PreviousChoice;
                paths.Last().SaveVariables(story);

                if (path?.HasChoices ?? false)
                {
                    // Reset story to previous choice.
                    var rewinded = Rewind(story, path);
                    path = rewinded.Path;

                    if (!path.HasVisitedAllChoices)
                    {
                        totalPossiblePaths += 1;
                        paths.Add(rewinded.Head);
                    }
                }
                else
                    break;
            }

            // Make a choice. Keep track of visited choices within a story line.
            foreach (var choice in story.currentChoices)
            {
                // Ignore visited choices.
                if (path.VisitedChoices.Contains(choice.index))
                    continue;

                // Ignore loop choices and mark them as visited.
                if (choice.text.StartsWith("-loops"))
                {
                    path.VisitedChoices.Add(choice.index);
                    continue;
                }

                story.ChooseChoiceIndex(choice.index);

                path.VisitedChoices.Add(choice.index);
                path = path.Progress(choice);
                break;
            }
        }

        if (story.canContinue)
        {
            story.Continue();
            path = path.Progress(story);
            continue;
        }

        path = path.PreviousChoice;
        paths.Last().SaveVariables(story);

        if (path?.HasChoices ?? false)
        {
            // Reset story to previous choice.
            var rewinded = Rewind(story, path);
            path = rewinded.Path;

            if (!path.HasVisitedAllChoices)
            {
                RemovePathIfNotGoodEnough();
                paths.Add(rewinded.Head);
            }

            if (story.canContinue)
                story.Continue();

            continue;
        }

        break;
    }

    RemovePathIfNotGoodEnough();

    if (paths.Count((path) => path.GlobalVariables.Count > 0) != paths.Count)
        throw new Exception("failed to save global variables");

    if (paths.Any((path) => path.Score != bestScore))
        throw new Exception("failed to filter best paths");

    WriteLine($"Total permutations: {totalPossiblePaths}");
    WriteLine($"Best outcome:       {bestScore} keypresses");
    WriteLine($"Best permutations:  {paths.Count}");

    totalKeyPresses += bestScore;

    var bestPath = default(Node);

    // Choose a better outcome determined by manual observation
    if (conversation == "a1_s2_b_lobbyFirstHangoutPostGame")
    {
        if (paths.Count != 2)
            throw new Exception($"outcome of {conversation} would not match previous observation");

        bestPath = paths.Find((best) => best.GlobalVariables["_askedKernelToHelp"] == "0");
    }
    //else if (conversation == "a1_s4_firstVisitToLibrary")
    //{
    //    if (paths.Count != 2)
    //        throw new Exception($"outcome of {conversation} would not match previous observation");

    //    bestPath = paths.Find((best) => best.GlobalVariables["_SierraOpinionTracker"] == "0");
    //}
    //else if (conversation == "a1_s6_b_returnToAdminPostGame")
    //{
    //    if (paths.Count != 2)
    //        throw new Exception($"outcome of {conversation} would not match previous observation");

    //    bestPath = paths.Find((best) => best.GlobalVariables["_accusedToAdmin"] == "3");
    //}
    else
    {
        bestPath = paths.First();
    }

    WriteLine("Globals:");
    foreach (var variable in bestPath.GlobalVariables)
        WriteLine($"{variable.Key} = {variable.Value}");

    WriteLine("Example:");
    var current = bestPath;
    while (current != default)
    {
        if (current.Previous?.HasChoices ?? false)
            WriteLine($"[{current.Previous.VisitedChoices.Count - 1}] {current.Id} ");
        current = current.Next;
    }

    WriteLine("");

    // Save consequences for next conversation
    globals = bestPath.GlobalVariables;

    //var tempGlobal = new Dictionary<string, string>(best.Paths.First().GlobalVariables);
    //foreach (var bestPath in best.Paths)
    //{
    //    foreach (var global in bestPath.GlobalVariables)
    //    {
    //        if (tempGlobal.TryGetValue(global.Key, out var value))
    //        {
    //            if (value != global.Value)
    //                WriteLine($"different global {global.Key} = {value} -> {global.Value}");
    //        }
    //        else
    //            tempGlobal[global.Key] = global.Value;
    //    }
    //}
    //WriteLine();
}

WriteLine($"Keypresses: {totalKeyPresses}");
WriteLine($"Calculation took {(DateTime.Now - start).TotalMinutes:F2} minutes");

// The ink framework does not have an undo feature.
// This means we have to reset the story, go back to the head node and
// replay everything again within the current story line.
(Node Head, Node Path) Rewind(Story story, Node path)
{
    // Save original ID for safety check at the end.
    var originalId = path.Id;

    // Temporarily unlink next node as we only want to clone
    // everything up until this point.
    var tempPath = path.Next;
    path.Next = default;

    // Clone and reset link of old path.
    var clonedHead = path.Head.Clone(default);
    path.Next = tempPath;

    // Reset and replay story.
    story.ResetState();
    InitConsequences(story);

    path = clonedHead;

    while (true)
    {
        if (story.currentChoices.Any())
        {
            if (path.Next != default)
            {
                var choice = path.VisitedChoices.ElementAt(path.VisitedChoices.Count - 1);
                story.ChooseChoiceIndex(choice);
                path = path.Next;
            }
            else
                break;
        }

        if (story.canContinue)
        {
            story.Continue();

            if (path.Next != default)
            {
                path = path.Next;
                continue;
            }
        }

        break;
    }

    if (path.Id != originalId)
        throw new Exception("invalid story");

    return (clonedHead, path);
}

// A node can be seen as a progress/choose keypress.
// It's either a choice (MaxChoices > 0) or just story text (MaxChoices = 0).
// Nodes within a story are connected via a doubly-linked list.
[DebuggerDisplay("[{Index}] (choices = {MaxChoices}) {Id}")]
class Node
{
    // Story text. Can be a choice or part of a conversation.
    public string Id { get; init; }

    // Number of choices for this node.
    public int MaxChoices { get; init; }

    // Keep track of previously visited choices by their index number.
    public List<int> VisitedChoices { get; init; } = new();

    // Keep track of global variables once the story ends.
    public Dictionary<string, string> GlobalVariables { get; init; } = new();

    // Is this a choice?
    public bool HasChoices { get { return MaxChoices > 0; } }

    // Did we go through all choices?
    public bool HasVisitedAllChoices { get { return VisitedChoices.Count == MaxChoices; } }

    // Index within the story line.
    public int Index
    {
        get
        {
            var index = 0;
            var current = Previous;
            while (current != default)
            {
                index += 1;
                current = current.Previous;
            }
            return index;
        }
    }

    // Link to previous node. Node is the head if this is null.
    public Node Previous { get; set; } = default;

    // Link to next node. Node is the tail if this is null.
    public Node Next { get; set; } = default;

    // Head node of current story line.
    public Node Head
    {
        get
        {
            var head = this;
            while (head.Previous != default)
                head = head.Previous;
            return head;
        }
    }

    // Get previous choice of current story line.
    public Node PreviousChoice
    {
        get
        {
            var previous = Previous;
            while (previous != default && !previous.HasChoices)
                previous = previous.Previous;
            return previous;
        }
    }

    // Get the amount of keypresses of current story line.
    public int Score
    {
        get
        {
            var count = 0;
            var current = this;
            while (current != default)
            {
                count += current.Previous?.HasChoices ?? false
                    ? current.Previous.VisitedChoices.Count
                    : 1;
                current = current.Next;
            }
            return count;
        }
    }

    public Node Progress(Story story)
    {
        Next = new Node()
        {
            Id = story.currentText,
            MaxChoices = story.currentChoices.Count,
            Previous = this,
        };
        return Next;
    }

    public Node Progress(Choice choice)
    {
        Next = new Node()
        {
            Id = "* " + choice.text,
            Previous = this,
        };
        return Next;
    }

    public Node Clone(Node previous)
    {
        var cloned = new Node()
        {
            Id = Id,
            MaxChoices = MaxChoices,
            VisitedChoices = new List<int>(VisitedChoices),
            Previous = previous,
        };
        cloned.Next = Next?.Clone(cloned);
        return cloned;
    }

    public void SaveVariables(Story story)
    {
        foreach (var variableName in story.variablesState)
        {
            var variable = story.variablesState.GetVariableWithName(variableName);
            GlobalVariables[variableName] = variable.ToString();
        }
    }
}

// Needed for "init" keyword because older .NET frameworks are not compatible with
// newer language features. Thanks Microsoft :>
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
