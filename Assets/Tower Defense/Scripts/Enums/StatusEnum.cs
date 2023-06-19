public enum StatusEnum
{
    FREEZE, //MOVES AND ATTACK SLOWLY OR CANNOT MOVE NEITHER ATTACK (DEPENDS ON THE CONFIGURATION)
    BURN, //TAKES DAMAGE PER TURN
    PARALYZE, //TAKES DAMAGE PER TURN, CANNOT MOVE, CANNOT ATTACK, ITS POSITION GETS CRAZY FOR A FEW STEPS RANDOMLY (THEN RETURN TO THE INITIAL POSITION)
    DROWN, // TAKES DAMAGE PER TURN, CANNOT MOVE, CANNOT ATTACK, THE AFFECTED FLOATS ITS POSITION A FEW STEPS FROM THE GROUND (THEN RETURN TO THE INITIAL POSITION)
    CONFUSION, //ATTACK ITSELF AND MOVES RANDOMLY FOR THE DURATION OF THE CONFIGURATION
    POISON, //DAMAGED PER TURN BY CONFIGURED AMOUNT AND DURATION
    ASLEEP, //CANNOT MOVE, NEITHER ATTACK, IF HITTEN N" TIMES IT WILL AWAKE
    GROUNDED, //CANNOT MOVE (BUT CAN ATTACK)
    HEALBLOCK, //CANNOT HEAL FOR THE DURATION OF THE EFFECT
    TAUNT //AFFECTED MUST ATTACK THE AFFECTOR (IF AGRESSIVE ATTACK, IF NOT JUST STARE THE AFFECTOR)
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////