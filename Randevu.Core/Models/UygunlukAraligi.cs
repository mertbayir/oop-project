namespace Randevu.Core.Models;

// Bir calisanin haftalik uygunluk araligi, orn: Pazartesi 09:00 - 12:00.
// Odev: "hastalar sadece bu zaman araliklarina randevu alabilecek".
// Degismez (immutable): olustuktan sonra alanlari degistirilemez, gecersiz ara durum olusmaz.
public class UygunlukAraligi
{
    public int Id { get; }
    public DayOfWeek Gun { get; }
    public TimeOnly Baslangic { get; }
    public TimeOnly Bitis { get; }

    public UygunlukAraligi(int id, DayOfWeek gun, TimeOnly baslangic, TimeOnly bitis)
    {
        if (bitis <= baslangic)
            throw new ArgumentException("Bitis saati baslangictan sonra olmali.", nameof(bitis));

        Id = id;
        Gun = gun;
        Baslangic = baslangic;
        Bitis = bitis;
    }

    // Verilen saat araligi bu uygunluk araliginin tamamen icinde mi?
    // Randevu bu araliga sigiyorsa alinabilir.
    public bool Kapsiyor(DayOfWeek gun, TimeOnly baslangic, TimeOnly bitis)
        => Gun == gun && Baslangic <= baslangic && bitis <= Bitis;

    // Ayni gun ve saatleri ortusen iki aralik mi? (Calisanin kendi uygunluklari cakismasin diye.)
    // Cakisma kurali: A, B'den once bitmiyor VE B, A'dan once bitmiyor.
    public bool Cakisiyor(UygunlukAraligi diger)
        => Gun == diger.Gun && Baslangic < diger.Bitis && diger.Baslangic < Bitis;

    public override string ToString() => $"{Gun} {Baslangic:HH\\:mm}-{Bitis:HH\\:mm}";
}
