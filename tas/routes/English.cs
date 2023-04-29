namespace Ident.TAS;
using System.Collections.Generic;
public partial class Routes {
public static readonly Dictionary<string, Dictionary<string, int>> English = new()
{
    {
        "a1_s1_exteriorIntro", new()
        {
            { "Are you ready to begin, program?", 0 },
            { "Suddenly, the screech of tires against asphalt to your left. The rhythmic pulse of a Light Cycle at full speed. You turn, seeing cold blue light racing towards you.", 0 },
            { "Bit of trouble on the road there chief?", 0 },
            { "Can I help you?", 0 },
            { "This isn't going to be an easy night.", 0 },
        }
    },
    {
        "a1_s2_a_lobbyFirstHangout", new()
        {
            { "You note that it is empty, quiet.", 0 },
            { "And the prisoner, in the Library.", 0 },
            { "It wasn't my call to bring you in. I had this handled. I was about to begin my investigation.", 0 },
            { "You've been told nothing?", 0 },
            { "But I respect the effort.", 0 },
            { "Yeah. a long way up. I've not been up there yet, but it was loud.", 0 },
            { "I... What?", 0 },
            { "Since the explosion, I've been missing... things.", 0 },
            { "You're DoT you said? You can defragment Identity Discs, can't you? Recover memories?", 0 },
        }
    },
    {
        "a1_s2_b_lobbyFirstHangoutPostGame", new()
        {
            { "I've unlocked repeat access rights for you. You'll be able to travel to any location Prinz allows.", 0 },
            { "I'm not going to let you do this without me.", 0 },
            { "I'm the one between you and the door, program.", 0 },
            { "You push forward. He's surprised.", 1 },
            { "Grish's eyes flare.", 0 },
        }
    },
    {
        "a1_s3_adminOfficeFirstMeeting", new()
        {
            { "Very glad you could join us, Query. You come very highly recommended.", 0 },
            { "A little showy perhaps. I've been told it looks like a shrine. But, much like the Users, I enjoy a little theater.", 0 },
            { "Tell me, Query, for whom do you fight?", 0 },
            { "He will return.", 0 },
            { "Grish saw many things in the Adjunct War. I think he carries more scars than the one we can see. It has dented his belief. He's not alone in that.", 0 },
            { "My interest in the Users goes beyond blind faith. I imagine you've noticed my collection.", 1 },
            { "I need to know who is responsible, and what they stole from us.", 0 },
            { "Give my apologies to Ada when you see her. Not exactly her role to play jailer.", 0 },
        }
    },
    {
        "a1_s4_firstVisitToLibrary", new()
        {
            { "You enjoy my garden?", 0 },
            { "I am Ada. Welcome to my garden.", 0 },
            { "Do you know what they are?", 0 },
            { "At multiple points in your investigation, you'll have the opportunity to dig deeper via investigation choices, marked with a magnifying glass. Take your time to ask as many as you like, then move on.", 3 },
            { "Under normal circumstances, this is a space of tranquility. The observation deck provides a place to meditate, to consider the knowledge collected here.", 0 },
            { "Ah, a student of Flynn?", 0 },
            { "Greetings, program.", 0 },
            { "Am I your first Automata, Query?", 0 },
            { "In truth, I am proud of the difference. We choose not to look like the Users of myth, but walk our own path.", 0 },
            { "A silence hangs in the air for a moment.", 0 },
        }
    },
    {
        "a1_s5_a_firstCassEncounter", new()
        {
            { "Best to approach this carefully.", 0 },
            { "A kid left to guard something worth destroying this Vault over.", 0 },
            { "Am I in trouble?", 0 },
        }
    },
    {
        "a1_s5_b_firstCassEncounterPostGame", new()
        {
            { "It's a start. You smile, and help Cass to their feet.", 0 },
            { "Like what?", 0 },
            { "You chuckle. You've never been asked that before. Saying DoT tends to make programs less talkative, if anything.", 0 },
            { "And you're here to solve the mystery of... me?", 0 },
            { "You turn back to them.", 0 },
        }
    },
    {
        "a1_s6_a_returnToAdmin", new()
        {
            { "Welcome back, program.", 0 },
            { "I fear things have continued to slide in your absence. I find myself increasingly forgetful. Some kind of lasting symptom of the explosion, you think?", 0 },
            { "How many programs function entirely in the dark? How many remain ignorant? So peaceful.", 0 },
            { "What do you mean?", 0 },
            { "You can help, program?", 0 },
        }
    },
    {
        "a1_s6_b_returnToAdminPostGame", new()
        {
            { "So. What is your conclusion in this case?", 0 },
            { "How long does this usually take?", 0 },
            { "I am of course comfortable to answer any questions you may have of me, or Grish here, before you resume your investigation. Ask as many as you like.", 3 },
            { "It would appear we have an issue.", 0 },
            { "My exterior guards tell me a known criminal has just landed on the building. Her name is Proxy, have you encountered her before?", 0 },
            { "When the time is right, I will return.", 0 },
        }
    },
    {
        "a2_s1_landingPadProxyArrives", new()
        {
            { "Do you have a name?", 1 },
            { "I'm Proxy. I'm sure you've already been told a great deal about me?", 0 },
            { "Before you ask me your questions and we get to negotiating what happens next, I have one pressing question, if I may?", 0 },
            { "Something happen here? Are the usual programs... occupied?", 0 },
            { "You're all they had to send up to me, no doubt tightening security around the zealot Prinz. How is Grish, by the way?", 0 },
            { "I'm here because I see an opportunity. An opportunity to take something of my own, while you're busy chasing around that other thief.", 0 },
            { "Ask your questions, then.", 3 },
            { "She reaches to her back, draws her Disc. It sparks and shivers with energy. You have no doubt she knows how to use it.", 0 },
            { "She jumps back, adopting a throwing stance. You mirror her.", 0 },
            { "This moment matters. A choice, either way.", 0 },
        }
    },
    {
        "a2_s2_a_returnToCass", new()
        {
            { "Hello Query.", 0 },
            { "The rain feels peculiar to me. It shouldn't. It's rain. Most common thing on The Grid, right?", 0 },
            { "It's cold out there.", 0 },
            { "How goes the investigation?", 0 },
            { "Whatever I was guarding, it's safe to assume I failed in my task. I can't let that happen again.", 0 },
            { "Sometimes though...", 0 },
            { "You nod. They remove their Disc and hand it to you. Their hands are shaking.", 0 },
        }
    },
    {
        "a2_s2_b_returnToCassPostGame", new()
        {
            { "Silence.", 0 },
            { "I don't know what was stolen, or who stole it or-", 0 },
            { "Did they...", 1 },
            { "New information. Not much, but something to build on.", 0 },
            { "You help Cass back to their feet.", 0 },
        }
    },
    {
        "a2_s3_sierras question", new()
        {
            { "Definitely uncomfortable.", 0 },
            { "Did she say what she was looking for?", 0 },
            { "Its... Ethical complexity.", 0 },
            { "I need to confess something to you.", 0 },
            { "I worry that I may have played a role in the theft, and now in Proxy's attempt.", 0 },
            { "Rumors of secrets still hidden here have been circulating. Building interest.", 0 },
            { "You let that hang in the air for a moment.", 0 },
            { "Query.", 0 },
            { "I am grateful that you came alone. Ask your questions.", 5 },
            { "I was in here. During the explosion I was here, and nobody has even thought to ask. Nobody thought that might be an important piece of information. Not even you.", 0 },
            { "Is everything alright?", 0 },
            { "I am done.", 0 },
            { "You consider his request.", 1 },
        }
    },
    {
        "a2_s4_a_lobbyGrishCheckIn", new()
        {
            { "I've been looking for you.", 0 },
            { "But first I need you to clean my Disc again. Still dropping memories. Prinz needs me at the top of my game, and I'm far from it.", 0 },
        }
    },
    {
        "a2_s4_b_lobbyGrishCheckInPostGame", new()
        {
            { "It's quiet for now. No sign of Proxy. None of my men have spotted her, and no call-ins. You wouldn't know anything about that, would you?", 0 },
            { "Did she say anything to you about her plans?", 0 },
            { "Ask your questions. Make it quick.", 6 },
        }
    },
    {
        "a2_s5_adminReportBack", new()
        {
            { "You look around. This is a good opportunity to explore Prinz's collection of User artifacts.", 5 },
        }
    },
    {
        "a2_s6_landingPadProxyOutcome", new()
        {
            { "So this is where our interloper made her move?", 0 },
            { "She ended her process in the way she ran, small and pathetically outmatched.", 0 },
            { "Are our operations synchronized?", 0 },
            { "Where are we on the case, program?", 0 },
            { "One more thing, Query.", 0 },
            { "Before we went to Grish's favorite little hidden space in the Repository, I asked him to escort me to visit your witness, Cass.", 0 },
            { "It was, however, enough time for me to make a little deduction of my own.", 0 },
        }
    },
    {
        "a2_s7_a_cassIsNotAGuard", new()
        {
            { "Strange, to be the one in need of comfort, but Cass has obviously noticed something in your demeanor.", 0 },
            { "You nod.", 0 },
            { "You hesitate.", 0 },
        }
    },
    {
        "a2_s7_b_cassIsNotAGuardPostGame", new()
        {
            { "You kneel down in front of them.", 0 },
            { "You saw something. It was hazy, uncertain. A silhouette.", 0 },
            { "I was locked in a box, for as long as I've been alive I've been locked in a box.", 0 },
            { "I've been a prisoner for so long. The guard in my memory, he gave me this uniform. Took pity on me.", 0 },
            { "Well, one kindness among many cruelties. He kept me locked up. Probably helped with my transfer here.", 0 },
            { "I'm the danger.", 0 },
            { "If you have questions, I can answer them, but then I think I need some space for a couple millicycles.", 4 },
        }
    },
    {
        "a2_s8_LibraryClosure", new()
        {
            { "You've had quite the evening.", 0 },
            { "It's been a long night.", 0 },
            { "You change the subject.", 0 },
            { "Why would Core do that? Maybe they're dangerous?", 0 },
            { "Ada nods, and walks away to investigate her trees.", 0 },
            { "I suppose the ambassadorship was a resounding success in that regard.", 0 },
        }
    },
    {
        "a2_s9_a_AnotherVaultExplosion", new()
        {
            { "Hello.", 0 },
            { "Before I got locked in a tower.", 0 },
        }
    },
    {
        "a2_s9_b_AnotherVaultExplosionPostGame", new()
        {
            { "You know what you both saw. Everything falling apart, The Grid itself disintegrating, derezzing into a million fractured pieces. A terrible future.", 0 },
            { "The end. Of everything. Collapse.", 0 },
            { "The Grid is ending. It can't be stopped. Each time I realize that, I...", 0 },
            { "Truth encoded.", 0 },
            { "Ask me your questions. Last chance.", 6 },
        }
    },
    {
        "a3_s1_alt1_admin", new()
        {
            { "Same goes for the program himself.", 0 },
            { "Blasphemy!", 0 },
            { "We cannot let doubt in. We cannot break from His patterns. He will return and He will help us to bring about a new age.", 0 },
            { "He will come back to us.", 0 },
            { "I'm sorry Query. It is hard to hear some of what you say. Terrifying.", 0 },
            { "At least you take each case on its own merits. You were wise to leave the Automata spy in our custody.", 0 },
            { "I know your vows. I know it can't have been easy for you to intervene.", 0 },
            { "I fear we are rapidly approaching a good bye.", 0 },
            { "You take a deep breath.", 0 },
            { "You'd inflict that upon The Grid?", 0 },
            { "I have no interest in presenting a barrier to what needs to happen next.", 0 },
        }
    },
    {
        "a3_s2_a_culminationLandingPad", new()
        {
            { "You smile.", 0 },
            { "You stand next to them. Look out at the city below. It's different than it was earlier. More fragile.", 0 },
            { "Are you here to help me?", 0 },
            { "This will be a little faster now, as you already know who Cass is, and the view seems to be anchoring them.", 0 },
        }
    },
    {
        "a3_s2_b_culminationLandingPadPostGame", new()
        {
            { "They begin crying.", 0 },
            { "You're definitely out of your depth.", 0 },
            { "They seem startled by it.", 0 },
            { "I'm worried there aren't many cycles left.", 0 },
            { "I need you to copy my observations, my knowledge of what's coming. I'm an unreliable vessel for it.", 0 },
        }
    },
    {
        "a3_s2_c_culminationLandingPadPostPostGame", new()
        {
            { "How does that feel?", 0 },
            { "You don't need to read the data. Let it sit within you until someone else needs it.", 0 },
            { "It isn't.", 0 },
            { "Cass comes with me, we're going to head back to the Vault. You'll be safe. More importantly, the rest of us will be too.", 2 },
            { "Really?", 1 },
        }
    },
    {
        "a3_s3_anExit", new()
        {
            { "You activate it, reading your data and thinking about what you saw tonight.", 3 },
        }
    },
};
}
