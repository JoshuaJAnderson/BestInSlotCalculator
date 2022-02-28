namespace Objects
{
  public enum TypeOfSearch
  {
    DAMAGE,
    DPS,
    HPS,
    TANK,
    BURST,
  }

  public struct Augmenter
  {
    public string augName;
    public string type;
    public int skill;
    public int tech;
    public double Damage,
      RoF,
      CritPerc,
      CritStr,
      Multifiring,
      TransPower,
      TransEff,
      ElecRegen,
      ShieldRegen,
      Energy,
      Shield,
      Resist,
      ElectricalTempering,
      WeaponHold;
  };

  public struct classStats
  {
    public string ClassName;
    public double Damage,
      RoF,
      CritPerc,
      CritStr,
      Multifiring,
      TransPower,
      TransEff,
      ElecRegen,
      ShieldRegen,
      Energy,
      Shield,
      Resist,
      ElectricalTempering,
      WeaponHold;
  };

  public struct AugType
  {
    public bool BASE,
      LF,
      HF,
      FREIGHTER,
      CAPITAL;
  }
}
