﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BaseWebSite.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SurveyEntities : DbContext
    {
        public SurveyEntities()
            : base("name=SurveyEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<sbr_anket> sbr_anket { get; set; }
        public DbSet<sbr_anket_davet> sbr_anket_davet { get; set; }
        public DbSet<sbr_anket_durum_tarihcesi> sbr_anket_durum_tarihcesi { get; set; }
        public DbSet<sbr_anket_sorulari> sbr_anket_sorulari { get; set; }
        public DbSet<sbr_anket_test_mail_gonderi_kisi_tarihcesi> sbr_anket_test_mail_gonderi_kisi_tarihcesi { get; set; }
        public DbSet<sbr_anket_yayinlama_acik_anket_aktivasyon> sbr_anket_yayinlama_acik_anket_aktivasyon { get; set; }
        public DbSet<sbr_anket_yayinlama_mail_gonderi_aktivasyon> sbr_anket_yayinlama_mail_gonderi_aktivasyon { get; set; }
        public DbSet<sbr_anket_yayinlama_mail_gonderi_kisi_tarihcesi> sbr_anket_yayinlama_mail_gonderi_kisi_tarihcesi { get; set; }
        public DbSet<sbr_anket_yayinlama_mail_gonderi_tarihcesi> sbr_anket_yayinlama_mail_gonderi_tarihcesi { get; set; }
        public DbSet<sbr_mail_gruplari> sbr_mail_gruplari { get; set; }
        public DbSet<sbr_mail_gruplari_dosyalari> sbr_mail_gruplari_dosyalari { get; set; }
        public DbSet<sbr_mail_gruplari_kullanici_listesi> sbr_mail_gruplari_kullanici_listesi { get; set; }
        public DbSet<sbr_ref_anket_durumu> sbr_ref_anket_durumu { get; set; }
        public DbSet<sbr_ref_anket_kategori> sbr_ref_anket_kategori { get; set; }
        public DbSet<sbr_ref_anket_temalari> sbr_ref_anket_temalari { get; set; }
        public DbSet<sbr_ref_anket_test_sonucu> sbr_ref_anket_test_sonucu { get; set; }
        public DbSet<sbr_ref_anket_tipi> sbr_ref_anket_tipi { get; set; }
        public DbSet<sbr_ref_sablon_durumu> sbr_ref_sablon_durumu { get; set; }
        public DbSet<sbr_ref_soru_siralama_sekli> sbr_ref_soru_siralama_sekli { get; set; }
        public DbSet<sbr_ref_soru_sitili> sbr_ref_soru_sitili { get; set; }
        public DbSet<sbr_ref_soru_tipi> sbr_ref_soru_tipi { get; set; }
        public DbSet<sbr_sablon> sbr_sablon { get; set; }
        public DbSet<sbr_sablon_durum_tarihcesi> sbr_sablon_durum_tarihcesi { get; set; }
        public DbSet<sbr_sablon_soru_tipi_1_secenekleri> sbr_sablon_soru_tipi_1_secenekleri { get; set; }
        public DbSet<sbr_sablon_soru_tipi_10_secenekleri> sbr_sablon_soru_tipi_10_secenekleri { get; set; }
        public DbSet<sbr_sablon_soru_tipi_11_secenekleri> sbr_sablon_soru_tipi_11_secenekleri { get; set; }
        public DbSet<sbr_sablon_soru_tipi_2_secenekleri> sbr_sablon_soru_tipi_2_secenekleri { get; set; }
        public DbSet<sbr_sablon_soru_tipi_3_secenek_kolonlari> sbr_sablon_soru_tipi_3_secenek_kolonlari { get; set; }
        public DbSet<sbr_sablon_soru_tipi_3_secenekleri> sbr_sablon_soru_tipi_3_secenekleri { get; set; }
        public DbSet<sbr_sablon_soru_tipi_4_secenekleri> sbr_sablon_soru_tipi_4_secenekleri { get; set; }
        public DbSet<sbr_sablon_soru_tipi_5_secenekleri> sbr_sablon_soru_tipi_5_secenekleri { get; set; }
        public DbSet<sbr_sablon_soru_tipi_6_secenekleri> sbr_sablon_soru_tipi_6_secenekleri { get; set; }
        public DbSet<sbr_sablon_soru_tipi_7_secenekleri> sbr_sablon_soru_tipi_7_secenekleri { get; set; }
        public DbSet<sbr_sablon_soru_tipi_8_secenekleri> sbr_sablon_soru_tipi_8_secenekleri { get; set; }
        public DbSet<sbr_sablon_soru_tipi_9_secenekleri> sbr_sablon_soru_tipi_9_secenekleri { get; set; }
        public DbSet<sbr_sablon_sorulari> sbr_sablon_sorulari { get; set; }
        public DbSet<sbr_soru_tipi_1_cevaplari> sbr_soru_tipi_1_cevaplari { get; set; }
        public DbSet<sbr_soru_tipi_1_secenekleri> sbr_soru_tipi_1_secenekleri { get; set; }
        public DbSet<sbr_soru_tipi_10_cevaplari> sbr_soru_tipi_10_cevaplari { get; set; }
        public DbSet<sbr_soru_tipi_10_secenekleri> sbr_soru_tipi_10_secenekleri { get; set; }
        public DbSet<sbr_soru_tipi_11_cevaplari> sbr_soru_tipi_11_cevaplari { get; set; }
        public DbSet<sbr_soru_tipi_11_secenekleri> sbr_soru_tipi_11_secenekleri { get; set; }
        public DbSet<sbr_soru_tipi_2_cevaplari> sbr_soru_tipi_2_cevaplari { get; set; }
        public DbSet<sbr_soru_tipi_2_secenekleri> sbr_soru_tipi_2_secenekleri { get; set; }
        public DbSet<sbr_soru_tipi_3_cevaplari> sbr_soru_tipi_3_cevaplari { get; set; }
        public DbSet<sbr_soru_tipi_3_secenek_kolonlari> sbr_soru_tipi_3_secenek_kolonlari { get; set; }
        public DbSet<sbr_soru_tipi_3_secenekleri> sbr_soru_tipi_3_secenekleri { get; set; }
        public DbSet<sbr_soru_tipi_4_cevaplari> sbr_soru_tipi_4_cevaplari { get; set; }
        public DbSet<sbr_soru_tipi_4_secenekleri> sbr_soru_tipi_4_secenekleri { get; set; }
        public DbSet<sbr_soru_tipi_5_cevaplari> sbr_soru_tipi_5_cevaplari { get; set; }
        public DbSet<sbr_soru_tipi_5_secenekleri> sbr_soru_tipi_5_secenekleri { get; set; }
        public DbSet<sbr_soru_tipi_6_cevaplari> sbr_soru_tipi_6_cevaplari { get; set; }
        public DbSet<sbr_soru_tipi_6_secenekleri> sbr_soru_tipi_6_secenekleri { get; set; }
        public DbSet<sbr_soru_tipi_7_cevaplari> sbr_soru_tipi_7_cevaplari { get; set; }
        public DbSet<sbr_soru_tipi_7_secenekleri> sbr_soru_tipi_7_secenekleri { get; set; }
        public DbSet<sbr_soru_tipi_8_cevaplari> sbr_soru_tipi_8_cevaplari { get; set; }
        public DbSet<sbr_soru_tipi_8_secenekleri> sbr_soru_tipi_8_secenekleri { get; set; }
        public DbSet<sbr_soru_tipi_9_cevaplari> sbr_soru_tipi_9_cevaplari { get; set; }
        public DbSet<sbr_soru_tipi_9_secenekleri> sbr_soru_tipi_9_secenekleri { get; set; }
        public DbSet<sysdiagrams> sysdiagrams { get; set; }
    }
}