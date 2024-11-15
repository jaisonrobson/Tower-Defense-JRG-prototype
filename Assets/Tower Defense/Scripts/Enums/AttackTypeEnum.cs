public enum AttackTypeEnum
{
    MELEE, //Happens always next to the origin in an area of effect (the area of effect depends on each prefab)
    RANGED, //Travels to the target, if any collision happens before it ends (causing area of effect damage, the area of effect depends on each prefab)
    IMMEDIATE, //Instantaneous attack, happens directly on the target, without collision checks
    SIEGE //Happens continuously in a scheduled delayed time, always the same kind of attacks of Ranged attacks
}

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////