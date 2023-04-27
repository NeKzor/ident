
//var gm = Ident.GameManager.Instance;
var gi = Ident.GameInput.Instance;
var defragGame = gi.Minigame;

// TODO: check for m_isPlaying

var state = new UnityEngine.InputSystem.LowLevel.KeyboardState();

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
