namespace HastaneRandevu.Core.Enums;

// Sistemdeki her islem icin bir yetki. Kullanici.YetkiVarMi(...) bunlarla calisir.
public enum Yetki
{
    RandevuAl,
    RandevuIptalEt,
    RandevuOnayla,
    UygunlukDuzenle,
    CalisanYonet,
    HizmetYonet,
    KurumDuzenle,
    KurumTanimla
}
