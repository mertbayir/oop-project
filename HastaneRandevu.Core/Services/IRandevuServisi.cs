using HastaneRandevu.Core.Models;

namespace HastaneRandevu.Core.Services;

// Arayuz katmani (WinForms) sadece bu sozlesmeyi bilir; kurallar RandevuServisi'nde.
// "islemYapan": giris yapmis kisi. Hasta mi doktor mu oldugunu servis yetki kontroluyle anlar.
public interface IRandevuServisi
{
    Randevu RandevuOlustur(Kullanici islemYapan, Calisan calisan, Hizmet hizmet, DateTime baslangic);
    IReadOnlyList<DateTime> MusaitSlotlariGetir(Calisan calisan, Hizmet hizmet, DateTime tarih);

    void Onayla(int randevuId, Kullanici islemYapan);
    void Reddet(int randevuId, Kullanici islemYapan);
    void Tamamla(int randevuId, Kullanici islemYapan);
    void IptalEt(int randevuId, Kullanici islemYapan);

    IReadOnlyList<Randevu> CalisaninRandevulari(Calisan calisan);
    IReadOnlyList<Randevu> HastaninRandevulari(Hasta hasta);
}
