using HastaneRandevu.Core.Models;
using HastaneRandevu.Core.Services;

namespace HastaneRandevu.App;

// Uygulamanin "ortak masasi": tum ekranlar ayni servisleri buradan kullanir.
// Veritabani olmadigi icin program acilirken ornek verilerle doldurulur.
public class UygulamaVerisi
{
    public KullaniciServisi KullaniciServisi { get; } = new();
    public KurumYonetimServisi YonetimServisi { get; } = new();
    public IRandevuServisi RandevuServisi { get; } = new RandevuServisi();

    public static UygulamaVerisi Olustur()
    {
        var veri = new UygulamaVerisi();
        veri.OrnekVeriYukle();
        return veri;
    }

    // Veriyi kendi servislerimizle (yetki kontrolleri dahil) olusturuyoruz, dogrudan listelere yazmiyoruz.
    private void OrnekVeriYukle()
    {
        int saatNo = 100;   // UygunlukAraligi numaralari icin sayac

        var sistem = new SistemYoneticisi(1, "Admin", "Sistem", "admin@hastane.com", "1234");
        KullaniciServisi.Kaydet(sistem);

        Kurum kurum = YonetimServisi.KurumTanimla(sistem, 1, "Merkez Hastanesi");
        var yonetici = new KurumYoneticisi(2, "Zeynep", "Ak", "zeynep@hastane.com", "1234", kurum);
        KullaniciServisi.Kaydet(yonetici);

        YonetimServisi.SubeEkle(yonetici, new Sube(1, "Merkez Şube", "Adapazarı, Sakarya"));
        foreach (var gun in new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday })
            YonetimServisi.CalismaSaatiEkle(yonetici, 1, new UygunlukAraligi(saatNo++, gun, new TimeOnly(8, 0), new TimeOnly(17, 0)));

        var kardiyoloji = new Bolum(1, "Kardiyoloji");
        var dahiliye = new Bolum(2, "Dahiliye");
        YonetimServisi.BolumEkle(yonetici, 1, kardiyoloji);
        YonetimServisi.BolumEkle(yonetici, 1, dahiliye);

        var kardMuayene = new Hizmet(1, "Kardiyoloji Muayenesi", TimeSpan.FromMinutes(30), 500m);
        var ekg = new Hizmet(2, "EKG", TimeSpan.FromMinutes(15), 250m);
        var dahMuayene = new Hizmet(3, "Dahiliye Muayenesi", TimeSpan.FromMinutes(20), 400m);
        YonetimServisi.HizmetEkle(yonetici, 1, kardMuayene);
        YonetimServisi.HizmetEkle(yonetici, 1, ekg);
        YonetimServisi.HizmetEkle(yonetici, 2, dahMuayene);

        var ayse = new Doktor(3, "Ayşe", "Kaya", "ayse@hastane.com", "1234", kardiyoloji, "Kardiyolog");
        var veli = new SaglikPersoneli(4, "Veli", "Demir", "veli@hastane.com", "1234", kardiyoloji, "Hemşire");
        var mehmet = new Doktor(5, "Mehmet", "Er", "mehmet@hastane.com", "1234", dahiliye, "Dahiliye Uzmanı");
        ayse.HizmetEkle(kardMuayene);
        veli.HizmetEkle(ekg);
        mehmet.HizmetEkle(dahMuayene);

        foreach (Calisan c in new Calisan[] { ayse, veli, mehmet })
        {
            YonetimServisi.CalisanEkle(yonetici, 1, c);
            KullaniciServisi.Kaydet(c);
        }

        // Her calisan kendi uygunlugunu kendisi ekler (sube saatleri icinde kalmak zorunda).
        foreach (var gun in new[] { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday })
            YonetimServisi.UygunlukEkle(ayse, ayse, new UygunlukAraligi(saatNo++, gun, new TimeOnly(9, 0), new TimeOnly(12, 0)));
        foreach (var gun in new[] { DayOfWeek.Tuesday, DayOfWeek.Thursday })
            YonetimServisi.UygunlukEkle(veli, veli, new UygunlukAraligi(saatNo++, gun, new TimeOnly(9, 0), new TimeOnly(15, 0)));
        foreach (var gun in new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday })
            YonetimServisi.UygunlukEkle(mehmet, mehmet, new UygunlukAraligi(saatNo++, gun, new TimeOnly(10, 0), new TimeOnly(16, 0)));

        KullaniciServisi.HastaKayitOl("Ali", "Yılmaz", "ali@mail.com", "1234", "5551234567");
    }
}
