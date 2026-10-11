# Algoritmalar - Kritik Mantık Açıklamaları

Bu dosya projede kullanılacak kritik algoritmaları ve karar mantıklarını
sade bir dille açıklamaktadır. Her algoritma, hocanın "bunu neden böyle yaptın?"
sorusuna cevap verebilecek şekilde belgelenmiştir.

---

## 1. RANDEVU ÇAKIŞMA KONTROL ALGORİTMASI

### Problem Nedir?
Bir doktora, aynı zaman aralığına birden fazla randevu verilmemelidir.
Çakışma kontrolü olmadan sistem anlamsız hale gelir.

### Nasıl Çalışır?
Yeni randevu: [yeniBaslangic, yeniBitis]
Mevcut randevu: [mevcutBaslangic, mevcutBitis]

Çakışma OLMAZ sadece şu iki durumda:
  a) Yeni randevu, mevcuttan tamamen önce biter:
     yeniBitis <= mevcutBaslangic
  b) Yeni randevu, mevcuttan tamamen sonra başlar:
     yeniBaslangic >= mevcutBitis

Diğer TÜM durumlarda çakışma VAR demektir.

### Pseudo-Kod
```
fonksiyon cakisiyorMu(yeniBaslangic, yeniBitis, mevcutBaslangic, mevcutBitis):
    EĞER yeniBitis <= mevcutBaslangic VEYA yeniBaslangic >= mevcutBitis:
        RETURN false  (çakışma yok)
    DEĞİLSE:
        RETURN true   (çakışma var)
```

### Neden Bu Yaklaşım?
- Tüm çakışma senaryolarını tek bir koşulla yakalar
- Okunması ve test edilmesi kolaydır
- Hatalı "içinde mi?" kontrolü yerine "dışında mı?" kontrolü daha güvenlidir

---

## 2. MÜSAİT SLOT HESAPLAMA ALGORİTMASI

### Problem Nedir?
Doktor belirli saatler arasında çalışıyor (örn: 09:00 - 17:00).
Bu süreyi belirli aralıklara (slotlara) bölüp, hangilerinin boş olduğunu bulmak gerekir.

### Nasıl Çalışır?
1. Doktorun o günkü çalışma saatlerini al
2. Başlangıçtan bitişe kadar hizmet süresi kadar adımlarla ilerle
3. Her adım için: bu slotta onaylanmış randevu var mı?
4. Varsa "dolu", yoksa "müsait" olarak işaretle
5. Müsait slotlar listesini döndür

### Pseudo-Kod
```
fonksiyon musaitSlaotlariGetir(doktor, tarih, hizmetSuresi):
    calisma = doktor.gunlukCalisma(tarih)
    EĞER calisma YOK:
        RETURN bos liste

    onaylananRandevular = randevularGetir(doktor, tarih)
    slotlar = []
    
    simdikiZaman = calisma.baslangic
    DÖNGÜ simdikiZaman + hizmetSuresi <= calisma.bitis:
        slotBitis = simdikiZaman + hizmetSuresi
        EĞER cakismaVar(simdikiZaman, slotBitis, onaylananRandevular) DEĞİLSE:
            slotlar.ekle(simdikiZaman)
        simdikiZaman = simdikiZaman + calisma.aralikDakika
    
    RETURN slotlar
```

---

## 3. RANDEVU OLUŞTURMA İŞ AKIŞI

### Adım Adım Akış
1. Hasta klinik/bölüm seçer
2. Hizmet türü seçer (muayene, laboratuvar vb.)
3. Doktor seçer (ya da sistem atar)
4. Tarih seçer
5. Sistem müsait slotları hesaplar (Algoritma 2)
6. Hasta saat seçer
7. Çakışma kontrolü tekrar yapılır (Algoritma 1) - güvenlik katmanı
8. Randevu BEKLEMEDE statüsünde oluşturulur
9. Doktora bildirim gönderilir
10. Doktor onaylarsa ONAYLANDI, reddederse İPTAL olur

---

## 4. ROL TABANLI ERİŞİM KONTROLÜ (RBAC)

### Problem Nedir?
Hasta, doktorun müsait saatlerini değiştirememelidir.
Doktor, başka doktorun randevularını görememelidir.
Yönetici her şeye erişebilmelidir.

### Nasıl Çalışır?
Her işlem için önce kullanıcının rolü kontrol edilir.
Rol uygun değilse işlem reddedilir (exception fırlatılır veya hata döner).

### Pseudo-Kod
```
fonksiyon yetkiKontrol(kullanici, gerekenRol):
    EĞER kullanici.rol != gerekenRol:
        EXCEPTION fırlat ("Bu işlem için yetkiniz yok")
```

---

## 5. PARA HESAPLAMA (ÜCRET)

### Nasıl Çalışır?
Randevunun ücreti, seçilen HizmetTuru'nun tanımlı ücretidir.
İleride indirim, sigorta indirimi vb. eklenirse bu metod güncellenir.

```
fonksiyon ucretHesapla(hizmetTuru):
    RETURN hizmetTuru.ucret
    // İleride: * indirimiUygula(hasta) gibi genişletilebilir
```

---

*Her algoritma kodlanmadan önce burada pseudo-kod olarak belgelenir, sonra koda dönüştürülür.*
