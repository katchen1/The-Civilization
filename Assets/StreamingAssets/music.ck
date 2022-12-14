// number of steps (MUST match the Unity NUM_STEPS!)
48 => int NUM_STEPS;

// notes
string notes[NUM_STEPS];
[
    "A", "E", " ", "A", "D", " ", "A", "C", " ", "B", "A", "g", // Verse 1
    "A", "E", " ", "A", "D", " ", "A", "C", " ", "B", "C", "D", // Verse 2
    "A", "E", " ", "A", "D", " ", "A", "C", " ", "B", "A", "g", // Verse 3
    "A", "e", " ", "A", "d", " ", "A", "c", " ", "A", " ", " "  // Verse 4
] @=> notes;

// global (outgoing) variables for Unity animation
global int currentStep; // which step we are at

// global (incoming) data from Unity
global string editAttribute;
global int editOn; // 1 - activate, 0 - deactivate, 2 - chicken noise
global Event editHappened;

// a sequence
float currRate; // default 1
float currGain; // default 1
dur currBeat; // beat (smallest division; dur between steps)
int currChord; // 0 = single note, 1 = double note

// initialize
1 => currRate;
0.5 => currGain;
600::ms => currBeat;
0 => currChord;

// sound buffers
SndBuf bufs[NUM_STEPS];
SndBuf bufs2[NUM_STEPS];
SndBuf chickenBuf;
// reverb
NRev reverb => dac;
// reverb mix
.1 => reverb.mix;

// connect them
for( int i; i < NUM_STEPS; i++ )
{
    // connect to dac
    bufs[i] => reverb;
    bufs2[i] => reverb;
    chickenBuf => reverb;
    // load sound
    me.dir() + "bell.wav" => bufs[i].read;
    me.dir() + "drum.wav" => bufs2[i].read;
    "special:dope" => chickenBuf.read;
    0 => bufs[i].gain;
    0 => bufs2[i].gain;
    0 => chickenBuf.gain;
}

// spork edit listener
spork ~ listenForEdit();

// simple sequencer loop
while( true )
{
    // play current
    playBell( currentStep, currGain, currRate );
    if( currChord == 1 )
    {
        if( currentStep % 3 == 0 ) playDrum( currentStep, currGain / 2, 1 );
        else if( currentStep % 3 == 1 ) playDrum( currentStep, currGain / 4, 1 );
        else if( currentStep % 3 == 2 ) playDrum( currentStep, currGain / 4, 1 );
    }
    currBeat => now;
    
    // rhythm change
    if( currBeat == 300::ms || currBeat == 200::ms )
    {
        playBell( currentStep, currGain, currRate );
        currBeat => now;
    }
    
    // increment to next step
    currentStep + 1 => currentStep;
    if( currentStep == NUM_STEPS ) 0 => currentStep;
}

fun void playBell( int whichStep, float gain, float rate )
{
    // restart
    0 => bufs[whichStep].pos;
    // set gain
    if( notes[whichStep] == " ") 0 => bufs[whichStep].gain;
    else gain => bufs[whichStep].gain;
    // set rate
    float adjustedRate;
    if( notes[whichStep] == "c" ) rate - 0.5 => adjustedRate;
    else if( notes[whichStep] == "d" ) rate - 0.45 => adjustedRate;
    else if( notes[whichStep] == "e" ) rate - 0.4 => adjustedRate;
    else if( notes[whichStep] == "f" ) rate - 0.34 => adjustedRate;
    else if( notes[whichStep] == "g" ) rate - 0.24 => adjustedRate;
    else if( notes[whichStep] == "A" ) rate - 0.2 => adjustedRate;
    else if( notes[whichStep] == "B" ) rate - 0.1 => adjustedRate;
    else if( notes[whichStep] == "C" ) rate => adjustedRate;
    else if( notes[whichStep] == "D" ) rate + 0.08 => adjustedRate;
    else if( notes[whichStep] == "E" ) rate + 0.2 => adjustedRate;
    adjustedRate => bufs[whichStep].rate;
}

fun void playDrum( int whichStep, float gain, float rate )
{
    // restart
    0 => bufs2[whichStep].pos;
    // set gain
    gain => bufs2[whichStep].gain;
    // set rate
    rate => bufs[whichStep].rate;
}

fun void playChicken( float gain, float rate )
{
    // restart
    0 => chickenBuf.pos;
    // set gain
    gain => chickenBuf.gain;
    // set rate
    rate => chickenBuf.rate;
    // pass time so that the file plays
	chickenBuf.length() / chickenBuf.rate() => now;
}

fun void playChickens( float gain, float rate )
{
    for( int i; i < 6; i++ )
    {
        playChicken( gain, rate );
    }
}

// this listens for events from Unity to update the sequence
fun void listenForEdit()
{
    while( true )
    {
        // wait for event
        editHappened => now;
        if( editOn == 1 )
        {
            if( editAttribute == "frequency1" ) currRate - 0.3 => currRate;
            else if( editAttribute == "dynamics" ) currGain + 1.5 => currGain;
            else if( editAttribute == "rhythm1" ) 
            {
                // double note
                if( currBeat == 400::ms ) 200::ms => currBeat;
                else if( currBeat == 600::ms ) 300::ms => currBeat;
            }
            else if( editAttribute == "frequency2" ) currRate + 0.15 => currRate;
            else if( editAttribute == "rhythm2" )
            {
                // speed up
                if( currBeat == 600::ms ) 400::ms => currBeat;
                else if( currBeat == 300::ms ) 200::ms => currBeat;
            }
            else if( editAttribute == "harmony" ) 1 => currChord;
        }
        else if( editOn == 0 )
        {
            if( editAttribute == "frequency1" ) currRate + 0.3 => currRate;
            else if( editAttribute == "dynamics" ) currGain - 1.5 => currGain;
            else if( editAttribute == "rhythm1" ) 
            {
                // single note
                if( currBeat == 200::ms ) 400::ms => currBeat;
                else if( currBeat == 300::ms ) 600::ms => currBeat;
            }
            else if( editAttribute == "frequency2" ) currRate - 0.15 => currRate;
            else if( editAttribute == "rhythm2" )
            {
                // slow down
                if( currBeat == 400::ms ) 600::ms => currBeat;
                else if( currBeat == 200::ms ) 300::ms => currBeat;
            }
            else if( editAttribute == "harmony" ) 0 => currChord;
        }
        else if( editOn == 2 ) // 1 low bok
        {
            playChicken( 0.25, 0.7 );
        }
        else if( editOn == 3 ) // 3 high boks
        {
            playChickens( 0.25, 1.3 );
        }
    }
}