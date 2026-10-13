using HastaneRandevu.Core.Enums;

namespace HastaneRandevu.Core.Models;

// Kurumlari tanimlayan ust duzey yonetici (odev: kurumlar "yetki cercevesinde" tanimlanabilecek).
public class SistemYoneticisi : Kullanici
{
    public SistemYoneticisi(int id, string ad, string soyad, string email, string sifre)
        : base(id, ad, soyad, email, sifre)
    {
    }

    public override string RolAdi => "Sistem Yoneticisi";

    public override bool YetkiVarMi(Yetki yetki) => yetki == Yetki.KurumTanimla;
}
