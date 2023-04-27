//var gm = Ident.GameManager.Instance;
var gi = Ident.GameInput.Instance;
var defragGame = gi.Minigame;

// TODO: check for m_isPlaying

var state = new UnityEngine.InputSystem.LowLevel.KeyboardState();

// TODO: What happens if we solve the puzzle during animation? Maybe with RNG manipulation?
if (!defragGame.menuObjectLink.paused && defragGame.m_playable && defragGame.isFullyVisible)
{
    // NOTE: Pause will be buffered until the animator finished running
    state.Press(UnityEngine.InputSystem.Key.Escape); // Pause
}

if (defragGame.menuObjectLink.paused)
{
    state.Press(UnityEngine.InputSystem.Key.DownArrow); // Down
    state.Press(UnityEngine.InputSystem.Key.DownArrow); // Down
    state.Press(UnityEngine.InputSystem.Key.Enter); // Skip
}

switch (gi.state)
{
    case DialogueManager.State.Dialogue:
        state.Press(UnityEngine.InputSystem.Key.P); // Continue
        break;
    case DialogueManager.State.Choice:
        // TODO: Insert menu down key presses based on gi.m_story.currentText
        state.Press(UnityEngine.InputSystem.Key.Space); // Choose
        break;
    case DialogueManager.State.NonChoiceButton:
        state.Press(UnityEngine.InputSystem.Key.Space); // Defrag/Stitch/End
        break;
    default:
        break;
}

var keyboard = UnityEngine.InputSystem.InputSystem.GetDevice<UnityEngine.InputSystem.Keyboard>();
UnityEngine.InputSystem.InputSystem.QueueStateEvent(keyboard, state);
