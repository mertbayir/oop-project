using HastaneRandevu.Core.Models;

namespace HastaneRandevu.Core.Services;

// Arayuz katmani (WinForms) sadece bu sozlesmeyi bilir; kurallar RandevuServisi'nde.
public interface IRandevuServisi
{
    Randevu RandevuOlustur(Hasta hasta, Calisan calisan, Hizmet hizmet, DateTime baslangic);
    IReadOnlyList<DateTime> MusaitSlotlariGetir(Calisan calisan, Hizmet hizmet, DateTime tarih);

    void Onayla(int randevuId, Calisan calisan);
    void Reddet(int randevuId, Calisan calisan);
    void Tamamla(int randevuId, Calisan calisan);
    void IptalEt(int randevuId, Hasta hasta);

    IReadOnlyList<Randevu> CalisaninRandevulari(Calisan calisan);
    IReadOnlyList<Randevu> HastaninRandevulari(Hasta hasta);
}
