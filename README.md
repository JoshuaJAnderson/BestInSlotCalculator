# What is the purpose of this repo?
This repo is made as an example of a different program I made. It loops through every combination of modifiers and figures out which combination would give the best dps (damage per second), dph (damage per hit), hps (heals per second).

## The math behind each:
- Dps is: Damage * RateOfFire * (1 + BaseCrit * CritStrength * CritPercentage) * MultiShot
- Dph is: Damage * (1 + BaseCit * CritStrength) * MultiShot
- Hps is: Damage * RateOfFire * (1 + BaseCrit * CritStrength * CritPercentage) * HealingPower
