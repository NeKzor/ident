namespace Ident.TAS;
using System.Collections.Generic;
public partial class Routes {
public static readonly Dictionary<string, Dictionary<string, (int, string)>> English = new()
{
    {
        "a1_s1_exteriorIntro", new()
        {
            { "Are you ready to begin, program?", (0, "Yes") },
            { "Suddenly, the screech of tires against asphalt to your left. The rhythmic pulse of a Light Cycle at full speed. You turn, seeing cold blue light racing towards you.", (0, "Hold your ground.") },
            { "Bit of trouble on the road there chief?", (0, "Nothing I can't handle.") },
            { "Can I help you?", (0, "Greetings, program.") },
            { "This isn't going to be an easy night.", (0, "This'll be easier if you work with me.") },
        }
    },
    {
        "a1_s2_a_lobbyFirstHangout", new()
        {
            { "You note that it is empty, quiet.", (0, "Where is everyone?") },
            { "And the prisoner, in the Library.", (0, "Prisoner?") },
            { "It wasn't my call to bring you in. I had this handled. I was about to begin my investigation.", (0, "Investigating what?") },
            { "You've been told nothing?", (0, "I get the call, I run, program.") },
            { "But I respect the effort.", (0, "An explosion?") },
            { "Yeah. a long way up. I've not been up there yet, but it was loud.", (0, "How tall is this building?") },
            { "I... What?", (0, "Is there a problem?") },
            { "Since the explosion, I've been missing... things.", (0, "Memories?") },
            { "You're DoT you said? You can defragment Identity Discs, can't you? Recover memories?", (0, "I can.") },
        }
    },
    {
        "a1_s2_b_lobbyFirstHangoutPostGame", new()
        {
            { "I've unlocked repeat access rights for you. You'll be able to travel to any location Prinz allows.", (0, "Thank you.") },
            { "I'm not going to let you do this without me.", (0, "That's not your call to make.") },
            { "I'm the one between you and the door, program.", (0, "You'd defy Prinz?") },
            { "You push forward. He's surprised.", (1, "I work alone.") },
            { "Grish's eyes flare.", (0, "I'm sorry.") },
        }
    },
    {
        "a1_s3_adminOfficeFirstMeeting", new()
        {
            { "Very glad you could join us, Query. You come very highly recommended.", (0, "Reputation is everything.") },
            { "A little showy perhaps. I've been told it looks like a shrine. But, much like the Users, I enjoy a little theater.", (0, "You didn't hire me to talk about desks.") },
            { "Tell me, Query, for whom do you fight?", (0, "I fight for the Users.") },
            { "He will return.", (0, "We owe Him everything.") },
            { "Grish saw many things in the Adjunct War. I think he carries more scars than the one we can see. It has dented his belief. He's not alone in that.", (0, "You can't save everyone.") },
            { "My interest in the Users goes beyond blind faith. I imagine you've noticed my collection.", (1, "Can we get to the case, please?") },
            { "I need to know who is responsible, and what they stole from us.", (0, "What was in the Vault?") },
            { "Give my apologies to Ada when you see her. Not exactly her role to play jailer.", (0, "I'll start with the Vault.") },
        }
    },
    {
        "a1_s4_firstVisitToLibrary", new()
        {
            { "You enjoy my garden?", (0, "I've never seen anything like this before.") },
            { "I am Ada. Welcome to my garden.", (0, "I don't understand.") },
            { "Do you know what they are?", (0, "Is that a trick question?") },
            { "At multiple points in your investigation, you'll have the opportunity to dig deeper via investigation choices, marked with a magnifying glass. Take your time to ask as many as you like, then move on.", (3, "Let's move on.") },
            { "Under normal circumstances, this is a space of tranquility. The observation deck provides a place to meditate, to consider the knowledge collected here.", (0, "To knock on the sky, listen to the sound?") },
            { "Ah, a student of Flynn?", (0, "I read.") },
            { "Greetings, program.", (0, "Greetings.") },
            { "Am I your first Automata, Query?", (0, "Yes. I'm sorry for staring.") },
            { "In truth, I am proud of the difference. We choose not to look like the Users of myth, but walk our own path.", (0, "You look... User-ish?") },
            { "A silence hangs in the air for a moment.", (0, "I don't think you did anything.") },
        }
    },
    {
        "a1_s5_a_firstCassEncounter", new()
        {
            { "Best to approach this carefully.", (0, "Hey, what's your name?") },
            { "A kid left to guard something worth destroying this Vault over.", (0, "You're confused?") },
            { "Am I in trouble?", (0, "Not at all.") },
        }
    },
    {
        "a1_s5_b_firstCassEncounterPostGame", new()
        {
            { "It's a start. You smile, and help Cass to their feet.", (0, "What do you remember?") },
            { "Like what?", (0, "A program? Anyone else here before the explosion?") },
            { "You chuckle. You've never been asked that before. Saying DoT tends to make programs less talkative, if anything.", (0, "I'm a detective, I solve mysteries.") },
            { "And you're here to solve the mystery of... me?", (0, "Well, of the explosion.") },
            { "You turn back to them.", (0, "I'll be back soon.") },
        }
    },
    {
        "a1_s6_a_returnToAdmin", new()
        {
            { "Welcome back, program.", (0, "It's good to be back.") },
            { "I fear things have continued to slide in your absence. I find myself increasingly forgetful. Some kind of lasting symptom of the explosion, you think?", (0, "That seems to be the case.") },
            { "How many programs function entirely in the dark? How many remain ignorant? So peaceful.", (0, "Isn't this place built to keep them ignorant?") },
            { "What do you mean?", (0, "I spoke to the Librarian.") },
            { "You can help, program?", (0, "I can.") },
        }
    },
    {
        "a1_s6_b_returnToAdminPostGame", new()
        {
            { "So. What is your conclusion in this case?", (0, "It's too early to say.") },
            { "How long does this usually take?", (0, "As long as it takes.") },
            { "I am of course comfortable to answer any questions you may have of me, or Grish here, before you resume your investigation. Ask as many as you like.", (3, "No more questions.") },
            { "It would appear we have an issue.", (0, "Yes.") },
            { "My exterior guards tell me a known criminal has just landed on the building. Her name is Proxy, have you encountered her before?", (0, "The name rings a bell.") },
            { "When the time is right, I will return.", (0, "Alright.") },
        }
    },
    {
        "a2_s1_landingPadProxyArrives", new()
        {
            { "Do you have a name?", (1, "I don't give my personal identifier to criminals while on a case.") },
            { "I'm Proxy. I'm sure you've already been told a great deal about me?", (0, "I know you're a thief.") },
            { "Before you ask me your questions and we get to negotiating what happens next, I have one pressing question, if I may?", (0, "Shoot.") },
            { "Something happen here? Are the usual programs... occupied?", (0, "You know what happened.") },
            { "You're all they had to send up to me, no doubt tightening security around the zealot Prinz. How is Grish, by the way?", (0, "He's been helping my investigation.") },
            { "I'm here because I see an opportunity. An opportunity to take something of my own, while you're busy chasing around that other thief.", (0, "Why tell me?") },
            { "Ask your questions, then.", (3, "Let's move on.") },
            { "She reaches to her back, draws her Disc. It sparks and shivers with energy. You have no doubt she knows how to use it.", (0, "I can't do that.") },
            { "She jumps back, adopting a throwing stance. You mirror her.", (0, "You're not going in there.") },
            { "This moment matters. A choice, either way.", (0, "Throw your Disc at Proxy.") },
        }
    },
    {
        "a2_s2_a_returnToCass", new()
        {
            { "Hello Query.", (0, "You OK?") },
            { "The rain feels peculiar to me. It shouldn't. It's rain. Most common thing on The Grid, right?", (0, "Yeah.") },
            { "It's cold out there.", (0, "It is.") },
            { "How goes the investigation?", (0, "There's a lot of power at play.") },
            { "Whatever I was guarding, it's safe to assume I failed in my task. I can't let that happen again.", (0, "Meaning...") },
            { "Sometimes though...", (0, "We need to find out what was stolen.") },
            { "You nod. They remove their Disc and hand it to you. Their hands are shaking.", (0, "Are you scared?") },
        }
    },
    {
        "a2_s2_b_returnToCassPostGame", new()
        {
            { "Silence.", (0, "So?") },
            { "I don't know what was stolen, or who stole it or-", (0, "It's OK.") },
            { "Did they...", (1, "You were alone?") },
            { "New information. Not much, but something to build on.", (0, "The explosion was triggered remotely?") },
            { "You help Cass back to their feet.", (0, "We are making progress.") },
        }
    },
    {
        "a2_s3_sierras question", new()
        {
            { "Definitely uncomfortable.", (0, "What's wrong?") },
            { "Did she say what she was looking for?", (0, "Some kind of super powered modification.") },
            { "Its... Ethical complexity.", (0, "I have nothing but respect for you.") },
            { "I need to confess something to you.", (0, "OK.") },
            { "I worry that I may have played a role in the theft, and now in Proxy's attempt.", (0, "You're working with someone on the outside?") },
            { "Rumors of secrets still hidden here have been circulating. Building interest.", (0, "You think they stole some data from the Vault?") },
            { "You let that hang in the air for a moment.", (0, "You're not responsible for the explosion.") },
            { "Query.", (0, "Greetings program.") },
            { "I am grateful that you came alone. Ask your questions.", (5, "No more questions.") },
            { "I was in here. During the explosion I was here, and nobody has even thought to ask. Nobody thought that might be an important piece of information. Not even you.", (0, "Why didn't Ada already tell me that you had an alibi?") },
            { "Is everything alright?", (0, "Why didn't you tell me Sierra was here during the explosion?") },
            { "I am done.", (0, "I heard you.") },
            { "You consider his request.", (1, "I will have more questions. You're staying, Sierra.") },
        }
    },
    {
        "a2_s4_a_lobbyGrishCheckIn", new()
        {
            { "I've been looking for you.", (0, "OK.") },
            { "But first I need you to clean my Disc again. Still dropping memories. Prinz needs me at the top of my game, and I'm far from it.", (0, "I can't keep doing this for you.") },
        }
    },
    {
        "a2_s4_b_lobbyGrishCheckInPostGame", new()
        {
            { "It's quiet for now. No sign of Proxy. None of my men have spotted her, and no call-ins. You wouldn't know anything about that, would you?", (0, "I've no idea.") },
            { "Did she say anything to you about her plans?", (0, "Nothing.") },
            { "Ask your questions. Make it quick.", (5, "No more questions.") },
        }
    },
    {
        "a2_s5_adminReportBack", new()
        {
            { "You look around. This is a good opportunity to explore Prinz's collection of User artifacts.", (5, "Leave.") },
        }
    },
    {
        "a2_s6_landingPadProxyOutcome", new()
        {
            { "So this is where our interloper made her move?", (0, "That's right, right where you're standing.") },
            { "She ended her process in the way she ran, small and pathetically outmatched.", (0, "I liked her.") },
            { "Are our operations synchronized?", (0, "They are.") },
            { "Where are we on the case, program?", (0, "Cass was alone when the explosion happened.") },
            { "One more thing, Query.", (0, "Go on.") },
            { "Before we went to Grish's favorite little hidden space in the Repository, I asked him to escort me to visit your witness, Cass.", (0, "I know.") },
            { "It was, however, enough time for me to make a little deduction of my own.", (0, "Yes?") },
        }
    },
    {
        "a2_s7_a_cassIsNotAGuard", new()
        {
            { "Strange, to be the one in need of comfort, but Cass has obviously noticed something in your demeanor.", (0, "A lot's happening.") },
            { "You nod.", (0, "Remember anything else?") },
            { "You hesitate.", (0, "You don't need to do that if you don't want to.") },
        }
    },
    {
        "a2_s7_b_cassIsNotAGuardPostGame", new()
        {
            { "You kneel down in front of them.", (0, "It's OK.") },
            { "You saw something. It was hazy, uncertain. A silhouette.", (0, "The thief?") },
            { "I was locked in a box, for as long as I've been alive I've been locked in a box.", (0, "So how did you get here?") },
            { "I've been a prisoner for so long. The guard in my memory, he gave me this uniform. Took pity on me.", (0, "You said it was cold.") },
            { "Well, one kindness among many cruelties. He kept me locked up. Probably helped with my transfer here.", (0, "Why were you transferred?") },
            { "I'm the danger.", (0, "Explosive? How do you survive that?") },
            { "If you have questions, I can answer them, but then I think I need some space for a couple millicycles.", (4, "No more questions, rest.") },
        }
    },
    {
        "a2_s8_LibraryClosure", new()
        {
            { "You've had quite the evening.", (0, "I'll remember it for a long time.") },
            { "It's been a long night.", (0, "How are you?") },
            { "You change the subject.", (0, "Cass is also a prisoner.") },
            { "Why would Core do that? Maybe they're dangerous?", (0, "Well, the explosions...") },
            { "Ada nods, and walks away to investigate her trees.", (0, "I do not interfere.") },
            { "I suppose the ambassadorship was a resounding success in that regard.", (0, "I don't think there's anything left to say.") },
        }
    },
    {
        "a2_s9_a_AnotherVaultExplosion", new()
        {
            { "Hello.", (0, "How are you feeling, Cass?") },
            { "Before I got locked in a tower.", (0, "Still no memory of why?") },
        }
    },
    {
        "a2_s9_b_AnotherVaultExplosionPostGame", new()
        {
            { "You know what you both saw. Everything falling apart, The Grid itself disintegrating, derezzing into a million fractured pieces. A terrible future.", (0, "What was that?") },
            { "The end. Of everything. Collapse.", (0, "It didn't happen.") },
            { "The Grid is ending. It can't be stopped. Each time I realize that, I...", (0, "How do you know this is real?") },
            { "Truth encoded.", (0, "The Grid is ending.") },
            { "Ask me your questions. Last chance.", (6, "I have nothing more to ask.") },
        }
    },
    {
        "a3_s1_alt1_admin", new()
        {
            { "Same goes for the program himself.", (0, "Do you know what Cass knows?") },
            { "Blasphemy!", (0, "How is that Blasphemy?") },
            { "We cannot let doubt in. We cannot break from His patterns. He will return and He will help us to bring about a new age.", (0, "It's been well over a thousand cycles, Prinz.") },
            { "He will come back to us.", (0, "This world was never meant to last forever.") },
            { "I'm sorry Query. It is hard to hear some of what you say. Terrifying.", (0, "We have to tell everyone.") },
            { "At least you take each case on its own merits. You were wise to leave the Automata spy in our custody.", (0, "That case isn't closed.") },
            { "I know your vows. I know it can't have been easy for you to intervene.", (0, "My choices were logical.") },
            { "I fear we are rapidly approaching a good bye.", (0, "I need your help.") },
            { "You take a deep breath.", (0, "I'm going to free Cass.") },
            { "You'd inflict that upon The Grid?", (0, "We can learn how to control it.") },
            { "I have no interest in presenting a barrier to what needs to happen next.", (0, "You'll let me take Cass from this place?") },
        }
    },
    {
        "a3_s2_a_culminationLandingPad", new()
        {
            { "You smile.", (0, "Hello Cass.") },
            { "You stand next to them. Look out at the city below. It's different than it was earlier. More fragile.", (0, "My name's Query.") },
            { "Are you here to help me?", (0, "I'm here to help The Grid.") },
            { "This will be a little faster now, as you already know who Cass is, and the view seems to be anchoring them.", (0, "Are you sure you want to know?") },
        }
    },
    {
        "a3_s2_b_culminationLandingPadPostGame", new()
        {
            { "They begin crying.", (0, "Console them.") },
            { "You're definitely out of your depth.", (0, "What do you want to do?") },
            { "They seem startled by it.", (0, "Another centicycle is beginning.") },
            { "I'm worried there aren't many cycles left.", (0, "You said.") },
            { "I need you to copy my observations, my knowledge of what's coming. I'm an unreliable vessel for it.", (0, "I can keep you safe.") },
        }
    },
    {
        "a3_s2_c_culminationLandingPadPostPostGame", new()
        {
            { "How does that feel?", (0, "Overwhelming.") },
            { "You don't need to read the data. Let it sit within you until someone else needs it.", (0, "You act like you're not going to make it.") },
            { "It isn't.", (0, "What's wrong Grish?") },
            { "Cass comes with me, we're going to head back to the Vault. You'll be safe. More importantly, the rest of us will be too.", (2, "Alright.") },
            { "Really?", (1, "I'm sorry, Cass.") },
        }
    },
    {
        "a3_s3_anExit", new()
        {
            { "You activate it, reading your data and thinking about what you saw tonight.", (3, "You're ready to leave.") },
        }
    },
};
}
