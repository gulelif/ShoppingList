# ShoppingListCore
Projenin amacı, kullanıcıların alışveriş süreçlerini kolaylaştırmak adına almayı planladıkları ürünlerin
listelerini oluşturabilmesi ve bu listelerin takiplerini yapabilmesidir.
Uygulamanın kullanılabilmesi için üye olmak gerekmektedir.

• Kullanıcılar sisteme kayıtlı oldukları bilgilerle giriş yaptıktan sonra işlevsellikleri
kullanabileceklerdir.
• Kullanıcılar sisteme giriş yaptıktan sonra oluşturdukları alışveriş listelerini göreceklerdir. Buradan
istedikleri listeyi seçip ürün ekleme ekrana geçebileceklerdir.
• Ürün ekleyebilmeleri için listelenen ürünlerden istediklerine tıklayıp beklenen bilgileri girecek ve
ekle diyeceklerdir.
• Ürünlerin içerisinde ismiyle arama yapılabilecektir. İstenirse kategorisine göre de filtrelenecektir.
• Kullanıcılar listeleri sadece ürün ekleyerek değil, ürün kaldırarak da güncelleyebileceklerdir.
• Kullanıcı bir liste için “Alışverişe Çıkıyorum” seçeneğini işaretlediğinde artık o listeye ürün
ekleyemeyecektir.
• Kullanıcı ürünleri aldıkça liste üzerinden ilgili ürünü seçip “Aldım” diye işaretleyecektir. Liste için
“Alışveriş Tamamlandı” seçeneğini işaretlediğinde liste müdaheleye açık hale gelecektir.
• Sistemde yer alan ürünleri bilgileriyle beraber sistem yöneticisi ekleyecektir.

Kullanım Senaryoları
Üye Olma
Site açıldığında yer alan Üye Ol linki üzerinden gidilen sayfada ad, soyad, mail adresi, şifre ve şifre tekrar
alanları bulunur. Bu alanların hepsi zorunludur. Şifre en az 8 karakter olmalı ve büyük harf, küçük harf ve
rakam içermelidir. Kullanıcı bilgileri girdikten sonra üyeliği tamamla diyerek kaydını gerçekleştirir.

Üye Girişi Yapma
Site açıldığında yer alan giriş ekranı üzerinden sisteme giriş yapılır. Üyeler kayıt esnasında verdiklei mail
ve şifre bilgileriyle giriş sağlarlar. Admin kullanıcısı ise sistem ayağa kaldırılırken oluşturulan mail ve şifre
bilgileriyle sisteme giriş yapar.
Her mail adresine sadece bir kullanıcı hesabı tanımlıdır.

Admin Kullanıcısı Olma
Admin kullanıcısı kendisine sunulan menü üzerinden sistemde tanımlı ürün ve kategorilerle ilgili işlemleri
yürütür. Kategori ekleme, güncelleme ve kaldırma işlemlerini gerçekleştirir. Ürün ekleme, güncelleme ve
kaldırma işlemlerini yapar.
Kategorilerin sadece adı vardır ve bir ad sadece bir kategoriyi tanımlar.
Ürünlerin adı ve görseli vardır. Her ürünün bir kategorisi vardır. Sistemde aynı isimde tek bir ürün olur.
Ürün görseli belli bir markayı işaret etmemelidir. Görsel olarak ikonlar da kullanılabilir.

Liste Oluşturma ve Ürün Ekleme
Üyeler istedikleri kadar liste oluşturabilir. Listelerin sadece adı vardır.
Kullanıcılar ürün listesinden ürün seçerek listeye ürün eklerler. Seçilen ürün için isterlerse bir
açıklama(miktar, adet, marka vb. bilgiler için) girebilirler.
Listelerini görüntülediklerinde ürünlerin sadece adını ve sistemde kayıtlı görselini görürler. İsterlerse
detayına gidip ürün eklerken girdikleri açıklamayı görebilirler ve buradan bu bilgiyi değiştirebilirler.
Listede görüntülenen ürünlerden istediklerini kaldırabilirler.

Alışveriş Yapma
Üyeler listelerdeki ürünler için alışverişe çıktığında “Alışverişe Çıkıyorum” seçeneğini işaretlerler. Bu
işaretlemeyi yaptıktan sonra ilgili liste üzerinde oynama yapamazlar. Sadece görüntüleyebilirler.
Alışverişe çıkan üye, mağazada ürünleri aldıkça ilgili ürünü “Aldım” şeklinde işaretleyebilir.
Alışveriş bittiğinde ilgili liste için “Alışveriş Tamamlandı” seçeneğini işaretler. Böylece yeniden liste
üzerinde ürün ekleme, silme ve güncelleme işlemlerini yapabilirler. Ayrıca “Aldım” şeklinde işaretlediği
ürünler otomatik olarak listeden kaldırılır

Liste Silme
Üyeler kendi oluşturdukları listeleri istedikleri zaman tamamen silebilirler.
