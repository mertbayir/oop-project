using HastaneRandevu.Core.Enums;

namespace HastaneRandevu.Core.Models;

// Bir hastanin bir calisandan aldigi randevu.
// Odev: "randevu detaylari (islem, sure, ucret, calisan, tarih/saat) kaydedilecek" ve "onay mekanizmasi olacak".
public class Randevu
{
    public int Id { get; }
    public Hasta Hasta { get; }
    public Calisan Calisan { get; }
    public Hizmet Hizmet { get; }
    public DateTime Baslangic { get; }

    // Sure ve Ucret hizmetten KOPYALANIR: hizmetin ucreti sonradan degisse de gecmis randevu degismez.
    public TimeSpan Sure { get; }
    public decimal Ucret { get; }

    // KAPSULLEME: Durum disaridan atanamaz, sadece asagidaki metodlarla ve izinli gecislerle degisir.
    public RandevuDurumu Durum { get; private set; } = RandevuDurumu.Beklemede;

    public DateTime Bitis => Baslangic + Sure;

    // Cakisma kontrolunde sayilir mi? (karar 6: Beklemede ve Onaylandi slotu tutar)
    public bool SlotuTutuyor => Durum == RandevuDurumu.Beklemede || Durum == RandevuDurumu.Onaylandi;

    public Randevu(int id, Hasta hasta, Calisan calisan, Hizmet hizmet, DateTime baslangic)
    {
        ArgumentNullException.ThrowIfNull(hasta);
        ArgumentNullException.ThrowIfNull(calisan);
        ArgumentNullException.ThrowIfNull(hizmet);

        if (!calisan.HizmetSunuyorMu(hizmet))
            throw new InvalidOperationException("Calisan bu hizmeti sunmuyor.");

        TimeOnly bas = TimeOnly.FromDateTime(baslangic);
        TimeOnly bit = bas.Add(hizmet.Sure);
        if (bit <= bas)
            throw new InvalidOperationException("Randevu gece yarisini asamaz.");

        // Odev: "hastalar sadece uygunluk zaman araliklarina randevu alabilecek".
        if (!calisan.UygunMu(baslangic.DayOfWeek, bas, bit))
            throw new InvalidOperationException("Calisan bu gun ve saatte uygun degil.");

        Id = id;
        Hasta = hasta;
        Calisan = calisan;
        Hizmet = hizmet;
        Baslangic = baslangic;
        Sure = hizmet.Sure;
        Ucret = hizmet.Ucret;
    }

    // Karar 5: randevuyu ilgili calisan onaylar. Baska bir calisan onaylayamaz.
    public void Onayla(Calisan onaylayan)
    {
        CalisanKontrol(onaylayan);
        GecisYap(RandevuDurumu.Onaylandi, RandevuDurumu.Beklemede);
    }

    public void Reddet(Calisan reddeden)
    {
        CalisanKontrol(reddeden);
        GecisYap(RandevuDurumu.Iptal, RandevuDurumu.Beklemede);
    }

    // Hasta sadece kendi randevusunu iptal edebilir.
    public void IptalEt(Hasta hasta)
    {
        ArgumentNullException.ThrowIfNull(hasta);
        if (hasta.Id != Hasta.Id)
            throw new InvalidOperationException("Sadece kendi randevunuzu iptal edebilirsiniz.");

        GecisYap(RandevuDurumu.Iptal, RandevuDurumu.Beklemede, RandevuDurumu.Onaylandi);
    }

    public void Tamamla(Calisan tamamlayan)
    {
        CalisanKontrol(tamamlayan);
        GecisYap(RandevuDurumu.Tamamlandi, RandevuDurumu.Onaylandi);
    }

    // CAKISMA KONTROLU (projenin kalbi): ayni calisan, ikisi de slotu tutuyor ve zamanlar ortusuyor.
    // Zaman kurali UygunlukAraligi.Cakisiyor ile ayni: A.Baslangic < B.Bitis && B.Baslangic < A.Bitis
    public bool Cakisiyor(Randevu diger)
    {
        ArgumentNullException.ThrowIfNull(diger);

        return Calisan.Id == diger.Calisan.Id
            && SlotuTutuyor && diger.SlotuTutuyor
            && ZamanOrtusuyor(diger.Baslangic, diger.Bitis);
    }

    // Verilen zaman araligi bu randevuyla ortusuyor mu? (Slot hesabi da bunu kullanir; formul tek yerde.)
    public bool ZamanOrtusuyor(DateTime baslangic, DateTime bitis)
        => Baslangic < bitis && baslangic < Bitis;

    private void CalisanKontrol(Calisan calisan)
    {
        ArgumentNullException.ThrowIfNull(calisan);
        if (calisan.Id != Calisan.Id)
            throw new InvalidOperationException("Bu islemi sadece randevunun calisani yapabilir.");
    }

    // Tek yerde durum gecis kurali: yeni durum, ancak izin verilen onceki durumlardan gelinebilir.
    private void GecisYap(RandevuDurumu yeni, params RandevuDurumu[] izinliOncekiler)
    {
        if (!izinliOncekiler.Contains(Durum))
            throw new InvalidOperationException($"{Durum} durumundaki randevu {yeni} durumuna gecemez.");

        Durum = yeni;
    }

    public override string ToString()
        => $"#{Id} {Baslangic:dd.MM.yyyy HH:mm}-{Bitis:HH:mm} {Hizmet.Ad} | {Hasta.TamAd} -> {Calisan.TamAd} | {Ucret:0.00} TL | {Durum}";
}
